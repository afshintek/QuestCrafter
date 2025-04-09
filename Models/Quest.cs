using System.ComponentModel.DataAnnotations;

namespace QuestCrafter.Models
{
    public class Quest
    {
        [Key]
        public int QuestId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ICollection<string>? PictureUrl {get; set;}
        public DateTime CreatedAt { get; set; }
        public DateTime Deadline { get; set; }

        public ICollection<Participant>? Participants { get; set; } // Users in this quest
        public ICollection<LeaderBoard>? LeaderBoards { get; set; } // Leaderboard for this quest

        public int CreatorId { get; set; } // Foreign key to User
        public User? Creator { get; set; } // Navigation to creator
    }
}