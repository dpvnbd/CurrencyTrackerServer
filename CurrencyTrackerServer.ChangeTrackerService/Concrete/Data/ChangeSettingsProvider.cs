using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    public class ChangeSettingsProvider : FileDictionarySettingsProvider<ChangeSettings>
    {
        protected override string Filename => "changeSettings.json";
    }
}