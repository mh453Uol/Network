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

        [HttpPost("{followeeId:guid}")]
        public async Task<IActionResult> Follow(Guid followeeId)
        {
            var userId = User.GetUserId();

            var followeeExists = await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Id == followeeId);

            if (!followeeExists)
            {
                return NotFound(new { Message = $"Could not find user with id {followeeId}" });
            }

            var hasAlreadyFollowedUser = await _dbContext.Follows.AsNoTracking().AnyAsync(f => f.FolloweeId == followeeId && f.FollowerId == userId);

            if (hasAlreadyFollowedUser)
            {
                var following = _dbContext.Follows.Attach(new UserFollow(followeeId, userId.Value));

                following.Entity.Follow();

                _dbContext.Entry<UserFollow>(following.Entity).Property(ee => ee.IsDeleted).IsModified = true;
            }
            else
            {
                await _dbContext.Follows.AddAsync(new UserFollow(followeeId, userId.Value)
                {
                    CreatedById = userId.Value,
                    UpdatedById = userId.Value,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                });
            }

            await _dbContext.SaveChangesAsync();

            var followeeFollowersCount = await _dbContext.Follows.Where(f => f.FolloweeId == followeeId && f.IsDeleted == false).CountAsync();

            return Ok(new { FollowersCount = followeeFollowersCount });
        }

        [HttpDelete("{followeeId:guid}")]
        public async Task<IActionResult> Unfollow(Guid followeeId)
        {
            var userId = User.GetUserId();

            var following = await _dbContext.Follows.AsNoTracking().SingleOrDefaultAsync(f => f.FolloweeId == followeeId && f.FollowerId == userId);

            if (following != null)
            {
                _dbContext.Follows.Attach(following);

                following.Unfollow();

                _dbContext.Entry<UserFollow>(following).Property(ee => ee.IsDeleted).IsModified = true;

                await _dbContext.SaveChangesAsync();
            }

            var followeeFollowersCount = await _dbContext.Follows.Where(f => f.FolloweeId == followeeId && f.IsDeleted == false).CountAsync();

            return Ok(new { FollowersCount = followeeFollowersCount });
        }
    }
}
