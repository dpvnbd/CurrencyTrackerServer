using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Notices;

namespace CurrencyTrackerServer.NoticesService.Concrete
{
    public class PoloniexSiteNoticesDataSource:IDataSource<IEnumerable<Notice>>
    {
        private readonly string _noticesPageUrl;

        public PoloniexSiteNoticesDataSource()
        {
            _noticesPageUrl = "https://poloniex.com/lending";
        }
    public IEnumerable<Notice> GetData()
    {
      string pageHtml;
      using (var httpClient = new HttpClient())
      {
        var response = httpClient.GetAsync(_noticesPageUrl).Result;
        pageHtml = response.Content.ReadAsStringAsync().Result;
      }
      var notices = ParsePage(pageHtml);
      return notices;
    }

    public IEnumerable<Notice> ParsePage(string content)
        {
            var notices = new List<Notice>();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);

            var board = doc.GetElementbyId("noticesBoard");
            foreach (var node in board.ChildNodes)
            {
                if (node.Name == "div")
                {
                    var text = node.FirstChild.InnerText;
                    var timeText =
                        node.ChildNodes[1].InnerText.Split(new[] { "at" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    var time = DateTime.Parse(timeText);
                    notices.Add(new Notice { Time = time, Message = text, Source = Source});
                }
            }
            return notices;
        }

        public UpdateSource Source => UpdateSource.Poloniex;
    }
}
