using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Network.Core;
using Network.Data;
using Network.Util;
using Network.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Network.ViewComponents
{
    public class PaginatedFollowersViewComponent : ViewComponent
    {
        private readonly NetworkDbContext _dbContext;

        public PaginatedFollowersViewComponent(NetworkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<UserFollow> GetFollowers(Guid? userId)
        {
            return _dbContext.Follows.AsNoTracking()
                    .Where(f => f.FolloweeId == userId && f.IsDeleted == false)
                    .OrderByDescending(p => p.CreatedOn)
                    .Include(f => f.Follower)
                    .Select(f => new UserFollow()
                    {
                        FollowerId = f.FollowerId,
                        Follower = new ApplicationUser
                        {
                            Firstname = f.Follower.Firstname,
                            Surname = f.Follower.Surname
                        }
                    });
        }

        public async Task<IViewComponentResult> InvokeAsync(int pageIndex, int pageSize, Guid userId, int? totalCount)
        {
            var query = GetFollowers(userId);
           
            var paginated = await PaginatedList<UserFollow>.CreateAsync(query, pageIndex, pageSize, totalCount);

            return View(paginated);
        }
    }
}
