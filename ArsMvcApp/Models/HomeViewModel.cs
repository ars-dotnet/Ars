using System.ComponentModel.DataAnnotations;

namespace MyIdentityWithGithub.Models
{
    public class HomeViewModel
    {
        [Required()]
        [EmailAddress()]
        [Display(Name = "Your Email")]
        public string Email { get; set; }
    }
}
