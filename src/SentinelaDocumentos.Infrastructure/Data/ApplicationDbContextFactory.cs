using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SentinelaDocumentos.Infrastructure.Data;

// Esta classe permite que as ferramentas do EF Core (ex: para migrations)
// criem uma instância do DbContext em tempo de design.
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // --- IMPORTANTE: Configurar o caminho para ler appsettings.json ---
        // Isso assume que o comando 'dotnet ef' será executado a partir da pasta do projeto Api
        // ou da raiz da solução, e busca o appsettings.json do projeto Api.
        // Ajuste o caminho relativo se sua estrutura for diferente ou se executar de outro local.
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../SentinelaDocumentos.Api");
        // Se rodar da raiz da solução: var basePath = Path.Combine(Directory.GetCurrentDirectory(), "src/SentinelaDocumentos.Api");

         // Se estiver rodando o comando de dentro da pasta Api:
        if (!Directory.Exists(basePath))
        {
            basePath = Directory.GetCurrentDirectory(); // Assume que estamos na pasta Api
        }


        Console.WriteLine($"Usando base path para appsettings: {basePath}"); // Log para debug

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath) // Define o diretório base para encontrar o appsettings
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true) // Tenta carregar o de Dev
            .AddJsonFile("appsettings.json", optional: true) // Carrega o base se o de Dev não existir
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback se não encontrar connection string (ou defina um valor padrão aqui)
            connectionString = "Data Source=sentinela_design.db"; // Um fallback para design time
             Console.WriteLine($"AVISO: ConnectionString 'DefaultConnection' não encontrada. Usando fallback: {connectionString}");
        }
         else
        {
             Console.WriteLine($"ConnectionString encontrada: {connectionString}");
        }


        optionsBuilder.UseSqlite(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}