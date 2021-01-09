using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Network.Core
{
    public class Post: IAudit
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(1025)]
        public string Content { get; set; }

        public ICollection<Like> Likes { get; set; }

        [NotMapped]
        public HashSet<Guid> LikeSet { get; set; }

        public ApplicationUser CreatedBy { get; set; }
        public Guid CreatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
