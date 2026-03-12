using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMotive.WebApp.Data;
using SocialMotive.WebApp.Models;
using SocialMotive.WebApp.Models.Admin.Dtos;

namespace SocialMotive.WebApp.Controllers
{
    /// <summary>
    /// Admin Controller for managing all whitelisted database tables.
    /// Endpoints: GET, POST (Create), PUT (Update), DELETE
    /// All operations require AdminFull role.
    /// </summary>
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "AdminFull")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly SocialMotiveDbContext _dbContext;

        public AdminController(
            ILogger<AdminController> logger,
            SocialMotiveDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        #region Users

        /// <summary>
        /// Get paginated list of users
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<PaginatedResponse<UserDto>>> GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalCount = await _dbContext.Users.CountAsync();
                var users = await _dbContext.Users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        MobilePhone = u.MobilePhone,
                        Bio = u.Bio
                    })
                    .ToListAsync();

                return Ok(PaginatedResponse<UserDto>.SuccessResponse(users, pageNumber, pageSize, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return BadRequest(Response<UserDto>.FailureResponse("Error retrieving users", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Get single user by ID
        /// </summary>
        [HttpGet("users/{id:guid}")]
        public async Task<ActionResult<Response<UserDto>>> GetUser(Guid id)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(id);
                if (user == null)
                    return NotFound(Response<UserDto>.FailureResponse("User not found"));

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.MobilePhone,
                    Bio = user.Bio
                };

                return Ok(Response<UserDto>.SuccessResponse(userDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return BadRequest(Response<UserDto>.FailureResponse("Error retrieving user", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Create new user
        /// </summary>
        [HttpPost("users")]
        public async Task<ActionResult<Response<UserDto>>> CreateUser([FromBody] UserDto userDto)
        {
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    MobilePhone = userDto.MobilePhone,
                    Bio = userDto.Bio,
                    PasswordHash = "default_hash", // TODO: Implement proper password hashing
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                var resultDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.MobilePhone,
                    Bio = user.Bio
                };

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, Response<UserDto>.SuccessResponse(resultDto, "User created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return BadRequest(Response<UserDto>.FailureResponse("Error creating user", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Update user
        /// </summary>
        [HttpPut("users/{id:guid}")]
        public async Task<ActionResult<Response<UserDto>>> UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(id);
                if (user == null)
                    return NotFound(Response<UserDto>.FailureResponse("User not found"));

                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Email = userDto.Email;
                user.MobilePhone = userDto.MobilePhone;
                user.Bio = userDto.Bio;
                user.Modified = DateTime.UtcNow;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                var resultDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.MobilePhone,
                    Bio = user.Bio
                };

                return Ok(Response<UserDto>.SuccessResponse(resultDto, "User updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return BadRequest(Response<UserDto>.FailureResponse("Error updating user", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        [HttpDelete("users/{id:guid}")]
        public async Task<ActionResult<Models.Response>> DeleteUser(Guid id)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(id);
                if (user == null)
                    return NotFound(Models.Response.FailureResponse("User not found"));

                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();

                return Ok(Models.Response.SuccessResponse(null, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return BadRequest(Models.Response.FailureResponse("Error deleting user", new[] { ex.Message }));
            }
        }

        #endregion

        #region Trackers

        /// <summary>
        /// Get paginated list of trackers
        /// </summary>
        [HttpGet("trackers")]
        public async Task<ActionResult<PaginatedResponse<TrackerDto>>> GetTrackers(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalCount = await _dbContext.Trackers.CountAsync();
                var trackers = await _dbContext.Trackers
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TrackerDto
                    {
                        TrackerId = t.TrackerId,
                        DisplayName = t.DisplayName,
                        Phone = t.Phone,
                        Email = t.Email,
                        Mobile = t.Mobile,
                        LicensePlate = t.LicensePlate,
                        GroupId = t.GroupId,
                        TrackerRoleId = t.TrackerRoleId,
                        CityId = t.CityId,
                        IsAdmin = t.IsAdmin ?? false,
                        InviteId = t.InviteId,
                        UserId = t.UserId,
                        CheckIn = t.CheckIn,
                        CreatedAt = t.CreatedAt,
                        ModifiedAt = t.ModifiedAt,
                        JoinedAt = t.JoinedAt
                    })
                    .ToListAsync();

                return Ok(PaginatedResponse<TrackerDto>.SuccessResponse(trackers, pageNumber, pageSize, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trackers");
                return BadRequest(Response<TrackerDto>.FailureResponse("Error retrieving trackers", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Get single tracker by ID
        /// </summary>
        [HttpGet("trackers/{id:int}")]
        public async Task<ActionResult<Response<TrackerDto>>> GetTracker(int id)
        {
            try
            {
                var tracker = await _dbContext.Trackers.FindAsync(id);
                if (tracker == null)
                    return NotFound(Response<TrackerDto>.FailureResponse("Tracker not found"));

                var trackerDto = new TrackerDto
                {
                    TrackerId = tracker.TrackerId,
                    DisplayName = tracker.DisplayName,
                    Phone = tracker.Phone,
                    Email = tracker.Email,
                    Mobile = tracker.Mobile,
                    LicensePlate = tracker.LicensePlate,
                    GroupId = tracker.GroupId,
                    TrackerRoleId = tracker.TrackerRoleId,
                    CityId = tracker.CityId,
                    IsAdmin = tracker.IsAdmin ?? false,
                    InviteId = tracker.InviteId,
                    UserId = tracker.UserId,
                    CheckIn = tracker.CheckIn,
                    CreatedAt = tracker.CreatedAt,
                    ModifiedAt = tracker.ModifiedAt,
                    JoinedAt = tracker.JoinedAt
                };

                return Ok(Response<TrackerDto>.SuccessResponse(trackerDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tracker {TrackerId}", id);
                return BadRequest(Response<TrackerDto>.FailureResponse("Error retrieving tracker", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Create new tracker
        /// </summary>
        [HttpPost("trackers")]
        public async Task<ActionResult<Response<TrackerDto>>> CreateTracker([FromBody] TrackerDto trackerDto)
        {
            try
            {
                var tracker = new Tracker
                {
                    DisplayName = trackerDto.DisplayName,
                    Phone = trackerDto.Phone,
                    Email = trackerDto.Email,
                    Mobile = trackerDto.Mobile,
                    LicensePlate = trackerDto.LicensePlate,
                    GroupId = trackerDto.GroupId,
                    TrackerRoleId = trackerDto.TrackerRoleId,
                    CityId = trackerDto.CityId,
                    IsAdmin = trackerDto.IsAdmin,
                    InviteId = trackerDto.InviteId,
                    UserId = trackerDto.UserId,
                    CheckIn = trackerDto.CheckIn,
                    InviteCode = Guid.NewGuid(),
                    QrGuid = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    JoinedAt = DateTime.UtcNow
                };

                _dbContext.Trackers.Add(tracker);
                await _dbContext.SaveChangesAsync();

                var resultDto = new TrackerDto
                {
                    TrackerId = tracker.TrackerId,
                    DisplayName = tracker.DisplayName,
                    Phone = tracker.Phone,
                    Email = tracker.Email,
                    Mobile = tracker.Mobile,
                    LicensePlate = tracker.LicensePlate,
                    GroupId = tracker.GroupId,
                    TrackerRoleId = tracker.TrackerRoleId,
                    CityId = tracker.CityId,
                    IsAdmin = tracker.IsAdmin ?? false,
                    InviteId = tracker.InviteId,
                    UserId = tracker.UserId,
                    CheckIn = tracker.CheckIn,
                    CreatedAt = tracker.CreatedAt,
                    ModifiedAt = tracker.ModifiedAt,
                    JoinedAt = tracker.JoinedAt
                };

                return CreatedAtAction(nameof(GetTracker), new { id = tracker.TrackerId }, Response<TrackerDto>.SuccessResponse(resultDto, "Tracker created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tracker");
                return BadRequest(Response<TrackerDto>.FailureResponse("Error creating tracker", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Update tracker
        /// </summary>
        [HttpPut("trackers/{id:int}")]
        public async Task<ActionResult<TrackerUpdateResponse>> UpdateTracker(int id, [FromBody] TrackerUpdateDto trackerDto)
        {
            try
            {
                var tracker = await _dbContext.Trackers.FindAsync(id);
                if (tracker == null)
                    return NotFound(TrackerUpdateResponse.FailureResponse("Tracker not found"));

                tracker.DisplayName = trackerDto.DisplayName;
                tracker.Phone = trackerDto.Phone;
                tracker.Email = trackerDto.Email;
                tracker.Mobile = trackerDto.Mobile;
                tracker.LicensePlate = trackerDto.LicensePlate;
                tracker.GroupId = trackerDto.GroupId;
                tracker.TrackerRoleId = trackerDto.TrackerRoleId;
                tracker.CityId = trackerDto.CityId;
                tracker.IsAdmin = trackerDto.IsAdmin;
                tracker.InviteId = trackerDto.InviteId;
                tracker.UserId = trackerDto.UserId;
                tracker.CheckIn = trackerDto.CheckIn;
                tracker.ModifiedAt = DateTime.UtcNow;

                _dbContext.Trackers.Update(tracker);
                await _dbContext.SaveChangesAsync();

                var resultDto = new TrackerDto
                {
                    TrackerId = tracker.TrackerId,
                    DisplayName = tracker.DisplayName,
                    Phone = tracker.Phone,
                    Email = tracker.Email,
                    Mobile = tracker.Mobile,
                    LicensePlate = tracker.LicensePlate,
                    GroupId = tracker.GroupId,
                    TrackerRoleId = tracker.TrackerRoleId,
                    CityId = tracker.CityId,
                    IsAdmin = tracker.IsAdmin ?? false,
                    InviteId = tracker.InviteId,
                    UserId = tracker.UserId,
                    CheckIn = tracker.CheckIn,
                    CreatedAt = tracker.CreatedAt,
                    ModifiedAt = tracker.ModifiedAt,
                    JoinedAt = tracker.JoinedAt
                };

                return Ok(TrackerUpdateResponse.SuccessResponse(resultDto, "Tracker updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tracker {TrackerId}", id);
                return BadRequest(TrackerUpdateResponse.FailureResponse("Error updating tracker", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Delete tracker
        /// </summary>
        [HttpDelete("trackers/{id:int}")]
        public async Task<ActionResult<Models.Response>> DeleteTracker(int id)
        {
            try
            {
                var tracker = await _dbContext.Trackers.FindAsync(id);
                if (tracker == null)
                    return NotFound(Models.Response.FailureResponse("Tracker not found"));

                _dbContext.Trackers.Remove(tracker);
                await _dbContext.SaveChangesAsync();

                return Ok(Models.Response.SuccessResponse(null, "Tracker deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tracker {TrackerId}", id);
                return BadRequest(Models.Response.FailureResponse("Error deleting tracker", new[] { ex.Message }));
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Get paginated list of events
        /// </summary>
        [HttpGet("events")]
        public async Task<ActionResult<PaginatedResponse<EventDto>>> GetEvents(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalCount = await _dbContext.Events.CountAsync();
                var events = await _dbContext.Events
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new EventDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        EventTypeId = e.EventTypeId,
                        Status = e.Status,
                        OrganizerId = e.OrganizerId,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Address = e.Address,
                        Latitude = e.Latitude,
                        Longitude = e.Longitude,
                        City = e.City,
                        MaxParticipants = e.MaxParticipants,
                        MinParticipants = e.MinParticipants,
                        HoursEstimate = e.HoursEstimate,
                        SkillsRequired = e.SkillsRequired,
                        BenefitsDescription = e.BenefitsDescription,
                        RewardPoints = e.RewardPoints,
                        IsVerified = e.IsVerified ?? false,
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt,
                        PublishedAt = e.PublishedAt
                    })
                    .ToListAsync();

                return Ok(PaginatedResponse<EventDto>.SuccessResponse(events, pageNumber, pageSize, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting events");
                return BadRequest(Response<EventDto>.FailureResponse("Error retrieving events", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Get single event by ID
        /// </summary>
        [HttpGet("events/{id:int}")]
        public async Task<ActionResult<Response<EventDto>>> GetEvent(int id)
        {
            try
            {
                var @event = await _dbContext.Events.FindAsync(id);
                if (@event == null)
                    return NotFound(Response<EventDto>.FailureResponse("Event not found"));

                var eventDto = new EventDto
                {
                    Id = @event.Id,
                    Title = @event.Title,
                    Description = @event.Description,
                    EventTypeId = @event.EventTypeId,
                    Status = @event.Status,
                    OrganizerId = @event.OrganizerId,
                    StartDate = @event.StartDate,
                    EndDate = @event.EndDate,
                    Address = @event.Address,
                    Latitude = @event.Latitude,
                    Longitude = @event.Longitude,
                    City = @event.City,
                    MaxParticipants = @event.MaxParticipants,
                    MinParticipants = @event.MinParticipants,
                    HoursEstimate = @event.HoursEstimate,
                    SkillsRequired = @event.SkillsRequired,
                    BenefitsDescription = @event.BenefitsDescription,
                    RewardPoints = @event.RewardPoints,
                    IsVerified = @event.IsVerified ?? false,
                    CreatedAt = @event.CreatedAt,
                    UpdatedAt = @event.UpdatedAt,
                    PublishedAt = @event.PublishedAt
                };

                return Ok(Response<EventDto>.SuccessResponse(eventDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event {EventId}", id);
                return BadRequest(Response<EventDto>.FailureResponse("Error retrieving event", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Create new event
        /// </summary>
        [HttpPost("events")]
        public async Task<ActionResult<Response<EventDto>>> CreateEvent([FromBody] EventDto eventDto)
        {
            try
            {
                var @event = new Event
                {
                    Title = eventDto.Title,
                    Description = eventDto.Description,
                    EventTypeId = eventDto.EventTypeId,
                    Status = eventDto.Status,
                    OrganizerId = eventDto.OrganizerId,
                    StartDate = eventDto.StartDate,
                    EndDate = eventDto.EndDate,
                    Address = eventDto.Address,
                    Latitude = eventDto.Latitude,
                    Longitude = eventDto.Longitude,
                    City = eventDto.City,
                    MaxParticipants = eventDto.MaxParticipants,
                    MinParticipants = eventDto.MinParticipants,
                    HoursEstimate = eventDto.HoursEstimate,
                    SkillsRequired = eventDto.SkillsRequired,
                    BenefitsDescription = eventDto.BenefitsDescription,
                    RewardPoints = eventDto.RewardPoints,
                    IsVerified = eventDto.IsVerified,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.Events.Add(@event);
                await _dbContext.SaveChangesAsync();

                var resultDto = new EventDto
                {
                    Id = @event.Id,
                    Title = @event.Title,
                    Description = @event.Description,
                    EventTypeId = @event.EventTypeId,
                    Status = @event.Status,
                    OrganizerId = @event.OrganizerId,
                    StartDate = @event.StartDate,
                    EndDate = @event.EndDate,
                    Address = @event.Address,
                    Latitude = @event.Latitude,
                    Longitude = @event.Longitude,
                    City = @event.City,
                    MaxParticipants = @event.MaxParticipants,
                    MinParticipants = @event.MinParticipants,
                    HoursEstimate = @event.HoursEstimate,
                    SkillsRequired = @event.SkillsRequired,
                    BenefitsDescription = @event.BenefitsDescription,
                    RewardPoints = @event.RewardPoints,
                    IsVerified = @event.IsVerified ?? false,
                    CreatedAt = @event.CreatedAt,
                    UpdatedAt = @event.UpdatedAt,
                    PublishedAt = @event.PublishedAt
                };

                return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, Response<EventDto>.SuccessResponse(resultDto, "Event created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return BadRequest(Response<EventDto>.FailureResponse("Error creating event", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Update event
        /// </summary>
        [HttpPut("events/{id:int}")]
        public async Task<ActionResult<Response<EventDto>>> UpdateEvent(int id, [FromBody] EventDto eventDto)
        {
            try
            {
                var @event = await _dbContext.Events.FindAsync(id);
                if (@event == null)
                    return NotFound(Response<EventDto>.FailureResponse("Event not found"));

                @event.Title = eventDto.Title;
                @event.Description = eventDto.Description;
                @event.EventTypeId = eventDto.EventTypeId;
                @event.Status = eventDto.Status;
                @event.StartDate = eventDto.StartDate;
                @event.EndDate = eventDto.EndDate;
                @event.Address = eventDto.Address;
                @event.Latitude = eventDto.Latitude;
                @event.Longitude = eventDto.Longitude;
                @event.City = eventDto.City;
                @event.MaxParticipants = eventDto.MaxParticipants;
                @event.MinParticipants = eventDto.MinParticipants;
                @event.HoursEstimate = eventDto.HoursEstimate;
                @event.SkillsRequired = eventDto.SkillsRequired;
                @event.BenefitsDescription = eventDto.BenefitsDescription;
                @event.RewardPoints = eventDto.RewardPoints;
                @event.IsVerified = eventDto.IsVerified;
                @event.UpdatedAt = DateTime.UtcNow;

                _dbContext.Events.Update(@event);
                await _dbContext.SaveChangesAsync();

                var resultDto = new EventDto
                {
                    Id = @event.Id,
                    Title = @event.Title,
                    Description = @event.Description,
                    EventTypeId = @event.EventTypeId,
                    Status = @event.Status,
                    OrganizerId = @event.OrganizerId,
                    StartDate = @event.StartDate,
                    EndDate = @event.EndDate,
                    Address = @event.Address,
                    Latitude = @event.Latitude,
                    Longitude = @event.Longitude,
                    City = @event.City,
                    MaxParticipants = @event.MaxParticipants,
                    MinParticipants = @event.MinParticipants,
                    HoursEstimate = @event.HoursEstimate,
                    SkillsRequired = @event.SkillsRequired,
                    BenefitsDescription = @event.BenefitsDescription,
                    RewardPoints = @event.RewardPoints,
                    IsVerified = @event.IsVerified ?? false,
                    CreatedAt = @event.CreatedAt,
                    UpdatedAt = @event.UpdatedAt,
                    PublishedAt = @event.PublishedAt
                };

                return Ok(Response<EventDto>.SuccessResponse(resultDto, "Event updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {EventId}", id);
                return BadRequest(Response<EventDto>.FailureResponse("Error updating event", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Delete event
        /// </summary>
        [HttpDelete("events/{id:int}")]
        public async Task<ActionResult<Models.Response>> DeleteEvent(int id)
        {
            try
            {
                var @event = await _dbContext.Events.FindAsync(id);
                if (@event == null)
                    return NotFound(Models.Response.FailureResponse("Event not found"));

                _dbContext.Events.Remove(@event);
                await _dbContext.SaveChangesAsync();

                return Ok(Models.Response.SuccessResponse(null, "Event deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event {EventId}", id);
                return BadRequest(Models.Response.FailureResponse("Error deleting event", new[] { ex.Message }));
            }
        }

        #endregion

        #region Labels

        /// <summary>
        /// Get paginated list of labels
        /// </summary>
        [HttpGet("labels")]
        public async Task<ActionResult<PaginatedResponse<LabelDto>>> GetLabels(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalCount = await _dbContext.Labels.CountAsync();
                var labels = await _dbContext.Labels
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new LabelDto
                    {
                        LabelId = l.LabelId,
                        Name = l.Name,
                        ColorHex = l.ColorHex,
                        IconType = l.IconType,
                        LabelType = l.LabelType,
                        Publish = l.Publish,
                        Level = l.Level
                    })
                    .ToListAsync();

                return Ok(PaginatedResponse<LabelDto>.SuccessResponse(labels, pageNumber, pageSize, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting labels");
                return BadRequest(Response<LabelDto>.FailureResponse("Error retrieving labels", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Get single label by ID
        /// </summary>
        [HttpGet("labels/{id:int}")]
        public async Task<ActionResult<Response<LabelDto>>> GetLabel(int id)
        {
            try
            {
                var label = await _dbContext.Labels.FindAsync(id);
                if (label == null)
                    return NotFound(Response<LabelDto>.FailureResponse("Label not found"));

                var labelDto = new LabelDto
                {
                    LabelId = label.LabelId,
                    Name = label.Name,
                    ColorHex = label.ColorHex,
                    IconType = label.IconType,
                    LabelType = label.LabelType,
                    Publish = label.Publish,
                    Level = label.Level
                };

                return Ok(Response<LabelDto>.SuccessResponse(labelDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting label {LabelId}", id);
                return BadRequest(Response<LabelDto>.FailureResponse("Error retrieving label", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Create new label
        /// </summary>
        [HttpPost("labels")]
        public async Task<ActionResult<Response<LabelDto>>> CreateLabel([FromBody] LabelDto labelDto)
        {
            try
            {
                var label = new Label
                {
                    Name = labelDto.Name,
                    ColorHex = labelDto.ColorHex,
                    IconType = labelDto.IconType,
                    LabelType = labelDto.LabelType,
                    Publish = labelDto.Publish,
                    Level = labelDto.Level
                };

                _dbContext.Labels.Add(label);
                await _dbContext.SaveChangesAsync();

                var resultDto = new LabelDto
                {
                    LabelId = label.LabelId,
                    Name = label.Name,
                    ColorHex = label.ColorHex,
                    IconType = label.IconType,
                    LabelType = label.LabelType,
                    Publish = label.Publish,
                    Level = label.Level
                };

                return CreatedAtAction(nameof(GetLabel), new { id = label.LabelId }, Response<LabelDto>.SuccessResponse(resultDto, "Label created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating label");
                return BadRequest(Response<LabelDto>.FailureResponse("Error creating label", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Update label
        /// </summary>
        [HttpPut("labels/{id:int}")]
        public async Task<ActionResult<Response<LabelDto>>> UpdateLabel(int id, [FromBody] LabelDto labelDto)
        {
            try
            {
                var label = await _dbContext.Labels.FindAsync(id);
                if (label == null)
                    return NotFound(Response<LabelDto>.FailureResponse("Label not found"));

                label.Name = labelDto.Name;
                label.ColorHex = labelDto.ColorHex;
                label.IconType = labelDto.IconType;
                label.LabelType = labelDto.LabelType;
                label.Publish = labelDto.Publish;
                label.Level = labelDto.Level;

                _dbContext.Labels.Update(label);
                await _dbContext.SaveChangesAsync();

                var resultDto = new LabelDto
                {
                    LabelId = label.LabelId,
                    Name = label.Name,
                    ColorHex = label.ColorHex,
                    IconType = label.IconType,
                    LabelType = label.LabelType,
                    Publish = label.Publish,
                    Level = label.Level
                };

                return Ok(Response<LabelDto>.SuccessResponse(resultDto, "Label updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating label {LabelId}", id);
                return BadRequest(Response<LabelDto>.FailureResponse("Error updating label", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Delete label
        /// </summary>
        [HttpDelete("labels/{id:int}")]
        public async Task<ActionResult<Models.Response>> DeleteLabel(int id)
        {
            try
            {
                var label = await _dbContext.Labels.FindAsync(id);
                if (label == null)
                    return NotFound(Models.Response.FailureResponse("Label not found"));

                _dbContext.Labels.Remove(label);
                await _dbContext.SaveChangesAsync();

                return Ok(Models.Response.SuccessResponse(null, "Label deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting label {LabelId}", id);
                return BadRequest(Models.Response.FailureResponse("Error deleting label", new[] { ex.Message }));
            }
        }

        #endregion

        #region EventTypes

        /// <summary>
        /// Get paginated list of event types
        /// </summary>
        [HttpGet("eventtypes")]
        public async Task<ActionResult<PaginatedResponse<EventTypeDto>>> GetEventTypes(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalCount = await _dbContext.EventTypes.CountAsync();
                var eventTypes = await _dbContext.EventTypes
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(et => new EventTypeDto
                    {
                        Id = et.Id,
                        Name = et.Name,
                        Description = et.Description,
                        Icon = et.Icon,
                        Color = et.Color,
                        Created = et.Created
                    })
                    .ToListAsync();

                return Ok(PaginatedResponse<EventTypeDto>.SuccessResponse(eventTypes, pageNumber, pageSize, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event types");
                return BadRequest(Response<EventTypeDto>.FailureResponse("Error retrieving event types", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Get single event type by ID
        /// </summary>
        [HttpGet("eventtypes/{id:int}")]
        public async Task<ActionResult<Response<EventTypeDto>>> GetEventType(int id)
        {
            try
            {
                var eventType = await _dbContext.EventTypes.FindAsync(id);
                if (eventType == null)
                    return NotFound(Response<EventTypeDto>.FailureResponse("Event type not found"));

                var eventTypeDto = new EventTypeDto
                {
                    Id = eventType.Id,
                    Name = eventType.Name,
                    Description = eventType.Description,
                    Icon = eventType.Icon,
                    Color = eventType.Color,
                    Created = eventType.Created
                };

                return Ok(Response<EventTypeDto>.SuccessResponse(eventTypeDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting event type {EventTypeId}", id);
                return BadRequest(Response<EventTypeDto>.FailureResponse("Error retrieving event type", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Create new event type
        /// </summary>
        [HttpPost("eventtypes")]
        public async Task<ActionResult<Response<EventTypeDto>>> CreateEventType([FromBody] EventTypeDto eventTypeDto)
        {
            try
            {
                var eventType = new EventType
                {
                    Name = eventTypeDto.Name,
                    Description = eventTypeDto.Description,
                    Icon = eventTypeDto.Icon,
                    Color = eventTypeDto.Color,
                    Created = DateTime.UtcNow
                };

                _dbContext.EventTypes.Add(eventType);
                await _dbContext.SaveChangesAsync();

                var resultDto = new EventTypeDto
                {
                    Id = eventType.Id,
                    Name = eventType.Name,
                    Description = eventType.Description,
                    Icon = eventType.Icon,
                    Color = eventType.Color,
                    Created = eventType.Created
                };

                return CreatedAtAction(nameof(GetEventType), new { id = eventType.Id }, Response<EventTypeDto>.SuccessResponse(resultDto, "Event type created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event type");
                return BadRequest(Response<EventTypeDto>.FailureResponse("Error creating event type", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Update event type
        /// </summary>
        [HttpPut("eventtypes/{id:int}")]
        public async Task<ActionResult<Response<EventTypeDto>>> UpdateEventType(int id, [FromBody] EventTypeDto eventTypeDto)
        {
            try
            {
                var eventType = await _dbContext.EventTypes.FindAsync(id);
                if (eventType == null)
                    return NotFound(Response<EventTypeDto>.FailureResponse("Event type not found"));

                eventType.Name = eventTypeDto.Name;
                eventType.Description = eventTypeDto.Description;
                eventType.Icon = eventTypeDto.Icon;
                eventType.Color = eventTypeDto.Color;

                _dbContext.EventTypes.Update(eventType);
                await _dbContext.SaveChangesAsync();

                var resultDto = new EventTypeDto
                {
                    Id = eventType.Id,
                    Name = eventType.Name,
                    Description = eventType.Description,
                    Icon = eventType.Icon,
                    Color = eventType.Color,
                    Created = eventType.Created
                };

                return Ok(Response<EventTypeDto>.SuccessResponse(resultDto, "Event type updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event type {EventTypeId}", id);
                return BadRequest(Response<EventTypeDto>.FailureResponse("Error updating event type", new[] { ex.Message }));
            }
        }

        /// <summary>
        /// Delete event type
        /// </summary>
        [HttpDelete("eventtypes/{id:int}")]
        public async Task<ActionResult<Models.Response>> DeleteEventType(int id)
        {
            try
            {
                var eventType = await _dbContext.EventTypes.FindAsync(id);
                if (eventType == null)
                    return NotFound(Models.Response.FailureResponse("Event type not found"));

                _dbContext.EventTypes.Remove(eventType);
                await _dbContext.SaveChangesAsync();

                return Ok(Models.Response.SuccessResponse(null, "Event type deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event type {EventTypeId}", id);
                return BadRequest(Models.Response.FailureResponse("Error deleting event type", new[] { ex.Message }));
            }
        }

        #endregion
    }
}
