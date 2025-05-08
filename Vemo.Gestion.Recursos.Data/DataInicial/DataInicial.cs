using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vemo.Gestion.Recursos.Data.Entidades;
using Vemo.Gestion.Recursos.Data.Models;

namespace Vemo.Gestion.Recursos.Data.DataInicial
{
    public class DataInicial : IDataInicial
    {
        private readonly ILogger<DataInicial> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuarios> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DataInicial(
            ILogger<DataInicial> logger,
            ApplicationDbContext context,
            UserManager<Usuarios> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }



        public async Task InsercionDatos()
        {
            await AddMigraciones();

            await AddRoles();
            
            var admin = await AddAdminUsuario();

            if (admin != null)
            {
                await AddAdminUsuarioRoles(admin);
            }
        }




        public async Task AddMigraciones()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                    await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error de migración: {ex.Message}");
            }
        }


        

        public async Task AddRoles()
        {
            var roles = new RolesAcceso();


            foreach (var rol in roles.GetRoles())
            {
                if (await _roleManager.RoleExistsAsync(rol.Text))
                    continue;


                var result = await _roleManager.CreateAsync(new IdentityRole(rol.Text));


                if (!result.Succeeded)
                {
                    var errores = result.Errors.Select(e => e.Description).ToList();
                    _logger.LogError($"Error al crear los roles: {string.Join(", ", errores)}");
                }
            }
        }



        public async Task<Usuarios?> AddAdminUsuario()
        {
            var adminUser = new Usuarios()
            {
                UserName = "admin",
                Email = "admin@example.com",
                Nombre = "Miguel",
                ApellidoPaterno = "Montaño",
                ApellidoMaterno = "L.",
                FechaNacimiento = new DateTime(DateTime.Now.Year - 20, DateTime.Now.Month, DateTime.Now.Day)
            };

            if ((await _userManager.FindByEmailAsync(adminUser.Email)) == null)
            {
                var result = await _userManager.CreateAsync(adminUser, "Admin#1234");


                if (!result.Succeeded)
                {
                    var errores = result.Errors.Select(e => e.Description).ToList();
                    _logger.LogError($"Error al crear el usuario administrador: {string.Join(", ", errores)}");

                    return null;
                }
            }

            return adminUser;
        }



        public async Task AddAdminUsuarioRoles(Usuarios adminUser)
        {
            var roles = new RolesAcceso();


            foreach (var rol in roles.GetRoles())
            {
                bool isInRole = await _userManager.IsInRoleAsync(adminUser, rol.Text);

                
                if (!isInRole)
                {
                    var result = await _userManager.AddToRoleAsync(adminUser, rol.Text);


                    if (!result.Succeeded)
                    {
                        var errores = result.Errors.Select(e => e.Description).ToList();
                        _logger.LogError($"Error al asignar los roles al usuario: {string.Join(", ", errores)}");
                    }
                }
            }
        }
    }
}