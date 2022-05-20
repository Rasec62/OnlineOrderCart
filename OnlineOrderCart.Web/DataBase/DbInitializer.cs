using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OnlineOrderCart.Web.DataBase
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext dataContext)
        {
            dataContext.Database.EnsureCreated();
            if (!dataContext.TypeofPayments.Any())
            {
                var _TypeofPay = new List<TypeofPayments>
                {
                            new TypeofPayments { PaymentName  = "PAGO CON LINEA DE CREDITO", CodeKey = "PLC", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime() },
                            new TypeofPayments { PaymentName  = "PAGO CON TRANSFERENCIA",  CodeKey = "PT", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime() },
                            new TypeofPayments { PaymentName  = "PAGO CON NOTAS DE CREDITO",  CodeKey = "PNC", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime()  },
                            new TypeofPayments { PaymentName  = "PAGO MIXTO (NDC - TRANSFERENCIA)",  CodeKey = "MIX NDC TR", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime()  },
                            new TypeofPayments { PaymentName  = "PAGO MIXTO (LDC - TRANSFERENCIA)",  CodeKey = "MIX LDC TR", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime()  },
                };

                dataContext.TypeofPayments.AddRange(_TypeofPay);
                dataContext.SaveChanges();
            }
            if (!dataContext.Roles.Any())
            {
                var _rol = new List<Roles>
                {
                            new Roles { RolName  = "PowerfulUser", CodeKey = "SP", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime() },
                            new Roles { RolName  = "KAM-Administrador",  CodeKey = "KA", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime() },
                            new Roles { RolName  = "Coordinador-Administrador",  CodeKey = "CA", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime()  },
                            new Roles { RolName  = "Coordinador",  CodeKey = "C", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime()  },
                            new Roles { RolName  = "Kam",  CodeKey = "K", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime()  },
                            new Roles { RolName  = "Distributor",  CodeKey = "DD", IsDeleted =0 , RegistrationDate = DateTime.Now.ToUniversalTime()  },

                };

                dataContext.Roles.AddRange(_rol);
                dataContext.SaveChanges();
            }
            if (!dataContext.Trademarks.Any())
            {
                var Trad = new List<Trademarks> { new Trademarks { Description = "AT&T", CodeKey = "AT&T", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },
                new Trademarks { Description = "UNEFON", CodeKey = "UF", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  } };
                dataContext.Trademarks.AddRange(Trad);
                dataContext.SaveChanges();
            }

            if (!dataContext.SimTypes.Any())
            {
                var Sim = new List<SimTypes> { new SimTypes { Description = "BENEFICIOS UNEFON", CodeKey = "BU", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },
                new SimTypes { Description = "DISPLAY", CodeKey = "D", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },
                new SimTypes { Description = "BENEFICIOS ATT", CodeKey = "BA", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  } };

                dataContext.SimTypes.AddRange(Sim);
                dataContext.SaveChanges();
            }

            if (!dataContext.ActivationsForm.Any())
            {
                var Acti = new List<ActivationsForm> { new ActivationsForm { Description = "AVS Light", CodeKey = "AL", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },
                new ActivationsForm { Description = "*100", CodeKey = "*1", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },
                new ActivationsForm { Description = "WEBSERVICE (API RES)", CodeKey = "WSAPI", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  }};
                dataContext.ActivationsForm.AddRange(Acti);
                dataContext.SaveChanges();
            }

            if (!dataContext.Purposes.Any())
            {
                var Acti = new List<Purposes> { new Purposes { Description = "SHIP_TO", CodeKey = "STO", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(), }, };
                dataContext.Purposes.AddRange(Acti);
                dataContext.SaveChanges();
            }

            if (!dataContext.ActivationsType.Any())
            {
                var Acti = new List<ActivationsType> { new ActivationsType { Description = "DISPLAY", CodeKey = "D", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },
                new ActivationsType { Description = "BENEFICIOS", CodeKey = "B", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },
                new ActivationsType { Description = "COMBO", CodeKey = "C", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  }};
                dataContext.ActivationsType.AddRange(Acti);
                dataContext.SaveChanges();
            }

            if (!dataContext.ProductsType.Any())
            {
                var pt = new List<ProductsType> { new ProductsType { Description = "SIMS", CodeKey = "sms", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },
                new ProductsType { Description = "COMBO", CodeKey = "CB", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },
                new ProductsType { Description = "HANDSET", CodeKey = "HST", IsDeleted = 0, RegistrationDate = DateTime.Now.ToUniversalTime(),  },};
                dataContext.ProductsType.AddRange(pt);
                dataContext.SaveChanges();
            }

            if (!dataContext.Kams.Any() && !dataContext.Users.Any())
            {
                CheckUsersAsync(dataContext, "S911",string.Empty, "quetzalcoatl.ometecuhtli@yopmail.com", "quetzalcoatl", "ometecuhtli", "Master", "QOM", "PowerfulUser", "2", "911");
                CheckUsersAsync(dataContext, "E711821", string.Empty, "ja512u@att.com", "Jose Antonio", "Argomaniz", "Arias", "JAAA", "KAM-Administrador", "2", "711821");
                CheckUsersAsync(dataContext, "E1029580", "E711821", "ja318m@att.com", "JESSICA", "AVILA", "FLORES", "JAF", "Coordinador-Administrador", "1", "1029580");
            }
        }
        private static bool CheckUsersAsync(DataContext dataContext, string Username1, string Manager, string userName, string FirstName, string LastName1, string LastName2
            , string codeKey, string role, string gender, string numberEm)
        {
            // Add user
            bool _Result = false;
            Kams _kams = new Kams();
            var strategy = dataContext.Database.CreateExecutionStrategy();
            strategy.Execute(() => {
                using (var transaction = dataContext.Database.BeginTransaction())
                {
                    try
                    {

                        var rol = dataContext.Roles
                       .Where(r => r.RolName == role)
                       .FirstOrDefault();

                        if (rol == null)
                        {
                            return;
                        }
                        var Users = new Users();
                        if (rol.CodeKey == "SP")
                        {
                            Users.FirstName = FirstName;
                            Users.LastName1 = LastName1;
                            Users.LastName2 = LastName2;
                            Users.Email = userName;
                            Users.UserName = $"S{numberEm}";
                            Users.GenderId = gender;
                            Users.PicturePath = $"{"~/image/users/avatar.png"}";
                            Users.Password = CreateSHA256("D12345467#");
                            Users.IsDeleted = 0;
                            Users.IsDistributor = 0;
                            Users.RegistrationDate = DateTime.Now.ToUniversalTime();
                        }
                        else
                        {
                            Users.FirstName = FirstName;
                            Users.LastName1 = LastName1;
                            Users.LastName2 = LastName2;
                            Users.Email = userName;
                            Users.PicturePath = $"{"~/image/users/avatar.png"}";
                            Users.UserName = $"E{numberEm}";
                            Users.GenderId = gender;
                            Users.Password = CreateSHA256("S4rSC0v*2");
                            Users.IsDeleted = 100;
                            Users.IsDistributor = 0;
                            Users.RegistrationDate = DateTime.Now.ToUniversalTime();

                        }
                        dataContext.Users.Add(Users);
                        dataContext.SaveChanges();

                        var _managerk = dataContext.Kams
                        .Include(u => u.Users).Where(k => k.Users.UserName == Manager)
                        .FirstOrDefault();

                        if (_managerk == null)
                        {

                            _kams.UserId = Users.UserId;
                            _kams.CodeKey = codeKey;
                            _kams.EmployeeNumber = numberEm;
                            _kams.IsCoordinator = 0;
                        }
                        else
                        {
                            _kams.UserId = Users.UserId;
                            _kams.CodeKey = codeKey;
                            _kams.EmployeeNumber = numberEm;
                            _kams.KamManagerId = _managerk.KamId;
                            _kams.IsCoordinator = 1;
                        }

                        _kams.IsDeleted = 0;
                        _kams.RegistrationDate = DateTime.Now.ToUniversalTime();
                        dataContext.Kams.Add(_kams);
                        dataContext.SaveChanges();

                        var UsersRol = new RoleGroups
                        {
                            RolId = rol.RolId,
                            UserId = Users.UserId,
                            IsDeleted = 0,
                            RegistrationDate = DateTime.Now.ToUniversalTime(),
                        };

                        dataContext.RoleGroups.Add(UsersRol);
                        dataContext.SaveChanges();

                        transaction.Commit();

                        _Result = true;
                    }
                    catch (Exception ex)
                    {
                        string messages = string.Empty;
                        transaction.Rollback();
                        messages = ex.InnerException.Message;
                        _Result = false;
                    }
                }
            });
            return _Result;
        }

        private static string CreateSHA256(string Pass)
        {
            try
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(Pass);
                    byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                    return hash;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
