using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyTrackerServer.Infrastructure.Entities.Notices
{
    public class Notice:BaseChangeEntity
    {
        public override UpdateDestination Destination => UpdateDestination.Notices;
    }
}
