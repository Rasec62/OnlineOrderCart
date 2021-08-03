using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Common.Entities
{
    public class UserActivations
    {
        [Key]
        public int UserActivationsId { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long UserId { get; set; }
        public string UserName { get; set; }
        public Guid ActivationCode { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string EventAction { get; set; }
        public string JwtId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
