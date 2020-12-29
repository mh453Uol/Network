using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Core
{
    public class Like
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public Guid CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }

        public Guid UpdatedByUserId { get; set; }
        public ApplicationUser UpdatedByUser { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }

    }
}
