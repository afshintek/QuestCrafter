using Microsoft.AspNetCore.Identity;

public class CustomIdntityRules
{
    public static void CustomIdentityConfig(IdentityOptions options)
    {
        options.User.RequireUniqueEmail = true; // this also checks email validation(havig correct format!)
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.";
    }
}