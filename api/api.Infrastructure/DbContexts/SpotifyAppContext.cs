using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.DbContexts
{
    public class SpotifyAppContext : DbContext
    {
        public SpotifyAppContext (DbContextOptions<SpotifyAppContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<QueueItem> QueueItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*
             *  Members <--> Parties relationship
             */
            modelBuilder.Entity<User>()
                .HasOne(u => u.CurrentParty)
                .WithMany(p => p.Members)
                .HasForeignKey("CurrentPartyId");

            modelBuilder.Entity<User>()
                .HasOne(u => u.PendingParty)
                .WithMany(p => p.PendingMembers)
                .HasForeignKey("PendingPartyId");

            modelBuilder.Entity<User>()
                .HasOne(u => u.OwnedParty)
                .WithOne(p => p.Owner)
                .HasForeignKey(typeof(User).ToString(), "OwnedPartyId");

            /*
             * User membership computed columns
             */
            modelBuilder.Entity<User>()
                .Property(u => u.IsOwner)
                .HasComputedColumnSql("CAST(CASE WHEN OwnedPartyId IS NULL THEN 0 ELSE 1 END AS BIT)");

            modelBuilder.Entity<User>()
                .Property(u => u.IsMember)
                .HasComputedColumnSql("CAST(CASE WHEN CurrentPartyId IS NULL THEN 0 ELSE 1 END AS BIT)");

            modelBuilder.Entity<User>()
                .Property(u => u.IsPendingMember)
                .HasComputedColumnSql("CAST(CASE WHEN PendingPartyId IS NULL THEN 0 ELSE 1 END AS BIT)");

            /*
             * Party <--> Queue relationship
             */
            modelBuilder.Entity<QueueItem>()
                .HasOne(q => q.ForParty)
                .WithMany(p => p.QueueItems)
                .HasForeignKey("ForPartyId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QueueItem>()
                .HasOne(q => q.AddedByUser)
                .WithMany(u => u.QueueItems)
                .HasForeignKey("AddedByUserId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    
}
