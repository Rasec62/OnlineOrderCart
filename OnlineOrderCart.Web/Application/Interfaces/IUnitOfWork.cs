namespace OnlineOrderCart.Web.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IIncentiveOrderDetailTmpRepository IncentiveOrderDetailTmps { get; }
    }
}
