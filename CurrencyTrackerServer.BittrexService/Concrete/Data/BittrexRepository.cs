namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    internal class BittrexRepository<T> : Repository<T> where T : class
    {
        public BittrexRepository() : base(new BittrexContext())
        {
        }
    }
}