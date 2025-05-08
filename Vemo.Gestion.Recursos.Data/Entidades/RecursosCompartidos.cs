using System.ComponentModel.DataAnnotations;

namespace Vemo.Gestion.Recursos.Data.Entidades
{
    public class RecursosCompartidos
    {
        [Key]
        public int Id { get; set; }
        public int CapacidadTotal { get; set; }
        public int CapacidadDisponible { get; set; }
    }
}