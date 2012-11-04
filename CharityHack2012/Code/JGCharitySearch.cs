using JustGiving.Api.Sdk.Model.Charity;
using Microsoft.Http;
using Newtonsoft.Json;

namespace CharityHack2012.Code
{
    public static class JGCharitySearch
    {
        public static CharityEntity GetCharityDetails(int charityId)
        {
            var httpClient = new HttpClient("https://api-sandbox.justgiving.com");
            var httpResponseMessage = httpClient.Get(string.Format("/8b347861/v1/charity/{0}?format={1}", charityId, "json"));
            return JsonConvert.DeserializeObject<CharityEntity>(httpResponseMessage.Content.ReadAsString());
        }
    }

    public class CharityEntity : Charity
    {
        public string Keywords { get; set; }
    }
}