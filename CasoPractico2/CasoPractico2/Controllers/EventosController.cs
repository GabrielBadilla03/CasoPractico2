using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CasoPractico2.Data;
using CasoPractico2.Models;
using Microsoft.AspNetCore.Authorization;

namespace CasoPractico2.Controllers
{
    [Authorize(Roles = "Administrador,Organizador")]
    public class EventosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Eventos
        public async Task<IActionResult> Index()
        {
            IQueryable<Evento> eventosQuery = _context.Eventos
                .Include(e => e.Categoria)
                .Include(e => e.Usuario);

            // Solo se filtra si NO es administrador
            if (!User.IsInRole("Administrador") && User.IsInRole("Organizador"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    eventosQuery = eventosQuery.Where(e => e.UsuarioId == userId);
                }
            }

            var eventos = await eventosQuery.ToListAsync();
            return View(eventos);
        }


        // GET: Eventos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.Categoria)
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (evento == null)
            {
                return NotFound();
            }

            // Conteo de inscripciones para este evento
            var inscripcionesCount = await _context.Inscripciones
                .CountAsync(i => i.EventoId == evento.Id);

            // Evaluar si el evento está lleno
            bool eventoLleno = inscripcionesCount >= evento.CupoMaximo;

            // Enviar datos a la vista usando ViewData
            ViewData["InscripcionesCount"] = inscripcionesCount;
            ViewData["EventoLleno"] = eventoLleno;

            return View(evento);
        }

        // GET: Eventos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            return View();
        }

        // POST: Eventos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int Id,
            string Titulo,
            string Descripcion,
            int CategoriaId,
            DateTime Fecha,
            TimeSpan Hora,
            int DuracionMinutos,
            string Ubicacion,
            int CupoMaximo)
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var evento = new Evento
            {
                Id = Id,
                Titulo = Titulo,
                Descripcion = Descripcion,
                CategoriaId = CategoriaId,
                Fecha = Fecha,
                DuracionMinutos = DuracionMinutos,
                Ubicacion = Ubicacion,
                CupoMaximo = CupoMaximo,
                FechaRegistro = DateTime.Now,
                UsuarioId = userId
            };

            if (Fecha <= DateTime.Now)
            {
                ModelState.AddModelError("Fecha", "La fecha del evento debe ser en el futuro.");
            }

            if (DuracionMinutos <= 0)
            {
                ModelState.AddModelError("DuracionMinutos", "La duración debe ser mayor a 0.");
            }

            if (CupoMaximo <= 0)
            {
                ModelState.AddModelError("CupoMaximo", "El cupo máximo debe ser mayor a 0.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", evento.CategoriaId);
            return View(evento);
        }

        // GET: Eventos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", evento.CategoriaId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", evento.UsuarioId);
            return View(evento);
        }

        // POST: Eventos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descripcion,CategoriaId,Fecha,Hora,DuracionMinutos,Ubicacion,CupoMaximo,FechaRegistro,UsuarioId")] Evento evento)
        {
            if (id != evento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.Id))
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
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", evento.CategoriaId);
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", evento.UsuarioId);
            return View(evento);
        }

        // GET: Eventos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.Categoria)
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }
    }
}