using System.Web.Mvc;
using CharityHack2012.Code.Adapters;
using CharityHack2012.Models;
using JustGiving.Api.Sdk;
using System.Linq;

namespace CharityHack2012.Controllers
{
    public class CharityController : Controller
    {
        private readonly CharityComissionAdapter _adapter;
        private readonly IJustGivingClient _jgClient;

        public CharityController(CharityComissionAdapter adapter, IJustGivingClient jgClient)
        {
            _adapter = adapter;
            _jgClient = jgClient;
        }

        public ActionResult Index(string id)
        {
            var charityProfile = _adapter.LoadByRegNo(id);

            var vaguelyMatchingCharities = _jgClient.Search.CharitySearch(id);
            var thisCharity =
                vaguelyMatchingCharities.Results.FirstOrDefault(
                    x => x.RegistrationNumber.Contains(id) && charityProfile.CharityName.Contains(x.Name.ToLower()));

            charityProfile.JgCharityData = thisCharity;

            return View(charityProfile);
        }
    }
}