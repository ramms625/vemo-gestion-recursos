using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vemo.Gestion.Recursos.Data.Entidades
{
    public class Sesiones
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string UsuarioId { get; set; }
        public int Consumo { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }
    }
}