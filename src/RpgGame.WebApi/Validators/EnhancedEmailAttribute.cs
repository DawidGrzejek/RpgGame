using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RpgGame.WebApi.Validators
{
    /// <summary>
    /// Enhanced email validation attribute that requires proper domain format (user@domain.tld)
    /// </summary>
    public class EnhancedEmailAttribute : ValidationAttribute
    {
        private static readonly Regex EnhancedEmailRegex = new(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true; // Let Required attribute handle empty values
            }

            var email = value.ToString()!.Trim();

            // Check basic format with domain TLD requirement
            if (!EnhancedEmailRegex.IsMatch(email))
            {
                return false;
            }

            // Additional checks
            if (email.Contains("..") || 
                email.StartsWith('.') || 
                email.EndsWith('.'))
            {
                return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"The {name} field must be a valid email address with a proper domain (e.g., user@example.com).";
        }
    }
}