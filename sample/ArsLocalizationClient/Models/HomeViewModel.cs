using System.ComponentModel.DataAnnotations;

namespace MyIdentityWithGithub.Models
{
    public class HomeViewModel
    {
        [Required()]
        [EmailAddress(ErrorMessage = null)]
        [Display(Name = "Your Email")]
        public string Email { get; set; }
    }
}
