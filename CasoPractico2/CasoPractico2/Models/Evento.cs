using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CasoPractico2.Models
{
    public class Evento
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Range(1, int.MaxValue)]
        public int DuracionMinutos { get; set; }

        public string Ubicacion { get; set; }

        [Range(1, int.MaxValue)]
        public int CupoMaximo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string UsuarioId { get; set; }
        public IdentityUser? Usuario { get; set; }

        public ICollection<Inscripcion>? Inscripciones { get; set; }
    }
}
