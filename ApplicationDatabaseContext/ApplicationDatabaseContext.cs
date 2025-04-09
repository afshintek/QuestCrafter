using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuestCrafter.Models;

namespace ApplicationDatabaseContext
{
    public class AppDbCtx : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbCtx(DbContextOptions<AppDbCtx> options) : base (options) {}

        public DbSet<Quest> QuestsTable {get; set;}
        public DbSet<Participant> participantsTable {get; set;}
        public DbSet<LeaderBoard> LeaderTable {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Quest>()
            .HasOne(q => q.Creator)
            .WithMany(c => c.CreatedQuests)
            .HasForeignKey(q => q.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Quest>()
            .HasMany(q => q.Participants)
            .WithOne(x => x.Quest) // from participant model
            .HasForeignKey(x => x.QuestId) // from participant model
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Quest>()
            .HasMany(q => q.LeaderBoards)
            .WithOne(l => l.Quest)
            .HasForeignKey(l => l.QuestId)
            .OnDelete(DeleteBehavior.Cascade);  // Deletes leaderboard entries if the quest is deleted


            builder.Entity<Participant>()
            .HasIndex(p => new {p.UserId, p.QuestId})
            .IsUnique(); // Prevents a user from joining the same quest multiple times
            builder.Entity<Participant>()
            .HasOne(p => p.User)
            .WithMany(u => u.Participations)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of a user if they are participants is its restricted


            builder.Entity<LeaderBoard>()
            .HasOne(l => l.User)
            .WithMany(u => u.LeaderBoardEntries)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}


/*
.IsUnique():
    Makes the combination of UserId and QuestId unique in the Participant table. A user can’t join the same quest twice. For example:
    Allowed: { UserId = 1, QuestId = 1 } and { UserId = 1, QuestId = 2 } (same user, different quests).
    Not allowed: { UserId = 1, QuestId = 1 } twice (duplicate).
*/