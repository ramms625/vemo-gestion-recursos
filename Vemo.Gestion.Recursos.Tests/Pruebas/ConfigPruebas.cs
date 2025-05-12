using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Vemo.Gestion.Recursos.Data;
using Vemo.Gestion.Recursos.Data.Entidades;
using Vemo.Gestion.Recursos.Helpers;

namespace Vemo.Gestion.Recursos.Tests.Pruebas
{
    public class ConfigPruebas
    {
        protected ApplicationDbContext GetContext(string nombreDB)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: nombreDB).Options;
            return new ApplicationDbContext(options);
        }



        protected IMapper GetMapper()
        {
            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutomapperProfile());
            });

            return config.CreateMapper();
        }



        protected Mock<UserManager<Usuarios>> GetUserManager()
        {
            return new Mock<UserManager<Usuarios>>(
                new Mock<IUserStore<Usuarios>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<Usuarios>>().Object,
                new IUserValidator<Usuarios>[0],
                new IPasswordValidator<Usuarios>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<Usuarios>>>().Object);
        }



        protected Mock<SignInManager<Usuarios>> GetSignInManager()
        {
            var userManagerMock = GetUserManager();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<Usuarios>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var loggerMock = new Mock<ILogger<SignInManager<Usuarios>>>();
            var authenticationSchemeProviderMock = new Mock<IAuthenticationSchemeProvider>();
            var userConfirmationMock = new Mock<IUserConfirmation<Usuarios>>();

            return new Mock<SignInManager<Usuarios>>(
                userManagerMock.Object,
                httpContextAccessorMock.Object,
                userClaimsPrincipalFactoryMock.Object,
                optionsMock.Object,
                loggerMock.Object,
                authenticationSchemeProviderMock.Object,
                userConfirmationMock.Object
            );
        }



        protected Mock<RoleManager<IdentityRole>> GetRoleManager()
        {
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();

            var roleValidators = new List<IRoleValidator<IdentityRole>>()
            {
                new RoleValidator<IdentityRole>()
            };

            var lookupNormalizerMock = new Mock<ILookupNormalizer>();

            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            var loggerMock = new Mock<ILogger<RoleManager<IdentityRole>>>();

            var roleManager = new RoleManager<IdentityRole>(
                roleStoreMock.Object,
                roleValidators,
                lookupNormalizerMock.Object,
                identityErrorDescriberMock.Object,
                loggerMock.Object);

            return new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(),
                null,
                null,
                null,
                null);
        }



        protected Mock<IConfiguration> GetMockConfiguration()
        {
            return new Mock<IConfiguration>();
        }


        protected IConfiguration GetConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return configurationBuilder.Build();
        }
    }
}