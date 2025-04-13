using SentinelaDocumentos.Application.Interfaces;
using SentinelaDocumentos.Application.Services;
using SentinelaDocumentos.Domain.Interfaces;
using SentinelaDocumentos.Infrastructure.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITipoDocumentoRepository, EfTipoDocumentoRepository>();
builder.Services.AddScoped<IDocumentoEmpresaRepository, EfDocumentoEmpresaRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDocumentoAppService, DocumentoAppService>();

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
