using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Core
{
    public class UserFollow : IAudit
    {
        public UserFollow(Guid followeeId, Guid followerId)
        {
            FolloweeId = followeeId;
            FollowerId = followerId;
        }

        public UserFollow()
        {

        }
        public Guid FolloweeId { get; set; }
        public ApplicationUser Followee { get; set; }

        public Guid FollowerId { get; set; }
        public ApplicationUser Follower { get; set; }

        public ApplicationUser CreatedBy { get; set; }
        public Guid CreatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        public void Unfollow()
        {
            IsDeleted = true;
        }

        public void Follow()
        {
            IsDeleted = false;
        }
    }
}
