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

namespace Network.Pages
{

    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NetworkDbContext _dbContext;
        private readonly ILogger<ProfileModel> _logger;
        public Guid? UserId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; }
        public int PageSize { get; } = 5;

        [BindProperty(SupportsGet = true)]
        public string Tab { get; set; }

        public bool IsFollowUserButtonVisible { get; set; }
        public bool IsFollowingUser { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }

        public ApplicationUser ProfileUser { get; set; }

        public ProfileModel(ILogger<ProfileModel> logger,
            UserManager<ApplicationUser> userManager,
            NetworkDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _logger = logger;

            PageIndex = 1;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            UserId = User.GetUserId();

            IsFollowUserButtonVisible = User.Identity.IsAuthenticated && UserId.Value != id;

            ProfileUser = await _dbContext.Users
                .Select(u => new ApplicationUser
                {
                    Id = u.Id,
                    Firstname = u.Firstname,
                    Surname = u.Surname,
                })
                .SingleOrDefaultAsync(u => u.Id == id);

            FollowerCount = await _dbContext.Follows.Where(f => f.FolloweeId == id && f.IsDeleted == false).CountAsync();
            FollowingCount = await _dbContext.Follows.Where(f => f.FollowerId == id && f.IsDeleted == false).CountAsync();

            if (IsFollowUserButtonVisible && FollowerCount > 0)
            {
                IsFollowingUser = await _dbContext.Follows
                        .AnyAsync(f => f.FolloweeId == id && f.FollowerId == UserId.Value);
            }

            PageIndex = Math.Max(1, PageIndex);

            return Page();
        }

        public bool IsTabVisible(string tab)
        {
            return Tab == tab;
        }
    }
}
