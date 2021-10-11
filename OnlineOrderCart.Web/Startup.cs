using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineOrderCart.Web.DataBase;
using OnlineOrderCart.Web.DataBase.Repositories;
using OnlineOrderCart.Web.Helpers;
using OnlineOrderCart.Web.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereyon.Web;

namespace OnlineOrderCart.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<IdentityOptions>(options =>
            {
                // User settings.
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";

                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;

                // Lockout settings.
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            });
            //services.AddAuthentication()
            //    .AddCookie()
            //    .AddJwtBearer(cfg => {
            //        cfg.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidIssuer = Configuration["Tokens:Issuer"],
            //            ValidAudience = Configuration["Tokens:Audience"],
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:SecretKey"]))
            //        };
            //    });
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:SecretKey"])),
            //        ValidIssuer = Configuration["Token:Issuer"],
            //        ValidateIssuer = true,
            //        ValidateAudience = false
            //    };

            //});
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.AccessDeniedPath = "/account/NotAuthorized";
                options.LoginPath = "/Account/NotAuthorized";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
            });
           
            //services.ConfigureApplicationCookie(options => {
            //    options.LoginPath = "/Account/NotAuthorized";
            //    options.AccessDeniedPath = "/Account/NotAuthorized";
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
            //});



            //Register dapper in scope    
            services.AddScoped<IDapper, Dapperr>();
            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
            services.AddScoped<IBlobHelper, BlobHelper>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<ITrademarkRolRepository, TrademarkRolRepository>();
            services.AddScoped<IActivationsFormRepository, ActivationsFormRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductsTypeRespository, ProductsTypeRespository>();
            services.AddScoped<IActivationsTypeRepository, ActivationsTypeRepository>();
            services.AddScoped<ISimTypesRepository, SimTypesRepository>();
            services.AddScoped<IConverterHelper, ConverterHelper>();
            services.AddScoped<ICombosHelper, CombosHelper>();
            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IDistributorHelper, DistributorHelper>();
            services.AddScoped<IMailHelper, MailHelper>();
            services.AddScoped<IMovementsHelper, MovementsHelper>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IDevelopmentHelper, DevelopmentHelper>();
            services.AddScoped<ICreateFileOrFolder, CreateFileOrFolder>();
            services.AddScoped<IOrderIncentiveRepository, OrderIncentiveRepository>();
            

            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });

            services.AddMvcCore().AddNewtonsoftJson();
            //services.AddMvc()
            //    .AddNewtonsoftJson(options => {
            //        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            //    });


            services.AddControllersWithViews()
               .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
               .AddDataAnnotationsLocalization()
               .AddRazorRuntimeCompilation();

            //services.AddMvc()
            //    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            //    .AddDataAnnotationsLocalization();
            //    .AddNewtonsoftJson(options =>{
            //         options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            //    });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo> {
                    new CultureInfo("en-US"),
                    new CultureInfo("es-MX"),
                    new CultureInfo("pt-PT")
                };
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
           

            services.AddFlashMessage();

            services.AddSession();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {

                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/StatusCode404";
                    await next();
                }
                await next();
            });
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
