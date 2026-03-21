/**
 * Canvas layer interaction module — move, resize, rotate via mouse, zoom via wheel.
 * Runs entirely in the browser; final transforms are pushed to Blazor via JS interop.
 */

let _dotNetRef = null;
let _canvas = null;
let _zoom = 1;
let _canvasWidth = 0;
let _canvasHeight = 0;

// drag state
let _mode = null;        // 'move' | 'resize' | 'rotate'
let _activeEl = null;
let _handle = null;      // which resize handle
let _startX = 0;
let _startY = 0;
let _origLeft = 0;
let _origTop = 0;
let _origW = 0;
let _origH = 0;
let _origRotDeg = 0;
let _centerX = 0;
let _centerY = 0;

/**
 * Initialise canvas interactions.
 * @param {DotNetObjectReference} dotNetRef  Blazor component ref with OnTransformChanged callback
 * @param {string} canvasSelector            CSS selector for the .designer-canvas element
 * @param {number} zoom                      Current canvas zoom level
 * @param {number} canvasWidth               Canvas width in pixels
 * @param {number} canvasHeight              Canvas height in pixels
 */
export function init(dotNetRef, canvasSelector, zoom, canvasWidth, canvasHeight) {
    _dotNetRef = dotNetRef;
    _canvas = document.querySelector(canvasSelector);
    _zoom = zoom;
    _canvasWidth = canvasWidth;
    _canvasHeight = canvasHeight;
    if (!_canvas) return;

    _canvas.addEventListener('mousedown', onMouseDown);
    _canvas.addEventListener('wheel', onWheel, { passive: false });
    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);
}

/** Update the zoom level when Blazor changes it. */
export function setZoom(zoom) {
    _zoom = zoom;
}

/** Update canvas dimensions when template changes. */
export function setCanvasSize(w, h) {
    _canvasWidth = w;
    _canvasHeight = h;
}

/** Tear down event listeners. */
export function dispose() {
    if (_canvas) {
        _canvas.removeEventListener('mousedown', onMouseDown);
        _canvas.removeEventListener('wheel', onWheel);
    }
    document.removeEventListener('mousemove', onMouseMove);
    document.removeEventListener('mouseup', onMouseUp);
    _dotNetRef = null;
    _canvas = null;
}

// ── Helpers ──────────────────────────────────────────────

function getLayerEl(target) {
    return target.closest('.canvas-layer[data-layer-id]');
}

function parseNum(val) {
    return parseFloat(val) || 0;
}

function getTransformValues(el) {
    const style = el.style;
    return {
        left: parseNum(style.left),
        top: parseNum(style.top),
        width: parseNum(style.width) || el.offsetWidth,
        height: parseNum(style.height) || el.offsetHeight,
    };
}

function getRotation(el) {
    const m = el.style.transform?.match(/rotate\(([-\d.]+)deg\)/);
    return m ? parseFloat(m[1]) : 0;
}

function getScale(el) {
    const m = el.style.transform?.match(/scale\(([-\d.]+)\)/);
    return m ? parseFloat(m[1]) : 1;
}

// ── Mouse Down ───────────────────────────────────────────

function onMouseDown(e) {
    if (e.button !== 0) return;

    // Check for rotate handle
    const rotateHandle = e.target.closest('.layer-rotate-handle');
    if (rotateHandle) {
        const layerEl = getLayerEl(rotateHandle);
        if (!layerEl) return;
        e.preventDefault();
        e.stopPropagation();
        startRotate(layerEl, e);
        return;
    }

    // Check for resize handle
    const resizeHandle = e.target.closest('.layer-resize-handle');
    if (resizeHandle) {
        const layerEl = getLayerEl(resizeHandle);
        if (!layerEl) return;
        e.preventDefault();
        e.stopPropagation();
        startResize(layerEl, resizeHandle.dataset.handle, e);
        return;
    }

    // Move — click on the layer itself
    const layerEl = getLayerEl(e.target);
    if (!layerEl) return;
    // Don't move fullbleed layers
    if (layerEl.dataset.fullbleed === 'true') return;
    e.preventDefault();
    startMove(layerEl, e);
}

function startMove(el, e) {
    _mode = 'move';
    _activeEl = el;
    const tv = getTransformValues(el);
    _startX = e.clientX;
    _startY = e.clientY;
    _origLeft = tv.left;
    _origTop = tv.top;
    el.classList.add('canvas-layer-dragging');
}

function startResize(el, handle, e) {
    _mode = 'resize';
    _activeEl = el;
    _handle = handle;
    const tv = getTransformValues(el);
    _startX = e.clientX;
    _startY = e.clientY;
    _origLeft = tv.left;
    _origTop = tv.top;
    _origW = tv.width;
    _origH = tv.height;
    el.classList.add('canvas-layer-dragging');
}

function startRotate(el, e) {
    _mode = 'rotate';
    _activeEl = el;
    _origRotDeg = getRotation(el);
    const tv = getTransformValues(el);
    const rect = _canvas.getBoundingClientRect();
    _centerX = rect.left + (tv.left + tv.width / 2) * _zoom;
    _centerY = rect.top + (tv.top + tv.height / 2) * _zoom;
    _startX = e.clientX;
    _startY = e.clientY;
    el.classList.add('canvas-layer-dragging');
}

// ── Mouse Move ───────────────────────────────────────────

function onMouseMove(e) {
    if (!_mode || !_activeEl) return;
    e.preventDefault();

    const dx = (e.clientX - _startX) / _zoom;
    const dy = (e.clientY - _startY) / _zoom;

    if (_mode === 'move') {
        _activeEl.style.left = (_origLeft + dx) + 'px';
        _activeEl.style.top = (_origTop + dy) + 'px';
    }
    else if (_mode === 'resize') {
        applyResize(dx, dy);
    }
    else if (_mode === 'rotate') {
        const angleBefore = Math.atan2(_startY - _centerY, _startX - _centerX);
        const angleNow = Math.atan2(e.clientY - _centerY, e.clientX - _centerX);
        let deg = _origRotDeg + (angleNow - angleBefore) * (180 / Math.PI);
        // Snap to 15-degree increments when holding Shift
        if (e.shiftKey) deg = Math.round(deg / 15) * 15;
        setTransformProp(_activeEl, 'rotate', `rotate(${deg.toFixed(1)}deg)`);
    }
}

function applyResize(dx, dy) {
    let newL = _origLeft, newT = _origTop, newW = _origW, newH = _origH;
    const h = _handle;

    // Width changes
    if (h.includes('e')) { newW = Math.max(10, _origW + dx); }
    if (h.includes('w')) { newW = Math.max(10, _origW - dx); newL = _origLeft + dx; }

    // Height changes
    if (h.includes('s')) { newH = Math.max(10, _origH + dy); }
    if (h.includes('n')) { newH = Math.max(10, _origH - dy); newT = _origTop + dy; }

    // Shift = maintain aspect ratio (corner handles only)
    if (window.event?.shiftKey && (h === 'nw' || h === 'ne' || h === 'sw' || h === 'se')) {
        const ratio = _origW / _origH;
        if (Math.abs(dx) > Math.abs(dy)) {
            newH = newW / ratio;
            if (h.includes('n')) newT = _origTop + _origH - newH;
        } else {
            newW = newH * ratio;
            if (h.includes('w')) newL = _origLeft + _origW - newW;
        }
    }

    _activeEl.style.left = newL + 'px';
    _activeEl.style.top = newT + 'px';
    _activeEl.style.width = newW + 'px';
    _activeEl.style.height = newH + 'px';
}

function setTransformProp(el, name, value) {
    let t = el.style.transform || '';
    const regex = new RegExp(name + '\\([^)]*\\)');
    if (regex.test(t)) {
        t = t.replace(regex, value);
    } else {
        t = t.trim() + ' ' + value;
    }
    el.style.transform = t.trim();
}

// ── Mouse Up ─────────────────────────────────────────────

function onMouseUp(e) {
    if (!_mode || !_activeEl) return;

    _activeEl.classList.remove('canvas-layer-dragging');

    const layerId = parseInt(_activeEl.dataset.layerId, 10);
    const tv = getTransformValues(_activeEl);
    const rotation = getRotation(_activeEl);
    const scale = getScale(_activeEl);

    _mode = null;
    _activeEl = null;
    _handle = null;

    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync('OnTransformChanged', layerId,
            tv.left, tv.top, tv.width, tv.height, rotation, scale);
    }
}

// ── Wheel (layer zoom) ──────────────────────────────────

function onWheel(e) {
    const layerEl = getLayerEl(e.target);
    if (!layerEl) return;
    if (layerEl.dataset.fullbleed === 'true') return;

    e.preventDefault();
    e.stopPropagation();

    const delta = e.deltaY > 0 ? -0.05 : 0.05;
    let currentScale = getScale(layerEl);
    currentScale = Math.max(0.05, Math.min(10, currentScale + delta));
    setTransformProp(layerEl, 'scale', `scale(${currentScale.toFixed(4)})`);

    const layerId = parseInt(layerEl.dataset.layerId, 10);
    const tv = getTransformValues(layerEl);
    const rotation = getRotation(layerEl);

    if (_dotNetRef) {
        _dotNetRef.invokeMethodAsync('OnTransformChanged', layerId,
            tv.left, tv.top, tv.width, tv.height, rotation, currentScale);
    }
}
