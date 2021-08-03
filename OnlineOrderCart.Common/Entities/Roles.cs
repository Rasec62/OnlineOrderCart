using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Common.Entities
{
    public class Roles
    {
        [Key]
        public int RolId { get; set; }
        [Display(Name = "RolName")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string RolName { get; set; }
        [Display(Name = "CodeKey")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string CodeKey { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        public ICollection<RoleGroups> GetRoleGroupsCollection { get; set; }
    }
}
