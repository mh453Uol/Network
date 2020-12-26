using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Network.Core;

namespace Network.Pages
{

    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileModel> _logger;

        public ApplicationUser ApplicationUser { get; set; }

        public ProfileModel(ILogger<ProfileModel> logger,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _logger = logger;
        }



        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser = await _userManager.GetUserAsync(User);

            return Page();
        }
    }
}
