
namespace Vemo.Gestion.Recursos.Data.DTOs
{
    public class UsuarioDTO
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}