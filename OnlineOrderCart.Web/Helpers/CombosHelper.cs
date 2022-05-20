using AutoMapper.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Enums;
using OnlineOrderCart.Web.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Helpers
{
    public class CombosHelper: ICombosHelper
    {
        private readonly DataContext _dataContext;
        public CombosHelper(DataContext context)
        {
            _dataContext = context;
        }
        public IEnumerable<SelectListItem> GetComboProdcutTypes()
        {
            var list = _dataContext.ProductsType.Where(t => t.IsDeleted == 0)
                 .Select(pt => new SelectListItem
                 {
                     Text = pt.Description,
                     Value = $"{pt.ProductTypeId}"
                 })
                  .OrderBy(pt => pt.Text)
                  .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a  Prodcut Types...)",
                Value = "0"
            });

            return list;
        }
        public IEnumerable<SelectListItem> GetComboTrademarks()
        {
            var list = _dataContext.Trademarks.Where(t => t.IsDeleted == 0)
                 .Select(pt => new SelectListItem
                 {
                     Text = pt.Description,
                     Value = $"{pt.TrademarkId}"
                 })
                  .OrderBy(pt => pt.Text)
                  .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Trademarks...)",
                Value = "0"
            });

            return list;
        }
        public IEnumerable<SelectListItem> GetComboGenders()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "(Seleccione un Genero...)" },
                new SelectListItem { Value = "1", Text = Genders.Femenino.ToString() },
                new SelectListItem { Value = "2", Text = Genders.Masculino.ToString() },
                new SelectListItem { Value = "3", Text = Genders.Generico.ToString() }
            };

            return list;
        }
        public IEnumerable<SelectListItem> GetComboRoles()
        {
            int[] _marks = new int[3] { 1,2,3 };
            string[] marks = new string[1] {"K"};
            var list = _dataContext.Roles.Where(t => t.IsDeleted == 0 && marks.Contains(t.CodeKey))
                  .Select(pt => new SelectListItem
                  {
                      Text = pt.RolName,
                      Value = $"{pt.RolId}"
                  })
                   .OrderBy(pt => pt.Text)
                   .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a  Rol...)",
                Value = "0"
            });

            return list;
        }
        public IEnumerable<SelectListItem> GetComboKams()
        {
            long[] marks = new long[2] {2,3};
       
            var list = (from k in _dataContext.Kams
                        join u in _dataContext.Users
                        on k.UserId equals u.UserId
                        where(k.IsDeleted == 0 && marks.Contains(k.KamId))
                        select new SelectListItem {
                            Text = $"{u.FirstName}{"-"}{u.LastName1}{"-"}{u.LastName2}",
                            Value = $"{k.KamId}"
                        }).OrderBy(x => x.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Kam...)",
                Value = "0"
            });

            return list;
        }
        public IEnumerable<SelectListItem> GetComboDisRoles()
        {
            var list = _dataContext
                .Roles
                .Where(t => t.IsDeleted == 0 && t.CodeKey == "DD")
                  .Select(pt => new SelectListItem
                  {
                      Text = pt.RolName,
                      Value = $"{pt.RolId}"
                  })
                   .OrderBy(pt => pt.Text)
                   .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a  Rol...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboAllKams()
        {
            string[] marks = new string[2] { "KA", "K" };

            var list = (from k in _dataContext.Kams
                        join u in _dataContext.Users
                        on k.UserId equals u.UserId
                        join rg in _dataContext.RoleGroups
                        on u.UserId equals rg.UserId
                        join r in _dataContext.Roles
                        on rg.RolId equals r.RolId
                        where (k.IsDeleted == 0 && marks.Contains(r.CodeKey))
                        select new SelectListItem
                        {
                            Text = $"{u.FirstName}{"-"}{u.LastName1}{"-"}{u.LastName2}",
                            Value = $"{k.KamId}"
                        }).OrderBy(x => x.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Kam...)",
                Value = "0"
            });

            return list;
        }
        public IEnumerable<SelectListItem> GetComboActivationForms()
        {
            var list = _dataContext
               .ActivationsForm
               .Where(t => t.IsDeleted == 0 )
                 .Select(pt => new SelectListItem
                 {
                     Text = pt.Description,
                     Value = $"{pt.ActivationFormId}"
                 })
                  .OrderBy(pt => pt.Text)
                  .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a  Activation Form...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboActivationTypes()
        {
            var list = _dataContext
               .ActivationsType
               .Where(t => t.IsDeleted == 0)
                 .Select(pt => new SelectListItem
                 {
                     Text = pt.Description,
                     Value = $"{pt.ActivationTypeId}"
                 })
                  .OrderBy(pt => pt.Text)
                  .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Activation Type...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboSimTypes()
        {
            var list = _dataContext
               .SimTypes
               .Where(t => t.IsDeleted == 0)
                 .Select(pt => new SelectListItem
                 {
                     Text = pt.Description,
                     Value = $"{pt.SimTypeId}"
                 })
                  .OrderBy(pt => pt.Text)
                  .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Sim Type...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboDistributors()
        {
            int[] marks1 = new int[2] { 0, 100 };
            var list = _dataContext
              .Distributors
              .Where(t => marks1.Contains(t.IsDeleted))
                .Select(pt => new SelectListItem
                {
                    Text = pt.BusinessName,
                    Value = $"{pt.DistributorId}"
                })
                 .OrderBy(pt => pt.Text)
                 .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Distributor...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboProdcuts()
        {
            var list = _dataContext
               .Products
               .Where(t => t.IsDeleted == 0)
                 .Select(pt => new SelectListItem
                 {
                     Text = pt.Description,
                     Value = $"{pt.ProductId}"
                 })
                  .OrderBy(pt => pt.Text)
                  .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Products...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboPurposes()
        {
            var list = _dataContext
               .Purposes
               .Where(t => t.IsDeleted == 0)
                 .Select(pt => new SelectListItem
                 {
                     Text = pt.Description,
                     Value = $"{pt.PurposeId}"
                 })
                  .OrderBy(pt => pt.Text)
                  .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Purposes...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GettoNextComboProducts(long id, int SimTypeId)
        {
            var ProductGroup = (from w in _dataContext.Warehouses
                                join dw in _dataContext.DeatilWarehouses
                                on w.StoreId equals dw.StoreId
                                join p in _dataContext.Products
                                on dw.ProductId equals p.ProductId
                                where w.DistributorId == id && w.SimTypeId.Equals(SimTypeId)
                                select new { ProductId = dw.ProductId }).ToList();

            int[] productIDs = new int[ProductGroup.Count];
            int count = 0;
            foreach (var item in ProductGroup)
            {
                productIDs[count] = item.ProductId;
                count++;
            }

            var listII = (from p in _dataContext.Products
                          where !productIDs.Contains(p.ProductId) && p.SimTypeId.Equals(SimTypeId)
                          select new { p.ProductId, p.Description }).ToList();

            var list = listII.Select(pt => new SelectListItem
            {
                Text = pt.Description,
                Value = $"{pt.ProductId}"
            })
              .OrderBy(pt => pt.Text)
              .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Selecciona un Product...)",
                Value = "0"
            });

            return list;
        }
        public IEnumerable<SelectListItem> GettoNextDisComboProducts(long id, int SimTypeId)
        {
            var ProductGroup = (from w in _dataContext.Warehouses
                                join dw in _dataContext.DeatilWarehouses
                                on w.StoreId equals dw.StoreId
                                join p in _dataContext.Products
                                on dw.ProductId equals p.ProductId
                                where w.StoreId == id && w.SimTypeId.Equals(SimTypeId) && p.IsDeleted == 0
                                select new { ProductId = dw.ProductId }).ToList();

            int[] productIDs = new int[ProductGroup.Count];
            int count = 0;
            foreach (var item in ProductGroup)
            {
                productIDs[count] = item.ProductId;
                count++;
            }

            var listII = (from p in _dataContext.Products
                          where !productIDs.Contains(p.ProductId) && p.SimTypeId.Equals(SimTypeId)
                          select new { p.ProductId, p.Description }).ToList();

            var list = listII.Select(pt => new SelectListItem
            {
                Text = pt.Description,
                Value = $"{pt.ProductId}"
            })
              .OrderBy(pt => pt.Text)
              .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Selecciona un Product...)",
                Value = "0"
            });

            return list;
        }
        public IEnumerable<SelectListItem> GetComboWarehouses(long id)
        {
            var list = _dataContext
                .Warehouses
                .Where(t => t.IsDeleted == 0 && t.DistributorId == id)
                  .Select(pt => new SelectListItem
                  {
                      Text = pt.ShippingBranchName,
                      Value = $"{pt.StoreId}"
                  })
                   .OrderBy(pt => pt.Text)
                   .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Warehouse...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetOrderStatuses()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = OrderStatus.Pending.ToString() },
                new SelectListItem { Value = "1", Text = OrderStatus.Spreading.ToString() },
                new SelectListItem { Value = "2", Text = OrderStatus.Sent.ToString() },
                new SelectListItem { Value = "3", Text = OrderStatus.Confirmed.ToString() },
                new SelectListItem { Value = "4", Text = OrderStatus.Cancelled.ToString() }
            };
        }
        public IEnumerable<SelectListItem> GetNextDWComboProducts(long id)
        {
            var list = _dataContext
                .DeatilWarehouses
                .Include(p => p.Products)
                .Where(t => t.IsDeleted == 0 && t.StoreId == id)
                  .Select(pt => new SelectListItem
                  {
                      Text = pt.Products.Description,
                      Value = $"{pt.DeatilStoreId}"
                  })
                   .OrderBy(pt => pt.Text)
                   .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a DWProducts...)",
                Value = "0"
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetComboTypeofPayments()
        {
            var list = _dataContext
                .TypeofPayments
                .Where(t => t.IsDeleted == 0)
                  .Select(pt => new SelectListItem
                  {
                      Text = pt.PaymentName,
                      Value = $"{pt.TypeofPaymentId}"
                  })
                   .OrderBy(pt => pt.Text)
                   .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Type of Payments...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboCoordRoles()
        {
            int[] marks1 = new int[4] { 1, 2, 3, 6 };
            string[] marks = new string[1] { "C" };
            var list = _dataContext.Roles.Where(t => t.IsDeleted == 0 && marks.Contains(t.CodeKey))
                  .Select(pt => new SelectListItem
                  {
                      Text = pt.RolName,
                      Value = $"{pt.RolId}"
                  })
                   .OrderBy(pt => pt.Text)
                   .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a  Rol...)",
                Value = "0"
            });

            return list;
        }
        public IEnumerable<SelectListItem> GetComboKamCoords()
        {
            string[] marks = new string[2] { "KD", "K" };
            List<SelectListItem> Listfruits = new List<SelectListItem>();

            var _Listk = (from k in _dataContext.Kams
                          join u in _dataContext.Users on k.UserId equals u.UserId
                          where u.IsDistributor == 0 && u.IsDeleted == 0
                          && k.IsCoordinator.Equals(0) && k.EmployeeNumber != "911"
                          select new
                          {
                              KamId = k.KamId,
                              FullName = $"{u.FirstName}{" "}{u.LastName1}{" "}{u.LastName2}"
                          }).ToList();

            foreach (var item in _Listk)
            {
                var _ListC = (from k in _dataContext.Kams
                              join u in _dataContext.Users on k.UserId equals u.UserId
                              where u.IsDistributor == 0 && u.IsDeleted == 0
                              && k.IsCoordinator.Equals(1) && k.KamManagerId == item.KamId && k.EmployeeNumber != "911"
                              select new
                              {
                                  KamId = k.KamId,
                                  CFullName = $"{u.FirstName}{" "}{u.LastName1}{" "}{u.LastName2}"
                              }).FirstOrDefault();

                if (_ListC != null)
                {
                    Listfruits.Add(new SelectListItem
                    {
                        Text = $"{item.FullName}{" - "}{_ListC.CFullName}",
                        Value = $"{item.KamId.ToString()}"
                    });
                }
                else
                {
                    Listfruits.Add(new SelectListItem
                    {
                        Text = $"{item.FullName}",
                        Value = $"{item.KamId.ToString()}"
                    });
                }
            }

            Listfruits.Insert(0, new SelectListItem
            {
                Text = "(Seleccione un Kam...)",
                Value = "0"
            });
            return Listfruits;
        }
        public IEnumerable<SelectListItem> GetComboKDistributors(string UserName)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _UserK = _dataContext.Kams
                .Include(u => u.Users)
                .Where(k => k.IsDeleted == 0 && k.Users.UserName == UserName && k.Users.IsDistributor == 0)
                .FirstOrDefault();


            if (_UserK.IsCoordinator == 1){
                list = _dataContext
             .Distributors
             .Where(t => t.IsDeleted == 0 && t.KamId.Equals(_UserK.KamManagerId))
               .Select(pt => new SelectListItem
               {
                   Text = pt.BusinessName,
                   Value = $"{pt.DistributorId}"
               })
                .OrderBy(pt => pt.Text)
                .ToList();
            }
            else {
                list = _dataContext
             .Distributors
             .Where(t => t.IsDeleted == 0 && t.KamId.Equals(_UserK.KamId))
               .Select(pt => new SelectListItem
               {
                   Text = pt.BusinessName,
                   Value = $"{pt.DistributorId}"
               })
                .OrderBy(pt => pt.Text)
                .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "(Seleccione un Distribuidor...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboIdKDistributors(long UserId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _UserK = _dataContext.Kams
                .Include(u => u.Users)
                .Where(k => k.IsDeleted == 0 && k.Users.UserId == UserId && k.Users.IsDistributor == 0)
                .FirstOrDefault();


            if (_UserK.IsCoordinator == 1)
            {
                list = _dataContext
             .Distributors
             .Where(t => t.IsDeleted == 0 && t.KamId.Equals(_UserK.KamManagerId))
               .Select(pt => new SelectListItem
               {
                   Text = pt.BusinessName,
                   Value = $"{pt.DistributorId}"
               })
                .OrderBy(pt => pt.Text)
                .ToList();
            }
            else
            {
                list = _dataContext
             .Distributors
             .Where(t => t.IsDeleted == 0 && t.KamId.Equals(_UserK.KamId))
               .Select(pt => new SelectListItem
               {
                   Text = pt.BusinessName,
                   Value = $"{pt.DistributorId}"
               })
                .OrderBy(pt => pt.Text)
                .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "(Seleccione un Distribuidor...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboAllKDistributors()
        {
            var list = _dataContext
              .Distributors
              .Where(t => t.IsDeleted == 0)
                .Select(pt => new SelectListItem
                {
                    Text = pt.BusinessName,
                    Value = $"{pt.DistributorId}"
                })
                 .OrderBy(pt => pt.Text)
                 .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Selecciona un  Distribuidor...)",
                Value = "0"
            });
            return list;
        }
        public IEnumerable<SelectListItem> GetComboIncTypeofPayments()
        {
            var list = _dataContext
                .TypeofPayments
                .Where(t => t.IsDeleted == 0)
                  .Select(pt => new SelectListItem{
                      Text = pt.PaymentName,
                      Value = $"{pt.TypeofPaymentId}"
                  })
                   .OrderBy(pt => pt.Text)
                   .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Seleccione un tipo de Pago...)",
                Value = "0"
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetCombomodalsDisRoles()
        {
            var list = _dataContext
                .Roles
                .Where(t => t.IsDeleted == 0 && t.CodeKey == "DD")
                  .Select(pt => new SelectListItem
                  {
                      Text = pt.RolName,
                      Value = $"{pt.RolId}"
                  })
                   .OrderBy(pt => pt.Text)
                   .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a  Rol...)",
                Value = ""
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetCombomodalsGenders()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "(Seleccione un Genero...)" },
                new SelectListItem { Value = "1", Text = Genders.Femenino.ToString() },
                new SelectListItem { Value = "2", Text = Genders.Masculino.ToString() },
                new SelectListItem { Value = "3", Text = Genders.Generico.ToString() }
            };

            return list;
        }

        public IEnumerable<SelectListItem> GetCombomodalsKamCoords()
        {
            string[] marks = new string[2] { "KA", "K" };
            List<SelectListItem> Listfruits = new List<SelectListItem>();

            var _Listk = (from k in _dataContext.Kams
                          join u in _dataContext.Users on k.UserId equals u.UserId
                          where u.IsDistributor == 0 && u.IsDeleted == 0
                          && k.IsCoordinator.Equals(0) && k.EmployeeNumber != "911"
                          select new
                          {
                              KamId = k.KamId,
                              FullName = $"{u.FirstName}{" "}{u.LastName1}{" "}{u.LastName2}"
                          }).ToList();

            foreach (var item in _Listk)
            {
                var _ListC = (from k in _dataContext.Kams
                              join u in _dataContext.Users on k.UserId equals u.UserId
                              where u.IsDistributor == 0 && u.IsDeleted == 0
                              && k.IsCoordinator.Equals(1) && k.KamManagerId == item.KamId && k.EmployeeNumber != "911"
                              select new
                              {
                                  KamId = k.KamId,
                                  CFullName = $"{u.FirstName}{" "}{u.LastName1}{" "}{u.LastName2}"
                              }).FirstOrDefault();

                if (_ListC != null)
                {
                    Listfruits.Add(new SelectListItem
                    {
                        Text = $"{item.FullName}{" - "}{_ListC.CFullName}",
                        Value = $"{item.KamId.ToString()}"
                    });
                }
                else
                {
                    Listfruits.Add(new SelectListItem
                    {
                        Text = $"{item.FullName}",
                        Value = $"{item.KamId.ToString()}"
                    });
                }
            }

            Listfruits.Insert(0, new SelectListItem
            {
                Text = "(Seleccione un Kam...)",
                Value = ""
            });
            return Listfruits;
        }
    }
}
