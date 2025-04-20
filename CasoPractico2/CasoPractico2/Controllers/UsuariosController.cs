using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CasoPractico2.Data;
using CasoPractico2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasoPractico2.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo los administradores pueden ver el listado de usuarios
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UsuariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Users.ToListAsync();

            var nombresCompletos = await _context.Database
        .SqlQuery<UsuarioNombreDTO>($"SELECT Id AS UserId, ISNULL(NombreCompleto, 'No definido') AS NombreCompleto FROM AspNetUsers")
        .ToListAsync();

            var usuariosConNombreCompleto = usuarios.Select(u => new UsuarioViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                NombreCompleto = nombresCompletos.FirstOrDefault(n => n.UserId == u.Id)?.NombreCompleto ?? "No definido",
                Roles = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .ToList()
            }).ToList();

            return View(usuariosConNombreCompleto);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(usuario);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar el usuario.");
                return View(usuario);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();

            // Obtener el rol actual del usuario
            var userRoles = await _userManager.GetRolesAsync(usuario);
            var selectedRole = userRoles.FirstOrDefault() ?? "Sin rol";

            // Obtener todos los roles disponibles
            ViewBag.Roles = new SelectList(await _roleManager.Roles.Select(r => r.Name).ToListAsync());

            var model = new UsuarioEditViewModel
            {
                Id = usuario.Id,
                UserName = usuario.UserName,
                Email = usuario.Email,
                PhoneNumber = usuario.PhoneNumber,
                SelectedRole = selectedRole
            };

            return View(model);
        }

        // POST: Usuarios/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(await _roleManager.Roles.Select(r => r.Name).ToListAsync());
                return View(model);
            }

            var usuario = await _userManager.FindByIdAsync(model.Id);
            if (usuario == null) return NotFound();

            usuario.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar el usuario.");
                return View(model);
            }
            var userRoles = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, userRoles);
            await _userManager.AddToRoleAsync(usuario, model.SelectedRole);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var usuarios = await _context.Users.Where(u => u.Id == id).ToListAsync();

            var nombresCompletos = await _context.Database
     .SqlQuery<UsuarioNombreDTO>($"SELECT Id AS UserId, ISNULL(NombreCompleto, 'No definido') AS NombreCompleto FROM AspNetUsers WHERE Id = '{id}'")
     .ToListAsync();

            var usuariosConNombreCompleto = usuarios.Select(u => new UsuarioViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                NombreCompleto = nombresCompletos.FirstOrDefault(n => n.UserId == u.Id)?.NombreCompleto ?? "No definido",
                Roles = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .ToList()
            }).FirstOrDefault();

            if (usuariosConNombreCompleto == null) return NotFound();

            return View(usuariosConNombreCompleto);
        }


    }
}
