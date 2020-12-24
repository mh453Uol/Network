using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Network.Core
{
    public class Post
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(1025)]
        public string Content { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public Guid UpdatedByUserId { get; set; }

        [ForeignKey("UpdatedByUserId")]
        public User UpdatedByUser { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }


    }
}
