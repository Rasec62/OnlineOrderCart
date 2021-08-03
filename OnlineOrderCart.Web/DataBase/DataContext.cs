using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.DataBase
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<RoleGroups> RoleGroups { get; set; }
        public DbSet<Kams> Kams { get; set; }
        public DbSet<Distributors> Distributors { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<ProductsType> ProductsType { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<TypeofPayments> TypeofPayments { get; set; }
        public DbSet<SimTypes> SimTypes { get; set; }
        public DbSet<Trademarks> Trademarks { get; set; }
        public DbSet<ActivationsForm> ActivationsForm { get; set; }
        public DbSet<ActivationsType> ActivationsType { get; set; }
        public DbSet<UserActivations> UserActivations { get; set; }
        public DbSet<Purposes> Purposes { get; set; }
        public DbSet<Warehouses> Warehouses { get; set; }
        public DbSet<DeatilWarehouses> DeatilWarehouses { get; set; }
        public DbSet<PrOrders> PrOrders { get; set; }
        public DbSet<PrOrderDetails> PrOrderDetails { get; set; }
        public DbSet<PrOrderDetailTmps> prOrderDetailTmps { get; set; }
    }
}
