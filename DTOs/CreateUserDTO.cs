using System.ComponentModel.DataAnnotations;

namespace QuestCrafter.DTO
{
    public class CreateUserDTO
    {
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "username must be between 3 and 20 characters")]
        public required string username {get; set;}
        
        [Required]
        // [EmailAddress] no need for this since we're using Customrules.cs Ln 7, Col 9
        public required string email {get; set;}

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public required string Password {get; set;}

    }
}