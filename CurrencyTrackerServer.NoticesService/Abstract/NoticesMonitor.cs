using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CurrencyTrackerServer.Data.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Notices;
using Microsoft.EntityFrameworkCore.Internal;

namespace CurrencyTrackerServer.NoticesService.Abstract
{
    public class NoticesMonitor : IMonitor<IEnumerable<BaseChangeEntity>>
    {
        private readonly IRepositoryFactory _repoFactory;
        private readonly INotifier _notifier;

        public virtual UpdateSource Source { get; }
        public UpdateDestination Destination => UpdateDestination.Notices;
        public string UserId { get; }
        public event EventHandler<IEnumerable<BaseChangeEntity>> Changed;
        private NoticesComparer _comparer = new NoticesComparer();
        public NoticesMonitor(IRepositoryFactory repoFactory, INotifier notifier)
        {
            _repoFactory = repoFactory;
            _notifier = notifier;
        }

        public async void CheckNotices(object sender, IEnumerable<Notice> notices)
        {
            using (var repo = _repoFactory.Create<NoticeEntity>())
            {
                var saved = repo.GetAll().Select((e) =>
                     new Notice
                     {
                         Message = e.Message,
                         Time = e.Time,
                         Source = e.Source
                     }
                ).AsEnumerable();

                var distinct = notices.Distinct(_comparer.Equals);
                var newNotices = distinct.Except(saved, _comparer);

                if (newNotices.Any())
                {
                    foreach (var notice in newNotices.OrderBy(n => n.Time))
                    {
                        await repo.Add(new NoticeEntity
                        {
                            Source = notice.Source,
                            Message = notice.Message,
                            Time = notice.Time ?? DateTimeOffset.Now,
                        }, saveChanges: false);
                    }

                    await repo.SaveChanges();
                    OnMessage(newNotices);
                }
            }
        }

        public IEnumerable<Notice> GetNotices()
        {
            using (var repo = _repoFactory.Create<NoticeEntity>())
            {
                var notices = repo.GetAll().OrderBy(n => n.Time).TakeLast(5).ToList();
                return notices.Select((e) =>
                    new Notice
                    {
                        Message = e.Message,
                        Time = e.Time,
                        Source = e.Source
                    }
                );
            }
        }

        private async void OnMessage(IEnumerable<BaseChangeEntity> message)
        {
            await _notifier.SendToAll(message);
        }

        public void Dispose()
        {
        }

        public class NoticesComparer : IEqualityComparer<Notice>
        {
            private const string UrlRegex = @"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)";

            public bool Equals(Notice x, Notice y)
            {
                if (x.Source != y.Source) return false;

                var xText = Regex.Replace(x.Message, UrlRegex, string.Empty);
                var yText = Regex.Replace(y.Message, UrlRegex, string.Empty);
                xText = Regex.Replace(xText, @"\W+", string.Empty);
                yText = Regex.Replace(yText, @"\W+", string.Empty);
                return xText == yText;
            }

            public int GetHashCode(Notice codeh)
            {
                string xText = Regex.Replace(codeh.Message, UrlRegex, string.Empty);
                xText = Regex.Replace(xText, @"\W+", string.Empty);

                return xText.GetHashCode() + (int)codeh.Source;
            }
        }
    }
}
