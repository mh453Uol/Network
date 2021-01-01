using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Network.Core
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ICollection<Post> Posts { get; set; }
        public ICollection<UserFollow> Following { get; set; }
        public ICollection<UserFollow> Followers { get; set; }

        public ApplicationUser() : base()
        {
            Posts = new List<Post>();
            Followers = new List<UserFollow>();
            Following = new List<UserFollow>();
        }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Surname { get; set; }

        public string FullName {
            get
            {
                return $"{Firstname} {Surname}";
            }
        }
    }
}
