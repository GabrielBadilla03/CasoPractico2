using CasoPractico2.Data;
using CasoPractico2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace CasoPractico2.Controllers
{
    [Authorize(Roles = "Administrador,Usuario")]
    public class InscripcionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public InscripcionesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Inscripciones o Inscripciones?eventoId=5
        public async Task<IActionResult> Index(int? eventoId)
        {
            IQueryable<Inscripcion> inscripciones = _context.Inscripciones
            .Include(i => i.Evento)
            .Include(i => i.Usuario);
            if (eventoId != null)
            {
                inscripciones = inscripciones.Where(i => i.EventoId == eventoId);
                ViewData["FiltradoPorEvento"] = true;
                ViewData["TituloEvento"] = (await _context.Eventos.FindAsync(eventoId))?.Titulo;
            }
            else
            {
                ViewData["FiltradoPorEvento"] = false;
            }
            return View(await inscripciones.ToListAsync());
        }
        // GET: Inscripciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var inscripcion = await _context.Inscripciones
            .Include(i => i.Evento).Include(i => i.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);
            if (inscripcion == null)
            {
                return NotFound();
            }
            return View(inscripcion);
        }
        // GET: Inscripciones/Create
        public IActionResult Create()
        {
            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Titulo");
            return View();
        }
        // POST: Inscripciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventoId)
        {
            // Validar que el evento exista
            var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == eventoId);
            if (evento == null)
            {
                TempData["Error"] = "Evento no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Validar que no se haya alcanzado el cupo
            var cupoActual = await _context.Inscripciones.CountAsync(i => i.EventoId == eventoId);
            if (cupoActual >= evento.CupoMaximo)
            {
                TempData["Error"] = "El evento ha alcanzado su cupo máximo.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener el usuario actual
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                TempData["Error"] = "Usuario no autenticado.";
                return RedirectToAction(nameof(Index));
            }

            // Validar que el usuario no esté inscrito a otro evento en la misma fecha
            var eventosInscritos = await _context.Inscripciones
                .Where(i => i.UsuarioId == usuario.Id)
                .Include(i => i.Evento)
                .ToListAsync();

            foreach (var ins in eventosInscritos)
            {
                var inicioNuevo = evento.Fecha;
                var finNuevo = evento.Fecha.AddMinutes(evento.DuracionMinutos);

                var inicioExistente = ins.Evento.Fecha;
                var finExistente = ins.Evento.Fecha.AddMinutes(ins.Evento.DuracionMinutos);

                bool seSuperpone = inicioNuevo < finExistente && finNuevo > inicioExistente;
                if (seSuperpone)
                {
                    TempData["Error"] = "Ya estás inscrito en otro evento que se superpone en horario.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Crear nueva inscripción
            var inscripcion = new Inscripcion
            {
                UsuarioId = usuario.Id,
                EventoId = eventoId,
                FechaInscripcion = DateTime.Now,
                Asistio = false
            };

            _context.Add(inscripcion);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Inscripción realizada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Inscripciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Evento)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inscripcion == null)
            {
                return NotFound();
            }

            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Titulo", inscripcion.EventoId);

            // Solo el usuario relacionado con la inscripción
            var usuarioRelacionado = await _context.Users
                .Where(u => u.Id == inscripcion.UsuarioId)
                .ToListAsync();

            ViewData["UsuarioId"] = new SelectList(usuarioRelacionado, "Id", "UserName", inscripcion.UsuarioId);

            return View(inscripcion);
        }
        // POST: Inscripciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsuarioId,EventoId,FechaInscripcion,Asistio")] Inscripcion inscripcion)
        {
            if (id != inscripcion.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inscripcion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InscripcionExists(inscripcion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Titulo", inscripcion.EventoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", inscripcion.UsuarioId);
            return View(inscripcion);
        }
        // GET: Inscripciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var inscripcion = await _context.Inscripciones
            .Include(i => i.Evento)
            .Include(i => i.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);
            if (inscripcion == null)
            {
                return NotFound();
            }
            return View(inscripcion);
        }
        // POST: Inscripciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion != null)
            {
                _context.Inscripciones.Remove(inscripcion);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool InscripcionExists(int id)
        {
            return _context.Inscripciones.Any(e => e.Id == id);
        }
    }
}