using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Network.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Data
{
    public class NetworkDbContext : IdentityDbContext<ApplicationUser, UserRole, Guid>
    {
        public DbSet<Post> Posts { get; set; }

        public NetworkDbContext(DbContextOptions<NetworkDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId);
        }
    }
}
