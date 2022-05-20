using OnlineOrderCart.Web.Application.Interfaces;

namespace OnlineOrderCart.Web.Application.RepositoryHelpers
{
    public class UnitOfWork: IUnitOfWork
    {
        public UnitOfWork(IIncentiveOrderDetailTmpRepository IncentiveOrderDetailTmpRepository)
        {
            IncentiveOrderDetailTmps = IncentiveOrderDetailTmpRepository;
        }

        public IIncentiveOrderDetailTmpRepository IncentiveOrderDetailTmps { get; }
    }
}
