using Backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar DBContext e Identity
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => // Usando IdentityUser e IdentityRole padrão
{
    options.SignIn.RequireConfirmedAccount = false; // Mantenha false se não for implementar confirmação de email agora
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false; // Ajuste conforme sua política
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders(); // Para gerar tokens de reset de senha, etc.

// 2. Configurar Autenticação JWT
var jwtSection = builder.Configuration.GetSection("JWT");
var jwtSecret = jwtSection["Secret"] ?? throw new InvalidOperationException("JWT:Secret não configurado.");
var jwtIssuer = jwtSection["ValidIssuer"] ?? throw new InvalidOperationException("JWT:ValidIssuer não configurado.");
var jwtAudience = jwtSection["ValidAudience"] ?? throw new InvalidOperationException("JWT:ValidAudience não configurado.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = builder.Environment.IsProduction(); // True em produção
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddControllers();

// Configuração do CORS (essencial para desenvolvimento com front e back em portas diferentes)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policyBuilder =>
    {
        policyBuilder.WithOrigins(jwtAudience) // URL do seu app React (Vite default: http://localhost:5173)
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

// Swagger (opcional, mas útil)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // Para ver erros detalhados no dev
}
else
{
    app.UseExceptionHandler("/Error"); // Criar uma página/endpoint de erro genérico
    app.UseHsts(); // Forçar HTTPS em produção
}

// app.UseHttpsRedirection(); // Descomente se for usar HTTPS em produção e o proxy reverso não cuidar disso

// IMPORTANTE: O build do React deve ser copiado para wwwroot ou um caminho configurado aqui.
// Se o build do React estiver em Frontend/dist, e você copiar para wwwroot/
app.UseStaticFiles("/src/frontend/dist"); // Serve arquivos da wwwroot (onde o index.html do React e seus assets estarão)

app.UseRouting();

app.UseCors("AllowReactApp"); // Aplicar a política CORS ANTES de Authentication/Authorization

app.UseAuthentication(); // Essencial para JWT funcionar
app.UseAuthorization();

app.MapControllers(); // Mapeia seus AuthController e outros

// Fallback para o index.html do React para que o roteamento do lado do cliente funcione
// Isso garante que se o usuário atualizar uma página /dashboard, o index.html seja servido
// e o React Router possa assumir o controle.
app.MapFallbackToFile("index.html"); // Assumindo que index.html está na raiz da wwwroot

app.Run();