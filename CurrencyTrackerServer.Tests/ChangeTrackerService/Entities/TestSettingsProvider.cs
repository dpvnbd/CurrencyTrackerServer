using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService.Entities
{
    class TestSettingsProvider:IChangeSettingsProvider
    {
        private readonly ChangeSettings _settings;

        public TestSettingsProvider(ChangeSettings settings)
        {
            _settings = settings;
        }
        public ChangeSettings GetSettings(ChangeSource key)
        {
            return _settings;
        }

        public void SaveSettings(ChangeSource key, ChangeSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
