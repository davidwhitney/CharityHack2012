using System.Collections.Generic;
using System.Web.Mvc;
using CharityHack2012.Code.Adapters;
using CharityHack2012.Models;
using JustGiving.Api.Sdk;
using System.Linq;
using JustGiving.Api.Sdk.Model.Search;
using CharitySearchResult = JustGiving.Api.Sdk.Model.Search.CharitySearchResult;

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
            var charityProfile = LoadCharityProfile(id);

            if (charityProfile.JgCharityData != null)
            {
                charityProfile.RelatedCharities = _jgClient.Search.CharitySearch(charityProfile.JgCharityData.Name) ?? new CharitySearchResults();
            }
            else
            {
                charityProfile.RelatedCharities = new CharitySearchResults {Results = new CharitySearchResult[0]};
            }

            return View(charityProfile);
        }

        public ActionResult Compare(string charityId1, string charityId2)
        {
            var charity1 = LoadCharityProfile(charityId1);
            var charity2 = LoadCharityProfile(charityId2);
            var list = new List<CharityProfile> {charity1, charity2};

            return View(list);
        }

        private CharityProfile LoadCharityProfile(string id)
        {
            var charityProfile = _charityComissionAdapter.LoadByRegNo(id);
            var charityNewsOnGuardian = _guardianApiAdapter.SearchContentByCharityName(charityProfile.CharityName);
            charityProfile.NewsItems = charityNewsOnGuardian.Response.Results;

            var vaguelyMatchingCharities = _jgClient.Search.CharitySearch(id);
            var thisCharity = vaguelyMatchingCharities.Results.FirstOrDefault(
                x => x.RegistrationNumber.Contains(id) && charityProfile.CharityName.Contains(x.Name.ToLower()));

            charityProfile.JgCharityData = thisCharity ?? new CharitySearchResult();
            charityProfile.CharityImage = "http://v3-sandbox.justgiving.com" + (thisCharity == null ? "" : thisCharity.LogoFileName);
            return charityProfile;
        }
    }
}