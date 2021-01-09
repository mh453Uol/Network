using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Core;
using Network.Data;
using Network.Util;
using Vereyon.Web;

namespace Network.Pages.Posts
{
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NetworkDbContext _dbContext;
        private readonly IFlashMessage _flashMessage;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<DeleteModel> _logger;

        public Post Post { get; set; }

        public DeleteModel(SignInManager<ApplicationUser> signInManager,
            ILogger<DeleteModel> logger,
            UserManager<ApplicationUser> userManager,
            NetworkDbContext dbContext,
            IFlashMessage flashMessage)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _flashMessage = flashMessage;
            _signInManager = signInManager;
            _logger = logger;

        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var userId = User.GetUserId().Value;

            Post = await _dbContext.Posts.AsNoTracking()
                    .Where(p => p.CreatedById == userId && p.Id == id && p.IsDeleted == false)
                    .Include(p => p.CreatedBy)
                    .Include(p => p.Likes)
                    .Select(p => new Post
                    {
                        Id = p.Id,
                        Content = p.Content,
                        UpdatedOn = p.UpdatedOn,
                        CreatedById = p.CreatedById,
                        CreatedBy = new ApplicationUser
                        {
                            Id = p.CreatedBy.Id,
                            Firstname = p.CreatedBy.Firstname,
                            Surname = p.CreatedBy.Surname
                        },
                        // Have to do .ToList().ToHashSet() rather than ToHashSet() due to this issue - https://github.com/dotnet/efcore/issues/20101
                        LikeSet = p.Likes.Where(l => l.IsDeleted == false).Select(l => l.CreatedByUserId).ToList().ToHashSet()
                    }).FirstAsync();


            if (Post == null)
            {
                _flashMessage.Warning($"Could not find the post with id {id}");
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var userId = User.GetUserId().Value;

            var exists = await _dbContext.Posts.AsNoTracking().AnyAsync(p => p.CreatedById == userId && p.Id == id && p.IsDeleted == false);

            if (!exists)
            {
                _flashMessage.Warning($"Could not find the post with id {id}");
                return RedirectToPage("/Index");
            }

            var post = _dbContext.Posts.Attach(new Post() { Id = id });

            post.Entity.IsDeleted = true;

            _dbContext.Entry<Post>(post.Entity).Property(ee => ee.IsDeleted).IsModified = true;

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Profile", new { id = userId, Tab = "posts" });
        }

    }
}
