using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Vemo.Gestion.Recursos.Data.DTOs
{
    public class UsuarioCreacionDTO
    {
        [Required(ErrorMessage = "El {0} es obligatorio")]
        [DisplayName("nombre")]
        public string Nombre { get; set; }


        [DisplayName("apellido paterno")]
        [Required(ErrorMessage = "El {0} es obligatorio")]
        public string ApellidoPaterno { get; set; }

        public string? ApellidoMaterno { get; set; }


        [DisplayName("correo electrónico")]
        [EmailAddress]
        [Required(ErrorMessage = "El {0} es obligatorio.")]
        public string Email { get; set; }

        public DateTime? FechaNacimiento { get; set; }
        
        
        [DisplayName("contraseña")]
        [Required(ErrorMessage = "La {0} es obligatoria")]
        [MinLength(6, ErrorMessage = "La {0} debe tener al menos {1} caracteres.")]
        public string Password { get; set; }
    }
}