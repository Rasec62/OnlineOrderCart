using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Helpers;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
    public class TypeofPaymentsController : Controller
    {
        private readonly DataContext _context;
        private readonly ITypeofPaymentRepository _typeofPaymentRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;

        public TypeofPaymentsController(DataContext context
            , ITypeofPaymentRepository typeofPaymentRepository, IConverterHelper converterHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _typeofPaymentRepository = typeofPaymentRepository;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
        }

        // GET: TypeofPayments
        public async Task<IActionResult> Index()
        {
            return View(await _typeofPaymentRepository.GetAllTypeofPaymentsAsync());
        }

        // GET: TypeofPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeofPayments = await _typeofPaymentRepository.GetOnlyTypeofPaymentAsync(id.Value);
            if (typeofPayments == null)
            {
                return NotFound();
            }

            return View(typeofPayments);
        }

        // GET: TypeofPayments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeofPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TypeofPayments typeofPayments)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var model = _converterHelper.ToTypeofPaymentsEntity(typeofPayments,true);
                   await _typeofPaymentRepository.CreateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe esta Tipo de pago .");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(typeofPayments);
        }

        // GET: TypeofPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var typeofPayments = await _typeofPaymentRepository.GetOnlyTypeofPaymentAsync(id.Value);
            if (typeofPayments == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            return View(typeofPayments);
        }

        // POST: TypeofPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TypeofPayments typeofPayments)
        {
            if (id != typeofPayments.TypeofPaymentId)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var _dupli = await _typeofPaymentRepository.GetOnlyTypeofPaymentAsync(typeofPayments.TypeofPaymentId);
                    _dupli.PaymentName = typeofPayments.PaymentName.ToUpper() ?? _dupli.PaymentName.ToUpper();
                    _dupli.CodeKey = typeofPayments.CodeKey.ToUpper() ?? _dupli.CodeKey.ToUpper();
                    await _typeofPaymentRepository.UpdateAsync(_dupli);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe esta Tipo de pago .");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(typeofPayments);
        }

        // GET: TypeofPayments/Delete/5

        //public async Task<IActionResult> Delete(int? id)
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            try
            {
                var typeofPayments = await _typeofPaymentRepository.GetOnlyTypeofPaymentAsync(id.Value);
                if (typeofPayments == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }
                typeofPayments.IsDeleted = 1;
                await _typeofPaymentRepository.UpdateAsync(typeofPayments);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Type of Payments can't be deleted because it has related records. {ex.Message}");
            }

            //return RedirectToAction(nameof(Index));
            return View();
        }
        [HttpPost]
        public JsonResult DeleteDetails(int? id)
        {

            if (id == null)
            {
                return Json(new { status = "Error" });
            }
            try
            {
                var typeofPayments = _context.TypeofPayments.FirstOrDefault(x => x.TypeofPaymentId==id.Value);
                if (typeofPayments == null)
                {
                    return Json(new { status = "Error" });
                }
                typeofPayments.IsDeleted = 1;
               _context.TypeofPayments.Update(typeofPayments);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Type of Payments can't be deleted because it has related records. {ex.Message}");
            }
            return Json(new { status = "Success" });

        }

        private bool TypeofPaymentsExists(int id)
        {
            return _context.TypeofPayments.Any(e => e.TypeofPaymentId == id);
        }
    }
}
