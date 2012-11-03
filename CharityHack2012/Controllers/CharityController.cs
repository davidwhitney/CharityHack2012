using System.Web.Mvc;

namespace CharityHack2012.Controllers
{
    public class CharityController : Controller
    {
        public ActionResult Index(string id)
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }
    }
}