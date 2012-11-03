using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace CharityHack2012.Code.Adapters
{
    public class CharityComissionAdapter
    {
        public string CharityComissionBaseUri { get { return "http://www.charity-commission.gov.uk"; } }
        public string UrlPart { get { return "/Showcharity/RegisterOfCharities/CharityWithPartB.aspx?SubsidiaryNumber=0&RegisteredCharityNumber=";; } }

        public string CharityComissionUriForRegistrationNumber(string registrationNumber)
        {
            return CharityComissionBaseUri + UrlPart + registrationNumber;
        }

        public void LoadByRegNo(string regNo)
        {
            var uri = CharityComissionUriForRegistrationNumber(regNo);
            var client = new HttpClient();

            var task = client.GetAsync(uri);
            task.Wait();
            var result = task.Result;
            var readBody = result.Content.ReadAsStringAsync();
            readBody.Wait();
           
            var body = readBody.Result;

            var doc = new HtmlDocument();
            doc.LoadHtml(body);


            var charityName = doc.GetElementbyId("ctl00_charityStatus_spnCharityName").InnerText;

            //foreach (var link in doc.DocumentElement.SelectNodes("//a[@href"])
            {
                //HtmlAttribute att = link["href"];
                //att.Value = FixLink(att);
            }
            //doc.Save("file.htm");

        }
    }
}