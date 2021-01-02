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

        public PaginatedFollowingViewComponent(NetworkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<UserFollow> GetFollowing(Guid? userId)
        {
            return _dbContext.Follows.AsNoTracking()
                    .Where(f => f.FollowerId == userId && f.IsDeleted == false)
                    .OrderByDescending(p => p.CreatedOn)
                    .Include(f => f.Followee)
                    .Select(f => new UserFollow()
                    {
                        Followee = new ApplicationUser
                        {
                            Id = f.Followee.Id,
                            Firstname = f.Followee.Firstname,
                            Surname = f.Followee.Surname
                        }
                    });
        }


        public async Task<IViewComponentResult> InvokeAsync(int pageIndex, int pageSize, Guid userId, int? totalCount, string tab = null)
        {
            var query = GetFollowing(userId);
           
            var paginated = await PaginatedList<UserFollow>.CreateAsync(query, pageIndex, pageSize, totalCount);

            paginated.AdditionalQueryStrings = new Dictionary<string, string>() { { "tab", tab } };

            return View(paginated);
        }
    }
}
