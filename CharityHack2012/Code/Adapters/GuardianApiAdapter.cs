using System;
using System.Collections.Generic;
using RestSharp;

namespace CharityHack2012.Code.Adapters
{
    public class GuardianApiAdapter
    {
        public const string ApiKey = "cb7544ye6y758hp8fks5p4ke";
        public const int PageSize = 50;

        public GuardianEnvelope SearchContentByCharityName(string charityName)
        {
            var restClient = new RestClient("http://content.guardianapis.com");
            var startOfLastWeek = DateTime.Now.StartOfLastWeek();
            var request = new RestRequest("search?q={charityName}&page-size={pageSize}&from-date={fromDate}&to-date={toDate}&format=json&show-fields=all&api-key={apiKey}", Method.GET);
            request.AddUrlSegment("charityName", string.Format("\"{0}\"", charityName));
            request.AddUrlSegment("fromDate", startOfLastWeek.ToString("yyyy-MM-dd"));
            request.AddUrlSegment("toDate", startOfLastWeek.AddDays(6).ToString("yyyy-MM-dd"));
            request.AddUrlSegment("pageSize", "50");
            request.AddUrlSegment("apiKey", "cb7544ye6y758hp8fks5p4ke");
            return restClient.Execute<GuardianEnvelope>(request).Data;
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfLastWeek(this DateTime date)
        {
            var daysFromMonThisWeek = date.DayOfWeek - DayOfWeek.Monday + 7;
            return date.AddDays(-1 * daysFromMonThisWeek);
        }
    }

    public class GuardianEnvelope
    {
        public Response Response { get; set; }
    }

    public class Response
    {
        public List<Item> Results { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }
        public string SectionId { get; set; }
        public string SectionName { get; set; }
        public string WebPublicationDate { get; set; }
        public string WebTitle { get; set; }
        public string WebUrl { get; set; }
        public string ApiUrl { get; set; }

        public Fields Fields { get; set; }
    }

    public class Fields
    {
        private string _body;

        public string TrailText { get; set; }
        public string Headline { get; set; }
        public string Body
        {
            get { return _body; }
            set { _body = value == "<!-- Redistribution rights for this field are unavailable -->" ? string.Empty : value; }
        }

        public string ShortUrl { get; set; }
        public string Thumbnail { get; set; }
    }
}