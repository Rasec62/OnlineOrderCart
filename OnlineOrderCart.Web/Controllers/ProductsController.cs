using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web.Controllers
{
    [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        private readonly DataContext _dataContext;
        private readonly IRepository<Products> _productrepository;

        public ProductsController(IProductRepository repository, IFlashMessage flashMessage,
            IConverterHelper converterHelper, ICombosHelper combosHelper, IImageHelper imageHelper,
            DataContext dataContext, IRepository<Products> Productrepository)
        {
            _repository = repository;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _dataContext = dataContext;
            _productrepository = Productrepository;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _repository.GetAllRecordsAsync();
            return View(data);
        }
        [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null){
                return new NotFoundViewResult("_ResourceNotFound");
            }

            try{
                var product = await ProducsExists(id.Value);

                if (product == null)
                {
                    return new NotFoundViewResult("_ResourceNotFound");
                }

                product.IsDeleted = 1;
                //await _repository.UpdateAsync(rol);
               await _productrepository.UpdateAsync(product);
                _flashMessage.Confirmation($"The Product { product.Description } was deleted.");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"The Products can't be deleted because it has related records. {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<Products> ProducsExists(int id)
        {
            //var _prod = await _repository
            //    .GetAll()
            //    .Where(s => s.ProductId == id)
            //    .FirstOrDefaultAsync();

            var _prod = await _productrepository.GetAsync(id);
            return _prod;
        }

        [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
        public IActionResult Create()
        {
            var model = new AddProductViewModel {
                ComboTrademarks = _combosHelper.GetComboTrademarks(),
                ComboProdTypes = _combosHelper.GetComboProdcutTypes(),
                ComboActivationForms = _combosHelper.GetComboActivationForms(),
                ComboActivationTypes = _combosHelper.GetComboActivationTypes(),
                ComboSimTypes = _combosHelper.GetComboSimTypes(),
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string path = string.Empty;

                    if (model.ImageFile != null)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "Products");
                        //model.ImageId = path;
                    }

                    var prod = _converterHelper.ToProductEntity(model,true);
                    //await _repository.CreateAsync(prod);
                    await _productrepository.CreateAsync(prod);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe esta Producto.");
                        _flashMessage.Danger($"The Products can't be created because it has related records. {"Ya Existe este Producto"}");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                        _flashMessage.Danger($"Error .{dbUpdateException.Message}");
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                    _flashMessage.Danger($"The Products can't be created because it has related records.  {exception.Message}");
                }
            }
            model.ComboTrademarks = _combosHelper.GetComboTrademarks();
            model.ComboProdTypes = _combosHelper.GetComboProdcutTypes();
            model.ComboActivationForms = _combosHelper.GetComboActivationForms();
            model.ComboActivationTypes = _combosHelper.GetComboActivationTypes();
            model.ComboSimTypes = _combosHelper.GetComboSimTypes();
            return View(model);
        }
        [Authorize(Roles = "PowerfulUser,KAM-Administrador,Coordinador-Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var _model = await _repository.GetOnlyProductAsync(id.Value);
            if (_model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }
            var model = _converterHelper.ToProductViewModel(_model);

            model.ComboTrademarks = _combosHelper.GetComboTrademarks();
            model.ComboProdTypes = _combosHelper.GetComboProdcutTypes();
            model.ComboActivationForms = _combosHelper.GetComboActivationForms();
            model.ComboActivationTypes = _combosHelper.GetComboActivationTypes();
            model.ComboSimTypes = _combosHelper.GetComboSimTypes();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddProductViewModel model)
        {
            if (id != model.ProductId)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var _Prod = await _dataContext
                          .Products
                          .Where(u => u.ProductId == model.ProductId)
                          .FirstOrDefaultAsync();

                    _Prod.ProductId = model.ProductId;
                    _Prod.Description = model.Description.ToUpper().Trim() ?? _Prod.Description.ToUpper().Trim();
                    _Prod.CodeKey = model.ShortDescription.ToUpper().Trim() ?? _Prod.ShortDescription.ToUpper().Trim();
                    _Prod.ProductTypeId = model.ProductTypeId;
                    _Prod.ValueWithOutTax = model.ValueWithOutTax;
                    _Prod.TrademarkId = model.TrademarkId;
                    _Prod.Price = model.Price;
                    _Prod.UnitsInStock = _Prod.UnitsInStock == model.UnitsInStock ? _Prod.UnitsInStock : (_Prod.UnitsInStock + model.UnitsInStock);
                    _Prod.UnitsInStock = model.UnitsInStock;
                    _Prod.ActivationFormId = model.ActivationFormId;
                    _Prod.ActivationTypeId = model.ActivationTypeId;
                    _Prod.SimTypeId = model.SimTypeId;
                    _Prod.OraclepId = model.OraclepId;
                    _Prod.ShortDescription = model.ShortDescription.ToUpper().Trim() ?? _Prod.ShortDescription.ToUpper().Trim();
                    _Prod.IsDeleted = 0;
                    _Prod.RegistrationDate = DateTime.Now.ToUniversalTime();

                    //await _repository.UpdateAsync(_Prod);
                   await _productrepository.UpdateAsync(_Prod);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe este Producto.");
                        _flashMessage.Danger("The Products Type can't be deleted because it has related records.  {0}", "Ya existe este Producto");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                        _flashMessage.Danger("The Products Type can't be deleted because it has related records.  {0}", dbUpdateException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                    _flashMessage.Danger("The Products Type can't be deleted because it has related records.  {0}", exception.Message);
                }
            }
            model.ComboTrademarks = _combosHelper.GetComboTrademarks();
            model.ComboProdTypes = _combosHelper.GetComboProdcutTypes();
            model.ComboActivationForms = _combosHelper.GetComboActivationForms();
            model.ComboActivationTypes = _combosHelper.GetComboActivationTypes();
            model.ComboSimTypes = _combosHelper.GetComboSimTypes();
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            var model = await _repository.GetOnlyProductAsync(id.Value);

            if (model == null)
            {
                return new NotFoundViewResult("_ResourceNotFound");
            }

            return View(model);
        }

    }
}
