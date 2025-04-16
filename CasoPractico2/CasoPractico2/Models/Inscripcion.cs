using Microsoft.AspNetCore.Identity;

namespace CasoPractico2.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }

        public IdentityUser? Usuario { get; set; }

        public int EventoId { get; set; }

        public Evento? Evento { get; set; }

        public DateTime FechaInscripcion { get; set; }

        public bool Asistio { get; set; }
    }
}
