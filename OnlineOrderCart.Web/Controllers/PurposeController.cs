using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
    public class PurposeController : Controller
    {
        private readonly IDapper _dapper;
        private readonly IFlashMessage _flashMessage;
        private readonly DataContext _dataContext;

        public PurposeController(IDapper dapper, IFlashMessage flashMessage, DataContext dataContext)
        {
            _dapper = dapper;
            _flashMessage = flashMessage;
            _dataContext = dataContext;
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

        [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            try
            {
                var act = await PurposesExists(id.Value);

                if (act == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                act.IsDeleted = 1;
               _dataContext.Purposes.Update(act);
               await _dataContext.SaveChangesAsync();
                _flashMessage.Confirmation("The Purposes was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Purposes can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<Purposes> PurposesExists(int id)
        {
            var _area = await _dataContext.Purposes
                .Where(s => s.PurposeId == id)
                .FirstOrDefaultAsync();
            return _area;
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var model = await _dataContext.Purposes.FindAsync(id.Value);

            if (model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            return View(model);
        }
        [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Purposes model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.IsDeleted = 0;
                    model.RegistrationDate = DateTime.UtcNow;
                    _dataContext.Purposes.Add(model);
                    await _dataContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger($"The Purposes can't be created because it has related records.  {ex.Message}");
                }
            }

            return View(model);
        }
        [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _model = await _dataContext.Purposes
                .Where(s => s.PurposeId == id)
                .FirstOrDefaultAsync();
            if (_model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            return View(_model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Purposes model)
        {
            if (id != model.PurposeId)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dataContext.Purposes.Update(model);
                    await _dataContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("The Purposes can't be deleted because it has related records.  {0}", ex.Message);
                }
            }
            return View(model);
        }
    }
}
