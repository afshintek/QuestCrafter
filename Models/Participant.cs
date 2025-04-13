using System.ComponentModel.DataAnnotations;

namespace QuestCrafter.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }
        [Required]public required int UserId { get; set; } // Foreign key to User
        public User User { get; set; } = null!; // Navigation to user

        [Required]public required int QuestId { get; set; } // Foreign key to Quest
        public Quest Quest { get; set; } = null!; // Navigation to quest

        public bool IsCompleted { get; set; }

        public ICollection<string>? CompletionPictureUrl { get; set; } // New: URL to completion picture
        public DateTime? CompletionDate { get; set; } // Nullable since incomplete quests won’t have it
    }
}