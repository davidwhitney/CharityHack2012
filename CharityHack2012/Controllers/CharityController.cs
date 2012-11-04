using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using CharityHack2012.Code;
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
            CharitySearchResult thisCharity;
            var charityProfile = LoadCharityProfile(id, out thisCharity);

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

        public ActionResult Compare(string charityId1, string charityId2)
        {
            return null;
        }

        private CharityProfile LoadCharityProfile(string id, out CharitySearchResult thisCharity)
        {
            var charityProfile = _charityComissionAdapter.LoadByRegNo(id);

            var vaguelyMatchingCharities = _jgClient.Search.CharitySearch(id);

            thisCharity = vaguelyMatchingCharities.Results.FirstOrDefault(x => x.RegistrationNumber.Contains(id));
            if (CharityFoundOnJustGiving(thisCharity))
            {
                PopulateDataFromJustGiving(charityProfile, thisCharity);
            }
            else
            {
                SetupDefaultDataForCharity(charityProfile);
            }

            return charityProfile;
        }

        private void SetupDefaultDataForCharity(CharityProfile charityProfile)
        {
            charityProfile.JgCharityData = new CharityEntity {Description = charityProfile.CharityName};
            var charityNewsOnGuardian = _guardianApiAdapter.SearchContentByCharityName(charityProfile.CharityName);
            charityProfile.NewsItems = charityNewsOnGuardian.Response.Results ?? new List<Item>();
        }

        private void PopulateDataFromJustGiving(CharityProfile charityProfile, CharitySearchResult thisCharity)
        {
            charityProfile.JgCharityData = JGCharitySearch.GetCharityDetails(Convert.ToInt32(thisCharity.CharityId));
            charityProfile.CharityImage = "http://v3-sandbox.justgiving.com" + thisCharity.LogoFileName;
            var charityNewsOnGuardian = _guardianApiAdapter.SearchContentByCharityNameAndKeywords(charityProfile.JgCharityData);
            charityProfile.NewsItems = charityNewsOnGuardian.Response.Results ?? new List<Item>();
        }

        private static bool CharityFoundOnJustGiving(CharitySearchResult thisCharity)
        {
            return thisCharity != null;
        }
    }
}