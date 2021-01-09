using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Core;
using Network.Data;
using Network.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Network.Controllers
{

    // We need to have an MVC controller since when we display the new post form its a partial view. When making a HTTP POST to the razor page (User wants to create a new post)
    // we need to have come from a razor page so we cant use razor pages for the post action. However a MVC controller can handle this situation. 
    // https://stackoverflow.com/questions/49868791/post-partial-view-form 

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

        // GET: Post/Create is required since when the user is unauthenticated and tries to add a post, the[authorize] redirects user to
        // https://localhost:5001/Identity/Account/Login?ReturnUrl=%2FPost%2FCreate
        // When the user has logged in, we then redirect to /Post/Create.Since the post is a partial form we redirect to the Index page
        [HttpGet]
        [Route("Post/Create")]
        public IActionResult Get()
        {
            return RedirectToPage("/Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content")] Post model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.GetUserId().Value;

                await _dbContext.Posts.AddAsync(new Post()
                {
                    Content = model.Content,
                    CreatedById = userId,
                    UpdatedById = userId,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                });

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage("/Index");
        }
    }
}
