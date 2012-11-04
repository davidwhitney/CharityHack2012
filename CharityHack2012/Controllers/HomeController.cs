using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustGiving.Api.Sdk;

namespace CharityHack2012.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJustGivingClient _jgClient;

        public HomeController(IJustGivingClient jgClient)
        {
            _jgClient = jgClient;
        }

        [HttpGet]
        public ActionResult Index(string query = null)
        {
            var resultsForRender = new List<CharitySearchResult>();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var results = _jgClient.Search.CharitySearch(query);

                resultsForRender.AddRange(results.Results.Select(charitySearchResult => new CharitySearchResult
                    {
                        CharityDisplayName = charitySearchResult.Name,
                        CharityRegistrationNumber = charitySearchResult.RegistrationNumber,
                        JgCharityId = Int32.Parse(charitySearchResult.CharityId)
                    }));
            }

            return View(resultsForRender);
        }

        [HttpPost]
        public ActionResult Search(string query)
        {
            return RedirectToAction("Index", "Home", new {query = query});
        }
    }

    public class CharitySearchResult
    {
        public int JgCharityId { get; set; }
        public string CharityDisplayName { get; set; }
        public string CharityRegistrationNumber { get; set; }
    }
}
