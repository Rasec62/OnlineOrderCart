using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Controllers
{
    public class OrderIncentiveController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
