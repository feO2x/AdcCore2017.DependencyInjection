using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebApp.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}