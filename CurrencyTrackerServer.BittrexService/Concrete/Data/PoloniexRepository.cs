namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    internal class PoloniexRepository<T> : Repository<T> where T : class
    {
        public PoloniexRepository() : base(new PoloniexContext())
        {
        }
    }
}
