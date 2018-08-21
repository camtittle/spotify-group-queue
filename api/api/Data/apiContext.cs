using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Models
{
    public class apiContext : DbContext
    {
        public apiContext (DbContextOptions<apiContext> options)
            : base(options)
        {
        }

        public DbSet<api.Models.User> Users { get; set; }
        public DbSet<api.Models.Party> Parties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*
             *  Members <--> Parties relationship
             */
            modelBuilder.Entity("api.Models.User", b =>
            {
                b.HasOne("api.Models.Party", "CurrentParty")
                    .WithMany("Members")
                    .HasForeignKey("CurrentPartyId");
            });

            modelBuilder.Entity("api.Models.User", b =>
            {
                b.HasOne("api.Models.Party", "PendingParty")
                    .WithMany("PendingMembers")
                    .HasForeignKey("PendingPartyId");
            });

        }
    }

    
}
