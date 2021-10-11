using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Models
{
    public class GenerateaNormalOrderViewModel : AddItemViewModel
    {
        public long GenerateUserId { get; set; }
        //[Display(Name = "Employee Number")]
        //[Required(ErrorMessage = "The field {0} is mandatory.")]
        //[MaxLength(10, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        //public string EmployeeNumber { get; set; }

        //[Range(1, long.MaxValue, ErrorMessage = "You must select a Kam.")]
        //[Required(ErrorMessage = "The field {0} is required")]
        //[Display(Name = "Gerente")]
        //public long KamId { get; set; }

        public IEnumerable<SelectListItem> CombosKams { get; set; }
        public IEnumerable<SelectListItem> CombosDistributors { get; set; }
    }
}
