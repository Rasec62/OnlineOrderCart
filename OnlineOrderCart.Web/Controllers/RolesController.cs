using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
    public class RolesController : Controller
    {
        private readonly IRolRepository _repository;
        private readonly IFlashMessage _flashMessage;

        public RolesController(IRolRepository repository, IFlashMessage flashMessage)
        {
            _repository = repository;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {

            if (!User.Identity.IsAuthenticated || User.Identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var data = await _repository.GetRolesAsync();
            ViewBag.dataSource = data;
            return View(data);
        }
        //public IActionResult UrlDatasource([FromBody] DataManagerRequest dm)
        //{
        //    IEnumerable DataSource = _repository.GetAll();//_repository.GetAllRecords();
        //    DataOperations operation = new DataOperations();
        //    if (dm.Search != null && dm.Search.Count > 0)
        //    {
        //        DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
        //    }
        //    if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
        //    {
        //        DataSource = operation.PerformSorting(DataSource, dm.Sorted);
        //    }
        //    if (dm.Where != null && dm.Where.Count > 0) //Filtering
        //    {
        //        DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
        //    }
        //    int count = DataSource.Cast<Roles>().Count();
        //    if (dm.Skip != 0)
        //    {
        //        DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
        //    }
        //    if (dm.Take != 0)
        //    {
        //        DataSource = operation.PerformTake(DataSource, dm.Take);
        //    }
        //    return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        //}
       

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = await _repository.GetOnlyRolAsync(id.Value);

            if (model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Roles model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.IsDeleted = 0;
                    model.RegistrationDate = DateTime.Now.ToUniversalTime();
                    await _repository.CreateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("The Role can't be created because it has related records.  {0}", ex.Message);
                }
            }
           
            return View(model);
        }
        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _model = await _repository.GetAll().Where(s => s.RolId == id).FirstOrDefaultAsync();
            if (_model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            return View(_model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Roles model)
        {
            if (id != model.RolId)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("The Areas can't be deleted because it has related records.  {0}", ex.Message);
                }
            }
            return View(model);
        }
        // GET: Groups/Delete/5

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            try
            {
                var rol = await RolesExists(id.Value);

                if (rol == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                rol.IsDeleted = 1;
                await _repository.UpdateAsync(rol);
                _flashMessage.Confirmation("The Rol was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The category can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<Roles> RolesExists(int id)
        {
            var _area = await _repository
                .GetAll()
                .Where(s => s.RolId == id)
                .FirstOrDefaultAsync();
            return _area;
        }
    }
}
