using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderCart.Web.Models
{
    public class DeliverViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Delivery date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
    }
}
