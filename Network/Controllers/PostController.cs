using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Core;
using Network.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Network.Controllers
{

    // We need to have an MVC controller since when we display the new post form its a partial view. When making a HTTP POST to the razor page we need to have come from 
    // a razor page. https://stackoverflow.com/questions/49868791/post-partial-view-form 

    [Authorize]
    public class PostController : Controller
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content")] Post model)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _dbContext.Posts.AddAsync(new Post()
                {
                    Content = model.Content,
                    UserId = userId,
                    UpdatedByUserId = userId,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                });

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage("/Index");
        }
    }
}
