﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Core;
using Network.Data;
using Network.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Network.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NetworkDbContext _dbContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        public PaginatedList<Post> Posts { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; }

        public int PageSize { get; } = 10;

        public IndexModel(SignInManager<ApplicationUser> signInManager,
            ILogger<IndexModel> logger,
            UserManager<ApplicationUser> userManager,
            NetworkDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _logger = logger;

            PageIndex = 1;
            Posts = new PaginatedList<Post>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var posts = _dbContext.Posts.AsNoTracking()
                .OrderByDescending(p => p.CreatedOn)
                .Include(p => p.UpdatedByUser)
                .Include(p => p.Likes)
                .Select(p => new Post
                {
                    Id = p.Id,
                    Content = p.Content,
                    UpdatedOn = p.UpdatedOn,
                    UserId = p.UserId,
                    User = new ApplicationUser
                    {
                        Id = p.User.Id,
                        Firstname = p.User.Firstname,
                        Surname = p.User.Surname
                    },
                    // Have to do .ToList().ToHashSet() rather than ToHashSet() due to this issue - https://github.com/dotnet/efcore/issues/20101
                    LikeSet = p.Likes.Where(l => l.IsDeleted == false).Select(l => l.CreatedByUserId).ToList().ToHashSet()
                });

            PageIndex = Math.Max(1, PageIndex);

            Posts = await PaginatedList<Post>.CreateAsync(posts, PageIndex, PageSize);

            return Page();
        }
    }
}
