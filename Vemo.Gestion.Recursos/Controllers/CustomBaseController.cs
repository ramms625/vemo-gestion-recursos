using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vemo.Gestion.Recursos.Data.Entidades;
using Vemo.Gestion.Recursos.Data.Models;

namespace Vemo.Gestion.Recursos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        private readonly UserManager<Usuarios> _userManager;
        public CustomBaseController(
            UserManager<Usuarios> userManager)
        {
            _userManager = userManager;
        }



        protected IActionResult GetNoContentResponse(HttpStatusCode statusCode, string message = "")
        {
            return StatusCode((int)statusCode, new NoContentApiResponse(message));
        }



        protected IActionResult GetResponse(HttpStatusCode statusCode, object data, string message = "")
        {
            return StatusCode((int)statusCode, new ApiResponse<object>(message: message, data: data));
        }



        protected async Task<Usuarios?> GetUsuarioLogueado()
        {
            var id = HttpContext.User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(id))
                return null;

            return await _userManager.FindByIdAsync(id);
        }



        protected async Task<Usuarios?> GetUsuarioPorEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}