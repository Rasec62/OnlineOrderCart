using Microsoft.AspNetCore.Mvc;
using OnlineOrderCart.Web.Errors;

namespace OnlineOrderCart.Web.Controllers.API
{
    [Route("errors")]
    public class ErrorController : BaseApiController
    {
        public IActionResult Error(int code)
        {
            return new ObjectResult(new CodeErrorResponse(code));
        }
    }
}
