window.socialMotiveTheme = (function () {
    const storageKey = "sm.theme";

    function normalize(theme, fallbackTheme = "light") {
        if (!theme) {
            return fallbackTheme;
        }

        return theme.toString().toLowerCase();
    }

    function apply(theme, stylesheetUrl) {
        const normalizedTheme = normalize(theme);
        const link = document.getElementById("sm-kendo-theme");

        if (link && stylesheetUrl && link.getAttribute("href") !== stylesheetUrl) {
            link.setAttribute("href", stylesheetUrl);
        }

        document.body.classList.remove("sm-theme-light", "sm-theme-dark");
        document.body.classList.add(`sm-theme-${normalizedTheme}`);
        document.body.setAttribute("data-theme", normalizedTheme);
        localStorage.setItem(storageKey, normalizedTheme);

        return normalizedTheme;
    }

    function get(fallbackTheme = "light") {
        return normalize(localStorage.getItem(storageKey), fallbackTheme);
    }

    return {
        apply,
        get
    };
})();
