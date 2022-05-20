using Microsoft.Extensions.DependencyInjection;
using OnlineOrderCart.Web.Application.Interfaces;

namespace OnlineOrderCart.Web.Application.RepositoryHelpers
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection service)
        {
            service.AddTransient<IUnitOfWork, UnitOfWork>();
            service.AddTransient<IIncentiveOrderDetailTmpRepository, IncentiveOrderDetailTmpRepository>();
        }
    }
}
