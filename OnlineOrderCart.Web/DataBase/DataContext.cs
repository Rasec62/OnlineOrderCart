using Microsoft.EntityFrameworkCore;
using OnlineOrderCart.Common.Entities;

namespace OnlineOrderCart.Web.DataBase
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<RoleGroups> RoleGroups { get; set; }
        public virtual DbSet<Kams> Kams { get; set; }
        public virtual DbSet<Distributors> Distributors { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<ProductsType> ProductsType { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<TypeofPayments> TypeofPayments { get; set; }
        public virtual DbSet<SimTypes> SimTypes { get; set; }
        public virtual DbSet<Trademarks> Trademarks { get; set; }
        public virtual DbSet<ActivationsForm> ActivationsForm { get; set; }
        public virtual DbSet<ActivationsType> ActivationsType { get; set; }
        public virtual DbSet<UserActivations> UserActivations { get; set; }
        public virtual DbSet<Purposes> Purposes { get; set; }
        public virtual DbSet<Warehouses> Warehouses { get; set; }
        public virtual DbSet<DeatilWarehouses> DeatilWarehouses { get; set; }
        public virtual DbSet<PrOrders> PrOrders { get; set; }
        public virtual DbSet<PrOrderDetails> PrOrderDetails { get; set; }
        public virtual DbSet<PrOrderDetailTmps> prOrderDetailTmps { get; set; }
        public virtual DbSet<IncentiveOrderDetailTmp> IncentiveOrderDetailTmp { get; set; }
        public virtual DbSet<IncentiveOrders> IncentiveOrders { get; set; }
        public virtual DbSet<IncentiveOrderDetails> IncentiveOrderDetails { get; set; }
        public virtual DbSet<TblOptionalEmail> TblOptionalEmail { get; set; }
        public virtual DbSet<TblResetPasswords> TblResetPasswords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //  modelBuilder.Entity(typeof(Distributors))
            // .HasOne(typeof(Users), "Users")
            // .WithMany()
            // .HasForeignKey("UserId")
            // .OnDelete(DeleteBehavior.Restrict);

            //  modelBuilder.Entity(typeof(PrOrders))
            // .HasOne(typeof(Users), "Users")
            // .WithMany()
            // .HasForeignKey("UserId")
            // .OnDelete(DeleteBehavior.Restrict);

            //  modelBuilder.Entity(typeof(PrOrderDetails))
            //.HasOne(typeof(PrOrders), "PrOrders")
            //.WithMany()
            //.HasForeignKey("OrderId")
            //.OnDelete(DeleteBehavior.Restrict);
        }
    }
}
