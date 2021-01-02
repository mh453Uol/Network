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
    public class PaginatedFollowingViewComponent : ViewComponent
    {
        private readonly NetworkDbContext _dbContext;
        private IQueryable<UserFollow> Query { get; set; }

        public PaginatedFollowingViewComponent(NetworkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public enum ViewComponentType
        {
            Following,
            Followers
        }

        public IQueryable<UserFollow> GetFollowing(Guid? userId)
        {
            return _dbContext.Follows.AsNoTracking()
                    .Where(f => f.FollowerId == userId && f.IsDeleted == false)
                    .OrderByDescending(p => p.CreatedOn)
                    .Include(f => f.Followee)
                    .Select(f => new UserFollow()
                    {
                        FolloweeId = f.FolloweeId,
                        Followee = new ApplicationUser
                        {
                            Firstname = f.Followee.Firstname,
                            Surname = f.Followee.Surname
                        }
                    });
        }


        public async Task<IViewComponentResult> InvokeAsync(int pageIndex, int pageSize, Guid userId, int? totalCount)
        {
            Query = GetFollowing(userId);
           
            var paginated = await PaginatedList<UserFollow>.CreateAsync(Query, pageIndex, pageSize, totalCount);

            return View(paginated);
        }
    }
}
