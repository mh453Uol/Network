using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Network.Core;
using Network.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Network.Controllers
{
    public class InputModel
    {
        [Required]
        public string Content { get; set; }
    }

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
                var userId = Guid.Parse(_userManager.GetUserId(User));

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
