using AjaxWorks.Models;
using Microsoft.AspNetCore.Mvc;

namespace AjaxWorks.Controllers
{
    public class APIController : Controller
    {
            private readonly MyDBContext _context;
            public APIController(MyDBContext context)
            {
                _context = context;
            }

            public IActionResult Index()
            {
                return View();
            }

            public IActionResult Cities()
            {
                var cities = _context.Addresses.Select(a => a.City).Distinct();
                return Json(cities);
            }

        
    }
}
