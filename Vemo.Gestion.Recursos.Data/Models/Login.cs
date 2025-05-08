using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Vemo.Gestion.Recursos.Data.Models
{
    public class Login
    {
        [DisplayName("correo electrónico")]
        [Required(ErrorMessage = "El {0} es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ser un {0} válido.")]
        public string Email { get; set; }

        [DisplayName("contraseña")]
        [Required(ErrorMessage = "La {0} es obligatoria")]
        [MinLength(6, ErrorMessage = "La {0} debe tener al menos {1} caracteres.")]
        public string Password { get; set; }
    }
}
