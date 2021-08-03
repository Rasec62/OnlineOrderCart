using Microsoft.AspNetCore.Mvc;

namespace OnlineOrderCart.Web.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
