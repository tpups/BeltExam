using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BeltExam.Models
{
        public class Register
    {
        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must consist only of letters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must consist only of letters")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email address must be valid")]
        [UniqueEmail]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter a password")]
        [MinLength(8, ErrorMessage="Password must be at least 8 characters")]
        [RegularExpression(@"^(?=\D*\d)(?=.*?[a-zA-Z]).*[\W_].*$", ErrorMessage="Password must contain at least one letter, one number, and one special character")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPW { get; set; }
    }
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext ValidationContext)
        {
            var _context = (BeltContext) ValidationContext.GetService(typeof(BeltContext));
            var EmailCheck = _context.Users.SingleOrDefault(user => user.Email == (string)value);
            if(EmailCheck == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Email is already registered");
            }
        }
    }
}