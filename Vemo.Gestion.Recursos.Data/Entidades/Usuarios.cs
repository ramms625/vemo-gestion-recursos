using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vemo.Gestion.Recursos.Data.Entidades
{
    public class Usuarios : IdentityUser
    {
        [Column(TypeName = "nvarchar(50)")]
        public string Nombre { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string ApellidoPaterno { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string? ApellidoMaterno { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}