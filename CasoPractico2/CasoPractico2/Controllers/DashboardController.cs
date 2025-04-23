using CasoPractico2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasoPractico2.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var mesActual = DateTime.Now.Month;
            var añoActual = DateTime.Now.Year;

            var totalEventos = await _context.Eventos.CountAsync();
            var totalUsuarios = await _userManager.Users.CountAsync();
            var asistentesMesActual = await _context.Inscripciones
                .Where(i => i.FechaInscripcion.Month == mesActual && i.FechaInscripcion.Year == añoActual)
                .CountAsync();

            var topEventos = await _context.Eventos
                .Include(e => e.Inscripciones)
                .OrderByDescending(e => e.Inscripciones!.Count)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalEventos = totalEventos;
            ViewBag.TotalUsuarios = totalUsuarios;
            ViewBag.AsistentesMesActual = asistentesMesActual;
            ViewBag.TopEventos = topEventos;

            return View();
        }
    }
}
