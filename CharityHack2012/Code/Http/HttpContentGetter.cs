using System.Net.Http;

namespace CharityHack2012.Code.Http
{
    public class HttpContentGetter : IHttpContentGetter
    {
        public string Get(string uri)
        {
            var client = new HttpClient();

            var task = client.GetAsync(uri);
            task.Wait();
            var result = task.Result;
            var readBody = result.Content.ReadAsStringAsync();
            readBody.Wait();

            return readBody.Result;
        }
    }
}