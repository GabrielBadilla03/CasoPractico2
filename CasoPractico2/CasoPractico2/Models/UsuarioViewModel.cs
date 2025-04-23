namespace CasoPractico2.Models
{
    public class UsuarioViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NombreCompleto { get; set; } 
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        
        public string SelectedRole { get; set; }
        public List<string> Roles { get; set; }
    }
}
