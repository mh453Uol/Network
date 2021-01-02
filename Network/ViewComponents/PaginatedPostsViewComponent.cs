using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Network.Core;
using Network.Data;
using Network.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Network.ViewComponents
{
    public class PaginatedPostsViewComponent : ViewComponent
    {
        private readonly NetworkDbContext _dbContext;

        public PaginatedPostsViewComponent(NetworkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Post> GetAllPosts(Guid? userId)
        {

            var predicate = PredicateBuilder.New<Post>(true);

            if (userId.HasValue)
            {
                predicate.And(p => p.UserId == userId);
            }

            return _dbContext.Posts.AsNoTracking()
                    .Where(predicate)
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

        }


        public async Task<IViewComponentResult> InvokeAsync(int pageIndex, int pageSize, Guid? userId, string tab = null)
        {
            var query = GetAllPosts(userId);

            var paginated = await PaginatedList<Post>.CreateAsync(query, pageIndex, pageSize);

            paginated.AdditionalQueryStrings = new Dictionary<string, string>() { { "tab", tab } };

            return View(paginated);
        }
    }
}
