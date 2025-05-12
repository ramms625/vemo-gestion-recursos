using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Vemo.Gestion.Recursos.Data.DTOs;
using Vemo.Gestion.Recursos.Data.Entidades;
using Vemo.Gestion.Recursos.Data.Models;

namespace Vemo.Gestion.Recursos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : CustomBaseController
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Usuarios> _userManager;
        private readonly SignInManager<Usuarios> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CuentasController(
            IMapper mapper,
            IConfiguration configuration,
            UserManager<Usuarios> userManager,
            SignInManager<Usuarios> signInManager,
            RoleManager<IdentityRole> roleManager) : base(userManager)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }




        /// <summary>
        /// Permite iniciar sesión a un usuario.
        /// </summary>
        /// <param name="login">Credenciales del usuario (correo electrónico y contraseña).</param>
        /// <returns>Datos del token JWT (token y fecha de expiración).</returns>
        [HttpPost("Login")]
        [SwaggerOperation(Summary = "Inicia sesión", Description = "Permite a un usuario iniciar sesión con su correo y contraseña.")]
        [ProducesResponseType(typeof(ApiResponse<TokenData>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NoContentApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(NoContentApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            try
            {
                var usuario = await GetUsuarioPorEmail(login.Email);


                if (usuario == null)
                    return GetNoContentResponse(HttpStatusCode.NotFound, "Usuario o contraseña incorrecto(s).");


                var result = await _signInManager.PasswordSignInAsync(usuario, login.Password, isPersistent: false, lockoutOnFailure: false);


                if (!result.Succeeded)
                    return GetNoContentResponse(HttpStatusCode.NotFound, "Usuario o contraseña incorrecto(s).");


                var tokenData = await GetTokenData(usuario);


                return GetResponse(HttpStatusCode.OK, message: "Acceso concedido.", data: tokenData);
            }
            catch (Exception ex)
            {
                return GetNoContentResponse(HttpStatusCode.InternalServerError, message: ex.Message);
            }
        }







        /// <summary>
        /// Permite registrar un nuevo usuario.
        /// </summary>
        /// <param name="usuarioCreacion">Datos del usuario a registrarse</param>
        /// <returns>Un reflejo de la cuenta creada</returns>
        [HttpPost("Registro")]
        [SwaggerOperation(Summary = "Registro", Description = "Permite a un usuario darse de alta en el sistema.")]
        [ProducesResponseType(typeof(ApiResponse<UsuarioDTO>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(NoContentApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioCreacionDTO usuarioCreacion)
        {
            try
            {
                if ((await GetUsuarioPorEmail(usuarioCreacion.Email!)) != null)
                    return GetResponse(HttpStatusCode.BadRequest, new { errors = "El correo electrónico ya esta en uso".ToArray() } );


                
                var usuario = _mapper.Map<Usuarios>(usuarioCreacion);

                var result = await _userManager.CreateAsync(usuario, usuarioCreacion.Password!);

                if (!result.Succeeded)
                    return GetResponse(HttpStatusCode.BadRequest, new { errors = GetErrores(result.Errors) });


                
                
                result = await _userManager.AddToRoleAsync(usuario, RolesAcceso.Usuario);

                if (!result.Succeeded)
                    return GetResponse(HttpStatusCode.BadRequest, new { errors = GetErrores(result.Errors) });



                var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);

                return GetResponse(HttpStatusCode.Created, usuarioDTO, message: "Cuenta creada correctamente.");
            }
            catch (Exception ex)
            {
                return GetNoContentResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }








        private async Task<TokenData> GetTokenData(Usuarios userLogged)
        {
            var claims = new List<Claim>()
            {
                new ("id", userLogged.Id),
                new (ClaimTypes.Name, userLogged.UserName!)
            };


            var claimsDB = await _userManager.GetClaimsAsync(userLogged);
            
            if (claimsDB != null)
                claims.AddRange(claimsDB);


            foreach (var role in new RolesAcceso().GetRoles())
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Text));
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Encryption:TokenKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var expiration = DateTime.UtcNow.AddHours(1);


            var securityToken = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);


            return new TokenData
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }

        private static List<string> GetErrores(IEnumerable<IdentityError> errores)
        {
            return errores.Select(e => e.Description).ToList();
        }
    }
}