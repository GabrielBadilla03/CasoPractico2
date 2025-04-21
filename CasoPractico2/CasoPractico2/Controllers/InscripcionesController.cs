using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CasoPractico2.Data;
using CasoPractico2.Models;
using Microsoft.AspNetCore.Identity;

namespace CasoPractico2.Controllers
{
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
                .Include(i => i.Evento)
                .Include(i => i.Usuario)
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
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Inscripciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsuarioId,EventoId,FechaInscripcion,Asistio")] Inscripcion inscripcion)
        {
            if (ModelState.IsValid)
            {
                var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == inscripcion.EventoId);

                if (evento == null)
                {
                    TempData["Error"] = "Evento no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var cupoActual = _context.Inscripciones.Count(i => i.EventoId == inscripcion.EventoId);
                if (cupoActual >= evento.CupoMaximo)
                {
                    TempData["Error"] = "El evento ha alcanzado su cupo máximo.";
                    return RedirectToAction(nameof(Index));
                }

                var usuario = await _userManager.GetUserAsync(User);
                var eventosInscritos = await _context.Inscripciones
                    .Where(i => i.UsuarioId == usuario.Id)
                    .Include(i => i.Evento)
                    .ToListAsync();

                foreach (var ins in eventosInscritos)
                {
                    if (ins.Evento.Fecha == evento.Fecha)
                    {
                        TempData["Error"] = "Ya estás inscrito en otro evento en este horario.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                _context.Add(inscripcion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Inscripción realizada con éxito.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Titulo", inscripcion.EventoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", inscripcion.UsuarioId);
            return View(inscripcion);
        }

        // GET: Inscripciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null)
            {
                return NotFound();
            }

            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Titulo", inscripcion.EventoId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", inscripcion.UsuarioId);
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