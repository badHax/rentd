using System.ComponentModel.DataAnnotations;

namespace Rentd.API.Models
{
   public class ViewModelLogin
    {
        [Required]
        [StringLength(10, MinimumLength=4,
            ErrorMessage = "This field is required to be at least 4 characters"
        )]
        public string UserName { get; set; }
        [Required]
         [StringLength(30,MinimumLength=8,
            ErrorMessage="The password is required to be at least 8 characters"
        )]
        public string Password { get; set; }
    }
}