using System.ComponentModel.DataAnnotations;

namespace QuestCrafter.Models
{
    public class LeaderBoard
    {
        [Key]
        public int LeaderBoardId { get; set; }
        [Required]public int QuestId { get; set; } // Foreign key to Quest
        public Quest? Quest { get; set; } = null!; // Navigation to quest

        [Required]public int UserId { get; set; } // Foreign key to User
        public User User { get; set; } = null!;  // Navigation to user

        public int RaterId { get; set; } //Who gave the score
        public int Score { get; set; }
    }
}