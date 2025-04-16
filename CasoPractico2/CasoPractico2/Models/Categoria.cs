using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CasoPractico2.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string UsuarioId { get; set; }
        public IdentityUser? Usuario { get; set; }
    }
}
