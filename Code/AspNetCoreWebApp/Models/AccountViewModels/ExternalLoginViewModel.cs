using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebApp.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}