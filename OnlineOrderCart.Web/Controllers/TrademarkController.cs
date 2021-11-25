using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
    public class TrademarkController : Controller
    {
        private readonly ITrademarkRolRepository _repository;
        private readonly IFlashMessage _flashMessage;
        private readonly IRepository<Trademarks> _trademarkrepository;

        public TrademarkController(ITrademarkRolRepository repository, IFlashMessage flashMessage, IRepository<Trademarks> Trademarkrepository)
        {
           _repository = repository;
           _flashMessage = flashMessage;
           _trademarkrepository = Trademarkrepository;
        }
        
        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            //var data = await _repository.GetAllRecordsAsync();
            var data = await _trademarkrepository.GetAllAsync();
            ViewBag.dataSource = data;
            return View(data);
        }
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            try
            {
                var trademark = await TrademarsExists(id.Value);

                if (trademark == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                trademark.IsDeleted = 1;
                //await _repository.UpdateAsync(trademark);
                await _trademarkrepository.UpdateAsync(trademark);
                _flashMessage.Confirmation("The Trademars was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Trademars can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<Trademarks> TrademarsExists(int id)
        {
            //var _area = await _repository
            //    .GetAll()
            //    .Where(s => s.TrademarkId == id)
            //    .FirstOrDefaultAsync();

            Trademarks trademark = await _trademarkrepository.GetAsync(id);

            return trademark;
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = await _trademarkrepository.GetAsync(id.Value);//await _repository.GetOnlyTrademarkAsync(id.Value);

            if (model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            return View(model);
        }
        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trademarks model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.IsDeleted = 0;
                    model.RegistrationDate = DateTime.Now.ToUniversalTime();
                    //await _repository.CreateAsync(model);
                    await _trademarkrepository.CreateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger($"The Trademark can't be created because it has related records.  {ex.Message}");
                }
            }

            return View(model);
        }

        [Authorize(Roles = "PowerfulUser,KamAdmin,KamAdCoordinator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _model = await _repository.GetAll().Where(s => s.TrademarkId == id).FirstOrDefaultAsync();
            if (_model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            return View(_model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trademarks model)
        {
            if (id != model.TrademarkId)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //await _repository.UpdateAsync(model);
                   await _trademarkrepository.UpdateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("The Trademarks can't be deleted because it has related records.  {0}", ex.Message);
                }
            }
            return View(model);
        }
    }
}
