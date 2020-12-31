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

namespace Network.Pages
{

    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NetworkDbContext _dbContext;
        private readonly ILogger<ProfileModel> _logger;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; }
        public int PageSize { get; } = 5;

        public ApplicationUser ProfileUser { get; set; }
        public bool UserLoggedIn { get; set; }

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
            PageIndex = Math.Max(1, PageIndex);

            ProfileUser = await _dbContext.Users.Select(u => new ApplicationUser
            {
                Id = u.Id,
                Firstname = u.Firstname,
                Surname = u.Surname
            })
            .FirstOrDefaultAsync(u => u.Id == id);

            return Page();
        }
    }
}
