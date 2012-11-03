using System.Web.Mvc;
using CharityHack2012.Code.Adapters;
using CharityHack2012.Models;

namespace CharityHack2012.Controllers
{
    public class CharityController : Controller
    {
        private readonly CharityComissionAdapter _charityComissionAdapter;
        private readonly GuardianApiAdapter _guardianApiAdapter;

        public CharityController(CharityComissionAdapter charityComissionAdapter, GuardianApiAdapter guardianApiAdapter)
        {
            _charityComissionAdapter = charityComissionAdapter;
            _guardianApiAdapter = guardianApiAdapter;
        }

        public ActionResult Index(string id)
        {
            var charityProfile = _charityComissionAdapter.LoadByRegNo(id);
            var charityNewsOnGuardian = _guardianApiAdapter.SearchContentByCharityName(charityProfile.CharityName);
            charityProfile.NewsItems = charityNewsOnGuardian.Response.Results; 

            return View(charityProfile);
        }
    }
}