using Microsoft.AspNetCore.Mvc;

namespace AjaxWorks.Controllers
{
    public class HWController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
