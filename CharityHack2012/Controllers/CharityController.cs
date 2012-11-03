using System.Web.Mvc;
using CharityHack2012.Code.Adapters;
using CharityHack2012.Models;

namespace CharityHack2012.Controllers
{
    public class CharityController : Controller
    {
        private readonly CharityComissionAdapter _adapter;

        public CharityController(CharityComissionAdapter adapter)
        {
            _adapter = adapter;
        }

        public ActionResult Index(string id)
        {
            var charityProfile = _adapter.LoadByRegNo(id);

            return View(charityProfile);
        }
    }
}