using SentinelaDocumentos.Application.Interfaces;
using SentinelaDocumentos.Application.Services;
using SentinelaDocumentos.Domain.Interfaces;
using SentinelaDocumentos.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using SentinelaDocumentos.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using SentinelaDocumentos.Domain.Entities;
using SentinelaDocumentos.Application.Services.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITipoDocumentoRepository, EfTipoDocumentoRepository>();
builder.Services.AddScoped<IDocumentoEmpresaRepository, EfDocumentoEmpresaRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDocumentoAppService, DocumentoAppService>();

// Adicionando o AutoMapper ao contêiner de serviços
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Configurando o ApplicationDbContext para usar SQLite no ambiente de desenvolvimento
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurando o Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Adicionando os serviços de controladores
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configurando os endpoints principais da API
app.MapControllers();

app.Run();
