using CineAPI.Controllers;
using CineAPI.Datos;
using CineAPI.Datos.ADO.NET;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApiAutores.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConexion,ConexionDB>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivoLocal>();

builder.Services.AddScoped<ConexionSQL>(x =>
{
    var config = x.GetRequiredService<IConfiguration>();

    //esta condicion es para hacer la conexion con la bd de mi pc de escritorio 
    string nombrePC = Environment.MachineName;

    if(nombrePC == "DESKTOP-A3FCCPG")
    {
        return new ConexionSQL(config.GetConnectionString("defaultConnection2"));
    }

    return new ConexionSQL(config.GetConnectionString("defaultConnection"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"])),
    ClockSkew = TimeSpan.Zero
});

builder.Services.AddSwaggerGen(c =>
{
    //configuracion para usar JWT con swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("ADMIN", politica => politica.RequireClaim("rol", "ADMIN"));
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
