using System.ComponentModel.DataAnnotations;

namespace QuestCrafter.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }
        public int UserId { get; set; } // Foreign key to User
        public required User User { get; set; } // Navigation to user

        public int QuestId { get; set; } // Foreign key to Quest
        public required Quest Quest { get; set; } // Navigation to quest

        public bool IsCompleted { get; set; }

        public ICollection<string>? CompletionPictureUrl { get; set; } // New: URL to completion picture
        public DateTime? CompletionDate { get; set; } // Nullable since incomplete quests won’t have it
    }
}