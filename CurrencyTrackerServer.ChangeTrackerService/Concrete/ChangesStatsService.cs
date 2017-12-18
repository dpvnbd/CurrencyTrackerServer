﻿using System;
using System.Collections.Generic;
using System.Linq;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract.Changes;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
  public class ChangesStatsService : IChangesStatsService<CurrencyChangeApiData>
  {
    private readonly IRepositoryFactory _repoFactory;
    private int _resetHours;
    public ChangesStatsService(IRepositoryFactory repoFactory, IOptions<AppSettings> settings)
    {
      _repoFactory = repoFactory;
      _resetHours = settings.Value.ChangesStatsPeriodHours;
    }

    public void UpdateStates(IEnumerable<CurrencyChangeApiData> changes)
    {
      using (var repo = _repoFactory.Create<StatsCurrencyState>())
      {
        foreach (var state in repo.GetAll())
        {
          if (DateTimeOffset.Now - state.Created >= TimeSpan.FromHours(_resetHours))
          {
            repo.Delete(state, false);
          }
        }
        repo.SaveChanges();

        foreach (var change in changes)
        {
          if (change.PercentChanged < 0)
          {
            continue;
          }

          var state = repo.GetAll().FirstOrDefault(s => s.Currency == change.Currency &&
          s.UpdateSource == change.UpdateSource);

          if (state != null)
          {
            if (change.PercentChanged > state.Percentage)
            {
              state.Percentage = change.PercentChanged;
              state.LastChangeTime = DateTimeOffset.Now;
              repo.Update(state, false);
            }
          }
          else
          {
            // TODO - use automapper
            state = new StatsCurrencyState
            {
              Created = DateTimeOffset.Now,
              LastChangeTime = DateTimeOffset.Now,
              Currency = change.Currency,
              UpdateSource = change.UpdateSource,
              Percentage = change.PercentChanged
            };
            repo.Add(state, false);
          }
        }
        repo.SaveChanges();
      }
    }

    public IEnumerable<StatsCurrencyState> GetStates(UpdateSource source)
    {
      using (var repo = _repoFactory.Create<StatsCurrencyState>())
      {
        foreach (var state in repo.GetAll())
        {
          if (DateTimeOffset.Now - state.Created >= TimeSpan.FromHours(_resetHours))
          {
            repo.Delete(state, false);
          }
        }
        repo.SaveChanges();

        return repo.GetAll().Where(s => s.UpdateSource == source).AsNoTracking().ToList();
      }
    }
  }
}