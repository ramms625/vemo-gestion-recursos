using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Vemo.Gestion.Recursos.Data;
using Vemo.Gestion.Recursos.Data.DataInicial;
using Vemo.Gestion.Recursos.Data.Entidades;
using Vemo.Gestion.Recursos.Data.Models;
using Vemo.Gestion.Recursos.Helpers;


var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var services = builder.Services;


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .ConfigureApiBehaviorOptions(o =>
    {
        o.InvalidModelStateResponseFactory = (errorContext) =>
        {
            var errors = new List<string>();
            var fields = errorContext.ModelState.Values.ToList();

            foreach (var field in fields)
            {
                foreach (var error in field.Errors)
                    errors.Add($"{error.ErrorMessage}");
            }

            var result = new ApiResponse<List<string>>(errors, "Campo(s) incorrecto(s).");
            return new BadRequestObjectResult(result);
        };
    });


services
    .AddSwaggerGen(o =>
    {
        o.SwaggerDoc("v1", new OpenApiInfo { Title = "Vemo - gestión de recursos", Version = "v1" });

        o.EnableAnnotations();

        o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        });

        o.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[]{}
            }
        });
    });


services
    .AddDbContext<ApplicationDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? builder.Configuration["ConnectionStrings__DefaultConnection"]; //config.GetConnectionString("ConnectionString");
        options.UseSqlServer(connectionString);
    });


services
    .AddCors(options =>
    {
        options.AddPolicy("DefaultCors", policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
    });


services
    .AddIdentity<Usuarios, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<Usuarios>>(TokenOptions.DefaultProvider);


services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Encryption:TokenKey"]!)),
        ClockSkew = TimeSpan.Zero
    });



services.AddDataProtection();

services.AddEndpointsApiExplorer();

services.AddHttpContextAccessor();

services.AddAutoMapper(typeof(Program));

services.AddScoped<IDataInicial, DataInicial>();






var app = builder.Build();

InsercionDataInicial(app);

app.UseSwagger();

app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "Vemo - gestión de recursos"));

app.UseStaticFiles();

app.UseCors("DefaultCors");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



void InsercionDataInicial(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var insercion = scope.ServiceProvider.GetRequiredService<IDataInicial>();
        insercion.InsercionDatos().GetAwaiter().GetResult();
    }
}