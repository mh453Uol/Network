using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Network.Core
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Surname { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ApplicationUser() : base()
        {

        }

        public string FullName {
            get
            {
                return $"{Firstname} {Surname}";
            }
        }
    }
}
