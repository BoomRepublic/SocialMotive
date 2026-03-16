using System.Reflection;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMotive.Core.Authorization;
using SocialMotive.Core.Model.Admin;
using SocialMotive.Core.Data;

namespace SocialMotive.Core.Controllers.Admin;

/// <summary>
/// Admin Controller for managing all whitelisted database tables.
/// Endpoints: GET, POST (Create), PUT (Update), DELETE
/// All operations require Admin role.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = AppRoles.Admin)]
public partial class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly SocialMotiveDbContext _dbContext;
    private readonly IMapper _mapper;

    public AdminController(
        ILogger<AdminController> logger,
        SocialMotiveDbContext dbContext,
        IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }
}
