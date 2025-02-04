using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NetworkDbContext _dbContext;
        private readonly IFlashMessage _flashMessage;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<EditModel> _logger;

        public Post Post { get; set; }

        public EditModel(SignInManager<ApplicationUser> signInManager,
            ILogger<EditModel> logger,
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
            var userId = User.GetUserId();

            Post = await _dbContext.Posts.AsNoTracking()
            .Select(p => new Post()
            {
                Id = p.Id,
                Content = p.Content,
                UpdatedOn = p.UpdatedOn,
                CreatedById = p.CreatedById,
                IsDeleted = p.IsDeleted
            })
            .FirstOrDefaultAsync(p => p.Id == id && p.CreatedById == userId && p.IsDeleted == false);

            if (Post == null)
            {
                _flashMessage.Warning($"Could not find the post with id {id}");
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id, [Bind("Content")] Post model)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(_userManager.GetUserId(User));

                var exists = await _dbContext.Posts.AsNoTracking().AnyAsync(p => p.Id == id && p.CreatedById == userId && p.IsDeleted == false);

                if(!exists)
                {
                    _flashMessage.Warning($"Could not edit the post with id {id}");
                    return RedirectToPage("/Index");
                }

                var post = _dbContext.Posts.Attach(new Post()
                {
                    Id = id,
                    Content = model.Content,
                    UpdatedById = userId,
                    UpdatedOn = DateTime.Now
                });

                _dbContext.Entry<Post>(post.Entity).Property(ee => ee.Content).IsModified = true;
                _dbContext.Entry<Post>(post.Entity).Property(ee => ee.UpdatedById).IsModified = true;
                _dbContext.Entry<Post>(post.Entity).Property(ee => ee.UpdatedOn).IsModified = true;
                
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage("/Index");
        }
    }
}

