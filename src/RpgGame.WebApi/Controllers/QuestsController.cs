using Microsoft.AspNetCore.Mvc;

namespace RpgGame.WebApi.Controllers
{
    public class QuestsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
