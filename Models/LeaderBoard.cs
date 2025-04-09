using System.ComponentModel.DataAnnotations;

namespace QuestCrafter.Models
{
    public class LeaderBoard
    {
        [Key]
        public int LeaderBoardId { get; set; }
        public int QuestId { get; set; } // Foreign key to Quest
        public required Quest Quest { get; set; } // Navigation to quest

        public int UserId { get; set; } // Foreign key to User
        public required User User { get; set; }  // Navigation to user

        public int Score { get; set; }
    }
}