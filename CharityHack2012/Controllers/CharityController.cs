using System.Web.Mvc;
using CharityHack2012.Code.Adapters;
using JustGiving.Api.Sdk;
using System.Linq;
using JustGiving.Api.Sdk.Model.Search;

namespace CharityHack2012.Controllers
{
    public class CharityController : Controller
    {
        private readonly CharityComissionAdapter _charityComissionAdapter;
        private readonly GuardianApiAdapter _guardianApiAdapter;
        private readonly IJustGivingClient _jgClient;

        public CharityController(CharityComissionAdapter charityComissionAdapter, GuardianApiAdapter guardianApiAdapter, IJustGivingClient jgClient)
        {
            _charityComissionAdapter = charityComissionAdapter;
            _guardianApiAdapter = guardianApiAdapter;
            _jgClient = jgClient;
        }

        public ActionResult Index(string id)
        {
            var charityProfile = _charityComissionAdapter.LoadByRegNo(id);
            var charityNewsOnGuardian = _guardianApiAdapter.SearchContentByCharityName(charityProfile.CharityName);
            charityProfile.NewsItems = charityNewsOnGuardian.Response.Results; 

            var vaguelyMatchingCharities = _jgClient.Search.CharitySearch(id);
            var thisCharity =
                vaguelyMatchingCharities.Results.FirstOrDefault(
                    x => x.RegistrationNumber.Contains(id) && charityProfile.CharityName.Contains(x.Name.ToLower()));

            charityProfile.JgCharityData = thisCharity ?? new CharitySearchResult();
            charityProfile.CharityImage = "http://v3-sandbox.justgiving.com" + (thisCharity == null ? "" : thisCharity.LogoFileName);

            if (thisCharity != null)
            {
                charityProfile.RelatedCharities = _jgClient.Search.CharitySearch(thisCharity.Name) ??
                                                  new CharitySearchResults();
            }
            else
            {
                charityProfile.RelatedCharities = new CharitySearchResults {Results = new CharitySearchResult[0]};
            }

            return View(charityProfile);
        }
    }
}