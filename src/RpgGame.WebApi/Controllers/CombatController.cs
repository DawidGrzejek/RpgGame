using Microsoft.AspNetCore.Mvc;

namespace RpgGame.WebApi.Controllers
{
    public class CombatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
