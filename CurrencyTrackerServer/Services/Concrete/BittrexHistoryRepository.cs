using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CurrencyTrackerServer.Data;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Services.Abstract;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Services.Concrete
{
    public class BittrexHistoryRepository : IBittrexHistoryRepository, IDisposable
    {
        private readonly BittrexContext _context;

        public BittrexHistoryRepository(BittrexContext context)
        {
            _context = context;
        }


        public IEnumerable<BittrexChange> GetHistory()
        {
            return _context.Changes.AsEnumerable();
        }

        public void Save()
        {
            try
            {
                _context.SaveChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ResetHistory()
        {
            _context.Changes.RemoveRange(_context.Changes);
            _context.SaveChanges();
        }

        public void Add(BittrexChange item)
        {
            if (item == null) { throw new Exception("ef"); }

            var result = _context.Changes.SingleOrDefault(c => c.Currency == item.Currency);
            _context.Changes.Add(item);
            //_context.SaveChanges();
        }

        public BittrexChange Find(string currency)
        {
            return _context.Changes.SingleOrDefault(e => e.Currency == currency);
        }

        public void Update(BittrexChange item)
        {
            if(item == null) { throw new Exception("ef");}
            _context.SaveChanges();
        }

        public void ResetHistoryIfOlderThan(double hours)
        {
            var cutoffDate = DateTime.Now - TimeSpan.FromHours(hours);
            _context.Changes.RemoveRange(_context.Changes.Where(c => cutoffDate > c.CreatedTime));
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}