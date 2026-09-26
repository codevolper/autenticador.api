using System.Text;
using Autenticador.API.Application.CasosUso;
using Autenticador.API.Domain.Interfaces;
using Autenticador.API.Infrastructure.Persistencia;
using Autenticador.API.Infrastructure.Seguranca;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Serviços de aplicação e infraestrutura
builder.Services.AddSingleton<BancoMemoria>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioDapper>();
builder.Services.AddScoped<ServicoHashDeSenha>();
builder.Services.AddScoped<GeradorJwt>();
builder.Services.AddScoped<CasoDeUsoCadastrarUsuario>();
builder.Services.AddScoped<CasoDeUsoAutenticarUsuario>();

// Autenticação JWT
var chave = builder.Configuration["Jwt:ChaveSecreta"] ?? "troque-esta-chave-por-uma-muito-segura";
var chaveBytes = Encoding.UTF8.GetBytes(chave);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Emissor"],
        ValidAudience = builder.Configuration["Jwt:Publico"],
        IssuerSigningKey = new SymmetricSecurityKey(chaveBytes)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{

    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(s => { 
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Autenticador API V1");
    s.RoutePrefix = string.Empty;  
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
