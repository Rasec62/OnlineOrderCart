using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Common.Entities
{
    public class Purposes
    {
        [Key]
        public int PurposeId { get; set; }
        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Description { get; set; }
        [Display(Name = "CodeKey")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string CodeKey { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        public ICollection<DeatilWarehouses> GetPuDWarehouses { get; set; }
    }
}
