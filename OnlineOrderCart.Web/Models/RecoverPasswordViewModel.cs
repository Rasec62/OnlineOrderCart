using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        //[EmailAddress]
        public string Email { get; set; }
    }
}
