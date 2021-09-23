using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Common.Entities
{
    public class TblOptionalEmail
    {
        [Key]
        public Guid Id { get; set; }
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
        public int IsDeleted { get; set; }
        [Display(Name = "Fecha de Registro")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime RegistrationDate { get; set; }
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime DateLocal => RegistrationDate.ToLocalTime();
    }
}
