using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderCart.Common.Entities
{
    public class Kams
    {
        [Key]
        public long KamId { get; set; }
        [Display(Name = "Employee Number")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [MaxLength(10, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string EmployeeNumber { get; set; }
        public string CodeKey { get; set; }
        [Display(Name = "Kam Manager")]
        public long? KamManagerId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime RegistrationDate { get; set; }
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Display(Name = "User")]
        [ForeignKey("Users")]
        public long UserId { get; set; }
        [JsonIgnore]
        public Users Users { get; set; }
        public ICollection<Distributors> GetDistributorCollection { get; set; }
    }
}
