using System.ComponentModel.DataAnnotations;

namespace Blog.Models.ViewModels.AccountViewModels
{
    public class RegisterVM
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = default!;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords are different!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; } = default!;
    }
}
