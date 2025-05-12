using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Vemo.Gestion.Recursos.Controllers;
using Vemo.Gestion.Recursos.Data.DTOs;
using Vemo.Gestion.Recursos.Data.Entidades;
using Vemo.Gestion.Recursos.Data.Models;

namespace Vemo.Gestion.Recursos.Tests.Pruebas.Cuentas
{
    [TestClass]
    public class LoginPruebas : ConfigPruebas
    {
        private IMapper _mapper;
        private CuentasController _controller;
        private Mock<IConfiguration> _configurationMock;
        private Mock<UserManager<Usuarios>> _userManagerMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private Mock<SignInManager<Usuarios>> _signInManagerMock;


        [TestInitialize]
        public void Setup()
        {
            _mapper = GetMapper();
            _roleManagerMock = GetRoleManager();
            _userManagerMock = GetUserManager();
            _signInManagerMock = GetSignInManager();
            _configurationMock = GetMockConfiguration();

            _controller = new CuentasController(
                _mapper,
                _configurationMock.Object,
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _roleManagerMock.Object);
        }



        [TestMethod]
        public async Task LoginExitoso()
        {
            // Arrange
            var login = new Login { Email = "user@vemo.com", Password = "Password#123" };
            
            var usuario = new Usuarios { Email = login.Email, UserName = "user" };

            
            _userManagerMock.Setup(um => um.FindByEmailAsync(login.Email)).ReturnsAsync(usuario);

            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(usuario, login.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);


            var tokenKey = GetConfiguration()["Encryption:TokenKey"];
            _configurationMock.Setup(c => c["Encryption:TokenKey"]).Returns(tokenKey);
            

            // Act
            var result = await _controller.Login(login) as ObjectResult;


            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);


            var response = result.Value as dynamic;
            var data = response?.Data as TokenData;

            Assert.IsNotNull(data);
            Assert.IsTrue(!string.IsNullOrEmpty(response?.Data.Token));
        }











        [TestMethod]
        public async Task LoginIncorrecto()
        {
            // Arrange
            var login = new Login { Email = "user@vemo.com", Password = "WrongPassword" };
            var usuario = new Usuarios { Email = login.Email, UserName = "user" };


            // Simular que el usuario existe
            _userManagerMock.Setup(um => um.FindByEmailAsync(login.Email)).ReturnsAsync(usuario);


            // Simular que la contraseña es incorrecta
            _signInManagerMock
                .Setup(sm => sm.PasswordSignInAsync(usuario, login.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);


            // Act
            var result = await _controller.Login(login) as ObjectResult;


            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }









        [TestMethod]
        public async Task RegistroExitoso()
        {
            // Arrange
            var usuarioCreacion = new UsuarioCreacionDTO
            {
                Nombre = "Juan",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "Gómez",
                Email = "juan.perez@vemo.com",
                Password = "Password#123",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };
            
            var usuario = new Usuarios
            {
                Nombre = usuarioCreacion.Nombre,
                ApellidoPaterno = usuarioCreacion.ApellidoPaterno,
                ApellidoMaterno = usuarioCreacion.ApellidoMaterno,
                Email = usuarioCreacion.Email,
                UserName = usuarioCreacion.Email,
                FechaNacimiento = usuarioCreacion.FechaNacimiento
            };

            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<Usuarios>(), usuarioCreacion.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<Usuarios>(), "Usuario"))
                .ReturnsAsync(IdentityResult.Success);
            

            // Act
            var result = await _controller.Registro(usuarioCreacion) as ObjectResult;


            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);

            

            var response = result.Value as dynamic;
            var data = response?.Data as UsuarioDTO;



            Assert.IsNotNull(response);
            Assert.AreEqual(usuarioCreacion.Email, data?.Email);
            Assert.AreEqual(usuarioCreacion.Nombre, data?.Nombre);
            Assert.AreEqual(usuarioCreacion.ApellidoPaterno, data?.ApellidoPaterno);
        }










        [TestMethod]
        public async Task Registro_Incorrecto()
        {
            // Arrange
            var usuarioCreacion = new UsuarioCreacionDTO
            {
                Nombre = "Juan",
                ApellidoPaterno = "Pérez",
                ApellidoMaterno = "Gómez",
                Email = "juan.perez@vemo.com",
                Password = "Password#123",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };

            
            // Simular que la creación del usuario falla
            _userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<Usuarios>(), usuarioCreacion.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "El correo ya está en uso." }));


            // Act
            var result = await _controller.Registro(usuarioCreacion) as ObjectResult;

            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }
    }
}