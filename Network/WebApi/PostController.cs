using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Core;
using Network.Data;
using Network.Util;
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
            var userId = User.GetUserId();

            var postExists = await _dbContext.Posts.AsNoTracking().AnyAsync(p => p.Id == postId);

            if (!postExists)
            {
                return NotFound();
            }

            var like = await HasLiked(postId, userId.Value);

            if (like == null)
            {
                await _dbContext.Likes.AddAsync(new Like()
                {
                    PostId = postId,
                    CreatedByUserId = userId.Value,
                    UpdatedByUserId = userId.Value,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                });
            }
            else
            {
                like.LikePost();
                _dbContext.Entry<Like>(like).Property(ee => ee.IsDeleted).IsModified = true;
            }

            await _dbContext.SaveChangesAsync();

            var likeCount = await GetLikeCountAsync(postId);

            return Ok(new { PostLikes = likeCount });
        }

        [Route("{postId:guid}/like")]
        [HttpDelete]
        public async Task<IActionResult> CancelLike(Guid postId)
        {
            var userId = User.GetUserId();

            var postExists = await _dbContext.Posts.AsNoTracking().AnyAsync(p => p.Id == postId);

            if (!postExists)
            {
                return NotFound();
            }

            var liked = await HasLiked(postId, userId.Value);

            if (liked != null)
            {
                liked.UnlikePost();

                _dbContext.Entry<Like>(liked).Property(ee => ee.IsDeleted).IsModified = true;

                await _dbContext.SaveChangesAsync();
            }

            var likeCount = await GetLikeCountAsync(postId);

            return Ok(new { PostLikes = likeCount });
        }


        public async Task<int> GetLikeCountAsync(Guid postId)
        {
            return await _dbContext.Likes
                    .AsNoTracking()
                    .Where(l => l.PostId == postId && l.IsDeleted == false)
                    .CountAsync();
        }

        public async Task<Like> HasLiked(Guid postId, Guid userId)
        {
            return await _dbContext.Likes
                   .Select(l => new Like
                   {
                       Id = l.Id,
                       PostId = l.PostId,
                       CreatedByUserId = l.CreatedByUserId
                   })
                   .FirstOrDefaultAsync(l => l.PostId == postId && l.CreatedByUserId == userId);
        }

    }
}
