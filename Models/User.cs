using Microsoft.AspNetCore.Identity;

namespace QuestCrafter.Models
{
    public class User : IdentityUser<int>
    {
        public ICollection<Quest>? CreatedQuests { get; set; } // Quests this user created
        public ICollection<Participant>? Participations { get; set; } // Quests this user joined
        public ICollection<LeaderBoard>? LeaderBoardEntries { get; set; } // Leaderboard scores
    }
}