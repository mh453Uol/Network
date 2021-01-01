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
    public class ProfileViewModel : ApplicationUser
    {
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
    }

    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NetworkDbContext _dbContext;
        private readonly ILogger<ProfileModel> _logger;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; }
        public int PageSize { get; } = 5;

        public bool IsFollowUserButtonVisible { get; set; }

        public bool IsFollowingUser { get; set; }

        public ProfileViewModel ProfileUser { get; set; }

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
            var userId = User.GetUserId();

            IsFollowUserButtonVisible = User.Identity.IsAuthenticated && userId != id;

            ProfileUser = await _dbContext.Users
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .Select(u => new ProfileViewModel
                {
                    Id = u.Id,
                    Firstname = u.Firstname,
                    Surname = u.Surname,
                    FollowersCount = u.Followers.Count(),
                    FollowingCount = u.Following.Count()
                })
                .SingleOrDefaultAsync(u => u.Id == id);

            if (IsFollowUserButtonVisible && ProfileUser.FollowersCount > 0)
            {
                IsFollowingUser = await _dbContext.Follows
                        .AnyAsync(f => f.FolloweeId == id && f.FollowerId == userId);
            }

            PageIndex = Math.Max(1, PageIndex);

            return Page();
        }
    }
}
