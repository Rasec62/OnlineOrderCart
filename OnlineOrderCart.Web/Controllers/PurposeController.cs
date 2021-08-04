using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
    public class PurposeController : Controller
    {
        private readonly IDapper _dapper;
        private readonly IFlashMessage _flashMessage;

        public PurposeController(IDapper dapper, IFlashMessage flashMessage)
        {
            _dapper = dapper;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {

            if (!User.Identity.IsAuthenticated || User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var DParameters = new DynamicParameters();

            var data = await Task.FromResult(_dapper.GetAll<Purposes>("sp_GetPurpose"
               , DParameters,
               commandType: CommandType.StoredProcedure));

            //var data = _dapper.GetAll<Purposes>("sp_GetPurpose", DParameters, commandType: CommandType.StoredProcedure).ToList();
            ViewBag.dataSource = data;
            return View(data);
        }
    }
}
