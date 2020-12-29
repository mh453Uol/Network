using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Core;
using Network.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Network.WebApi
{
    [Route("api/[controller]", Name = "PostController")]
    [Authorize]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NetworkDbContext _dbContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<PostController> _logger;

        public PostController(SignInManager<ApplicationUser> signInManager,
            ILogger<PostController> logger,
            UserManager<ApplicationUser> userManager,
            NetworkDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _logger = logger;
        }

        [Route("{postId:guid}/like")]
        [HttpPost]
        public async Task<IActionResult> Like(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var postExists = await _dbContext.Posts.AnyAsync(p => p.Id == postId);

            if (!postExists)
            {
                return NotFound();
            }

            var liked = await _dbContext.Likes
                    .Select(l => new Like { Id = l.Id, CreatedByUserId = l.CreatedByUserId })
                    .FirstOrDefaultAsync(l => l.CreatedByUserId == userId);

            if (liked == null)
            {
                await _dbContext.Likes.AddAsync(new Like()
                {
                    PostId = postId,
                    CreatedByUserId = userId,
                    UpdatedByUserId = userId,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                });
            }
            else
            {
                liked.IsDeleted = !liked.IsDeleted;

                var alreadyLiked = _dbContext.Likes.Attach(liked);

                _dbContext.Entry<Like>(alreadyLiked.Entity).Property(ee => ee.IsDeleted).IsModified = true;
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
