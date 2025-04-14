using System.ComponentModel.DataAnnotations;

namespace QuestCrafter.DTO
{
    public class LoginDTO : IValidatableObject
    {
        public string? username {get; set;}

        [EmailAddress] // witout this, any string will do the job!
        public string? email {get; set;}

        [Required]
        public required string password {get; set;}


        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
            {
                yield return new ValidationResult("Either Username or Email must be provided", new[] { nameof(username), nameof(email)});
            }
        }

    }
    /*
    otherway to force either username or email(requires : dotnet add package FluentValidation):
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
            
            RuleFor(x => x).Must(x => !string.IsNullOrEmpty(x.Username) || !string.IsNullOrEmpty(x.Email))
                .WithMessage("Either username or email must be provided")
                .OverridePropertyName("UsernameOrEmail");
                
            When(x => !string.IsNullOrEmpty(x.Email), () => 
            {
                RuleFor(x => x.Email).EmailAddress();
            });
        }
    }
    */
}


// we can also create a custom data annotation:
/*
using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [RequiredIfNull(nameof(Email), Display(Name = "Username")]
    public string? Username { get; set; }

    [RequiredIfNull(nameof(Username)), EmailAddress, Display(Name = "Email")]
    public string? Email { get; set; }

    [Required]
    public string Password { get; set; }
}

// Custom validation attribute
public class RequiredIfNullAttribute : ValidationAttribute
{
    private readonly string _otherProperty;
    
    public RequiredIfNullAttribute(string otherProperty)
    {
        _otherProperty = otherProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var otherPropertyInfo = context.ObjectType.GetProperty(_otherProperty);
        var otherValue = otherPropertyInfo?.GetValue(context.ObjectInstance);
        
        if (value == null && otherValue == null)
        {
            return new ValidationResult(
                $"Either {context.DisplayName} or {otherPropertyInfo?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() is DisplayAttribute display ? display.Name : _otherProperty} must be provided");
        }
        
        return ValidationResult.Success;
    }
}
*/