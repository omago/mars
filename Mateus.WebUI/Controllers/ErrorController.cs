using System.Web.Mvc;

namespace Mateus.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View("UnauthorizedAccess");
        }

        public ActionResult UnauthorizedAccess()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View("PageNotFound");
        }


        public ActionResult InternalServerError()
        {
            return View("InternalServerError");
        }

        public ActionResult ApplicationOffline()
        {
            return View("ApplicationOffline");
        }

    }
}
