using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Core;
using Network.Data;
using Network.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Network.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FollowController : ControllerBase
    {
        private readonly NetworkDbContext _dbContext;
        private readonly ILogger<FollowController> _logger;

        public FollowController(ILogger<FollowController> logger,
            NetworkDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [Route("{followeeId:guid}")]
        [HttpPost]
        public async Task<IActionResult> Follow(Guid followeeId)
        {
            var userId = User.GetUserId();

            var followeeExists = await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == followeeId);

            if (!followeeExists)
            {
                return NotFound(new { Message = $"Could not find user with id {followeeId}" });
            }

            var hasAlreadyFollowedUser = await _dbContext.Follows.AsNoTracking().AnyAsync(f => f.FolloweeId == followeeId && f.FollowerId == userId);

            if (!hasAlreadyFollowedUser)
            {
                await _dbContext.Follows.AddAsync(new UserFollow(followeeId, userId.Value)
                {
                    CreatedById = userId.Value,
                    UpdatedById = userId.Value,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                });

                await _dbContext.SaveChangesAsync();
            }

            var followeeFollowersCount = await _dbContext.Follows.Where(f => f.FolloweeId == followeeId).CountAsync();

            return Ok(new { FollowersCount = followeeFollowersCount });
        }

    }
}
