using System.Collections.Generic;

namespace OnlineOrderCart.Web.Models
{
    public class DKCEmailDetails
    {
        public string EmailK { get; set; }
        public string EmailC { get; set; }
        public string EmailD { get; set; }
        public List<OptionalEmailModel> OptionalEmailDetails { get; set; }
    }
}
