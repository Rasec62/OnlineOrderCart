using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class RoleGroups
    {
        [Key]
        public int RoleGroupId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [ForeignKey("Users")]
        public long UserId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [ForeignKey("Roles")]
        public int RolId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        public Roles Roles { get; set; }
        public Users Users { get; set; }
    }
}
