using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AppSec_Assignment.ViewModels
{
    public class Register
    {
        [Required]
        [DataType(DataType.Text)]
		[RegularExpression(@"^[a-zA-Z]{2,}$", ErrorMessage = "First Name must be at least 2 letters and contain only alphabets.")]
		public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
		[RegularExpression(@"^[a-zA-Z]{2,}$", ErrorMessage = "Last Name must be at least 2 letters and contain only alphabets.")]
		public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[ST]\d{7}[A-Za-z]$", ErrorMessage = "First character of NRIC must be an S or T, 9 characters in total and end with a letter.")]
        public string NRIC { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d!@#$%^&*()_+]{12,}$", ErrorMessage = "Passwords must be at least 12 characters long and contain at least an upper\r\ncase letter, lower case letter, digit and a symbol")]
        public string Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[CustomValidation(typeof(Register), "ValidateDateOfBirth")]
		public DateTime DateOfBirth { get; set; }

		public static ValidationResult ValidateDateOfBirth(DateTime? dateOfBirth, ValidationContext context)
		{
			if (dateOfBirth >= DateTime.Now.Date)
			{
				return new ValidationResult("Date of Birth must be set to a date before today.");
			}

			return ValidationResult.Success;
		}

		[Required]
		[DataType(DataType.Text)]
		public string WAI { get; set; }
	}
}