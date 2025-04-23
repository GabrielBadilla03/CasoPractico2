using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CasoPractico2.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;    

        public UsuariosController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var usuarios = _userManager.Users.ToList();
            return View(usuarios);
        }
    }
}
