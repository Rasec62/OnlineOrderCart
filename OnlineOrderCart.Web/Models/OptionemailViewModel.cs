using OnlineOrderCart.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class OptionemailViewModel
    {
        public Guid GuidId { get; set; }

        [Display(Name = "Razon Social")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string BusinessName { get; set; }

        [Display(Name = "Usuario")]
        [Range(1, double.MaxValue, ErrorMessage = "Usted debe de seleccionar un Usuario.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long UserId { get; set; }
        [Display(Name = "No Deudor")]
        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Debtor { get; set; }
        [Display(Name = "Correo electronico Opcional")]
        [MaxLength(70, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [EmailAddress]
        public string OptionalEmail { get; set; }

        [Display(Name = "Distribuidor")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public long DistributorId { get; set; }
        public List<TblOptionalEmail> DetailsOptionalEmail { get; set; }
    }
}
