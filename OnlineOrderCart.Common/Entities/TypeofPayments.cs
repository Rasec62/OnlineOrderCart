using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Common.Entities
{
    public class TypeofPayments
    {
        [Key]
        public int TypeofPaymentId { get; set; }
        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string PaymentName { get; set; }
        [Display(Name = "CodeKey")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string CodeKey { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        public ICollection<IncentiveOrderDetails> GetIncentiveOrderDetails { get; set; }
    }
}
