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
            var userId = User.GetUserId();

            Post = await _dbContext.Posts.AsNoTracking()
            .Select(p => new Post()
            {
                Id = p.Id,
                Content = p.Content,
                UpdatedOn = p.UpdatedOn,
                CreatedById = p.CreatedById,
            })
            .FirstOrDefaultAsync(p => p.Id == id && p.CreatedById == userId && p.IsDeleted == false);

            if (Post == null)
            {
                _flashMessage.Warning($"Could not find the post with id {id}");
                return RedirectToPage("/Index");
            }

            return Page();
        }

    }
}
