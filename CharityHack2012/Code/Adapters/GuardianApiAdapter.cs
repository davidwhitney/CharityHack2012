using System;
using System.Collections.Generic;
using System.Text;
using JustGiving.Api.Sdk.Model.Charity;
using RestSharp;

namespace CharityHack2012.Code.Adapters
{
    public class GuardianApiAdapter
    {
        public const string ApiKey = "cb7544ye6y758hp8fks5p4ke";
        public const int PageSize = 50;

        private readonly IRestClient _restClient;


        public GuardianApiAdapter()
        {
            _restClient = new RestClient("http://content.guardianapis.com");
        }

        public GuardianEnvelope SearchContentByCharityName(string charityName)
        {
            return TryCustomSearchOrDefault(SearchContentOnGuardian(charityName));
        }

        private GuardianEnvelope TryCustomSearchOrDefault(GuardianEnvelope customSearch)
        {
            return NoResultsFound(customSearch) ? TopHeadlinesInTheCharitiesSector() : customSearch;
        }

        private static bool NoResultsFound(GuardianEnvelope result)
        {
            return result.Response.Results == null || result.Response.Results.Count == 0;
        }

        public GuardianEnvelope SearchContentByCharityNameAndKeywords(CharityEntity charity)
        {
            var searchTerms = SearchTermForCharity(charity);
            return TryCustomSearchOrDefault(SearchContentOnGuardian(searchTerms.ToString()));
        }

        private static StringBuilder SearchTermForCharity(CharityEntity charity)
        {
            if (charity.Keywords == null)
            {
                charity.Keywords = "";
            }
            var searchTerms = new StringBuilder();
            searchTerms.Append(charity.Name);
            foreach (var keyword in charity.Keywords.Split(','))
            {
                searchTerms.Append(" " + keyword);
            }
            return searchTerms;
        }

        private GuardianEnvelope TopHeadlinesInTheCharitiesSector()
        {
            var lastWeekStart = 1.WeeksAgoStart();
            var lastWeekEnd = lastWeekStart.AddDays(6);
            var request =
                new RestRequest(
                    "/society/charities?page-size={pageSize}&from-date={fromDate}&to-date={toDate}&format=json&show-fields=all&api-key={apiKey}",
                    Method.GET);
            request.AddUrlSegment("fromDate", lastWeekStart.ToString("yyyy-MM-dd"));
            request.AddUrlSegment("toDate", lastWeekEnd.ToString("yyyy-MM-dd"));
            request.AddUrlSegment("pageSize", "5");
            request.AddUrlSegment("apiKey", "cb7544ye6y758hp8fks5p4ke");
            return _restClient.Execute<GuardianEnvelope>(request).Data;
        }

        private GuardianEnvelope SearchContentOnGuardian(string searchTerm)
        {
            var monthsAgo = 6.MonthsAgo();
            var request =
                new RestRequest(
                    "search?q={searchTerm}&page-size={pageSize}&from-date={fromDate}&to-date={toDate}&format=json&show-fields=all&api-key={apiKey}&section=society&tag=society/charities",
                    Method.GET);
            request.AddUrlSegment("searchTerm", string.Format("\"{0}\"", searchTerm));
            request.AddUrlSegment("fromDate", monthsAgo.ToString("yyyy-MM-dd"));
            request.AddUrlSegment("toDate", DateTime.Now.ToString("yyyy-MM-dd"));
            request.AddUrlSegment("pageSize", "50");
            request.AddUrlSegment("apiKey", "cb7544ye6y758hp8fks5p4ke");
            return _restClient.Execute<GuardianEnvelope>(request).Data;
        }
    }

    public static class IntegerExtensions
    {
        public static DateTime MonthsAgo(this int number)
        {
            return DateTime.Now.AddMonths(-1 * number);
        }

        public static DateTime WeeksAgoStart(this int number)
        {
            var todaysDayOfWeek = DateTime.Now.DayOfWeek;
            var startOfLastWeek = todaysDayOfWeek - DayOfWeek.Sunday + 7;
            return DateTime.Now.AddDays(-startOfLastWeek);
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