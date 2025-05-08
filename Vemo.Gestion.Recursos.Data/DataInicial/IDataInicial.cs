
using Vemo.Gestion.Recursos.Data.Entidades;

namespace Vemo.Gestion.Recursos.Data.DataInicial
{
    public interface IDataInicial
    {
        /// <summary>
        /// Metodo para inicializar la base de datos.
        /// </summary>
        /// <returns></returns>
        Task InsercionDatos();
        Task AddMigraciones();
        Task AddRoles();
        Task<Usuarios?> AddAdminUsuario();
        Task AddAdminUsuarioRoles(Usuarios usuario);
    }
}