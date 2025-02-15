﻿using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
    public class SimTypesController : Controller
    {
        private readonly ISimTypesRepository _repository;
        private readonly IFlashMessage _flashMessage;

        public SimTypesController(ISimTypesRepository repository,
            IFlashMessage flashMessage)
        {
           _repository = repository;
           _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _repository.GetAllRecordsAsync();
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
                var act = await SimTypesExists(id.Value);

                if (act == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                act.IsDeleted = 1;
                await _repository.UpdateAsync(act);
                _flashMessage.Confirmation("The Sim Types was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Sim Types can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<SimTypes> SimTypesExists(int id)
        {
            var _Sim = await _repository
                .GetOnlySimTypesAsync(id);
            return _Sim;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = await _repository.GetOnlySimTypesAsync(id.Value);

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
        public async Task<IActionResult> Create(SimTypes model)
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
                    _flashMessage.Danger($"The Sim Types can't be created because it has related records.  {ex.Message}");
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

            var _model = await _repository.GetAll().Where(s => s.SimTypeId == id).FirstOrDefaultAsync();
            if (_model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            return View(_model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SimTypes model)
        {
            if (id != model.SimTypeId)
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
                    _flashMessage.Danger("The Sim Types can't be deleted because it has related records.  {0}", ex.Message);
                }
            }
            return View(model);
        }
    }
}
