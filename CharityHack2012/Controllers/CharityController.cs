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

            var vaguelyMatchingCharities = _jgClient.Search.CharitySearch(id);
            var thisCharity = vaguelyMatchingCharities.Results.FirstOrDefault(
                x => x.RegistrationNumber.Contains(id) && charityProfile.CharityName.Contains(x.Name.ToLower()));

            if (thisCharity != null)
            {
                thisCharity.RegistrationNumber = thisCharity.RegistrationNumber.FixRegistrationNumber();
            }

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