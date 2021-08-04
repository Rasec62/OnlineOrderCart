using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
    public class ProductsTypeController : Controller
    {
        private readonly IProductsTypeRespository _repository;
        private readonly IFlashMessage _flashMessage;

        public ProductsTypeController(IProductsTypeRespository repository
            ,  IFlashMessage flashMessage)
        {
            _repository = repository;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _repository.GetAllRecordsAsync();
            return View(data);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            try
            {
                var rol = await ProductsTypeExists(id.Value);

                if (rol == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                rol.IsDeleted = 1;
                await _repository.UpdateAsync(rol);
                _flashMessage.Confirmation("The Products Type was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Products Type can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<ProductsType> ProductsTypeExists(int id)
        {
            var _prod = await _repository
                .GetAll()
                .Where(s => s.ProductTypeId == id)
                .FirstOrDefaultAsync();
            return _prod;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsType model)
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
                    _flashMessage.Danger($"The Role can't be created because it has related records.  {ex.Message}");
                }
            }

            return View(model);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _model = await _repository.GetAll().Where(s => s.ProductTypeId == id).FirstOrDefaultAsync();
            if (_model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            return View(_model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductsType model)
        {
            if (id != model.ProductTypeId)
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
                    _flashMessage.Danger("The Products Type can't be deleted because it has related records.  {0}", ex.Message);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = await _repository.GetOnlyProductsTypeAsync(id.Value);

            if (model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            return View(model);
        }
    }
}
