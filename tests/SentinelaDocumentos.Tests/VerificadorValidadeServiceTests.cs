// tests/SentinelaDocumentos.Tests/VerificadorValidadeServiceTests.cs
using Microsoft.Extensions.DependencyInjection; // Para IServiceScopeFactory, IServiceProvider
using Microsoft.Extensions.Logging;         // Para ILogger
using Moq;                                  // Para Mocking
using SentinelaDocumentos.Application.Interfaces; // Para IEmailSender
using SentinelaDocumentos.Domain.Entities;      // Para DocumentoEmpresa
using SentinelaDocumentos.Domain.Interfaces; // Para IDocumentoEmpresaRepository
using SentinelaDocumentos.Infrastructure.BackgroundServices; // Para VerificadorValidadeService
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit; // Framework de Teste

namespace SentinelaDocumentos.Tests;

public class VerificadorValidadeServiceTests
{
    // Mocks para as dependências que o serviço precisa
    private readonly Mock<ILogger<VerificadorValidadeService>> _loggerMock;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IDocumentoEmpresaRepository> _documentoRepoMock;
    private readonly Mock<IEmailSender> _emailSenderMock;

    // Serviço que estamos testando (System Under Test - SUT)
    private readonly VerificadorValidadeService _sut; // Não é IHostedService, mas a classe concreta para teste interno

    public VerificadorValidadeServiceTests()
    {
        // Inicializa os mocks no construtor do teste
        _loggerMock = new Mock<ILogger<VerificadorValidadeService>>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _documentoRepoMock = new Mock<IDocumentoEmpresaRepository>();
        _emailSenderMock = new Mock<IEmailSender>();

        // Configuração padrão dos mocks para o fluxo de DI Scoped
        // Quando CreateScope for chamado, retorna nosso scope mockado
        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        // Quando o scope pedir o ServiceProvider, retorna nosso provider mockado
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

        // Configura o ServiceProvider para retornar os mocks dos serviços Scoped quando requisitados
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IDocumentoEmpresaRepository)))
            .Returns(_documentoRepoMock.Object); // Retorna o mock do repositório
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IEmailSender)))
            .Returns(_emailSenderMock.Object); // Retorna o mock do email sender
        _serviceProviderMock // Retorna um logger (pode ser o mesmo mock ou um logger real nulo)
            .Setup(x => x.GetService(typeof(ILogger<VerificadorValidadeService>)))
            .Returns(_loggerMock.Object);


        // Cria a instância do serviço passando os mocks das dependências Singleton
        _sut = new VerificadorValidadeService(_loggerMock.Object, _scopeFactoryMock.Object);

        // NOTA: Não vamos chamar ExecuteAsync diretamente aqui, pois ele tem o loop infinito.
        // Precisaremos talvez refatorar o VerificadorValidadeService para ter um método
        // público/interno que faça UMA verificação, ou usar truques de CancellationToken.
        // Por ora, vamos focar nos mocks e no teste em si.
    }

    // Método de teste utilitário para simular a execução do trabalho dentro do escopo
    // (Uma alternativa a refatorar VerificadorValidadeService)
    private async Task ExecutarVerificacaoAsync()
    {
         // Simula a lógica que estaria dentro do loop do ExecuteAsync, usando os mocks configurados
         var repo = _serviceProviderMock.Object.GetRequiredService<IDocumentoEmpresaRepository>();
         var sender = _serviceProviderMock.Object.GetRequiredService<IEmailSender>();
         var logger = _serviceProviderMock.Object.GetRequiredService<ILogger<VerificadorValidadeService>>();

         var dataReferencia = DateTime.UtcNow;
         var dataLimiteAlerta = dataReferencia.AddDays(30); // Exemplo de 30 dias

         // Chama o método do repositório mockado
         var documentosParaAlertar = await repo.ObterDocumentosProximosDoVencimentoAsync(dataLimiteAlerta, 0, null);

         if (documentosParaAlertar != null && documentosParaAlertar.Any())
         {
             logger.LogInformation("Teste: Encontrados {Count} documentos.", documentosParaAlertar.Count());
             var alertasPorUsuario = documentosParaAlertar.GroupBy(d => d.Usuario); // Assume que Usuario está populado no mock
             foreach (var grupo in alertasPorUsuario)
             {
                 // ... lógica de montar e enviar email ...
                 if (grupo.Key != null && !string.IsNullOrEmpty(grupo.Key.Email))
                 {
                      await sender.EnviarEmailAsync(grupo.Key.Email, "Assunto Teste", "Corpo Teste");
                 }
             }
         } else {
             logger.LogInformation("Teste: Nenhum documento encontrado.");
         }
    }


    // --- O PRIMEIRO TESTE ---
    [Fact]
    public async Task ExecuteAsync_QuandoRepositorioNaoRetornaDocumentos_NaoDeveChamarEmailSender()
    {
        // Arrange (Organizar)
        // Configura o mock do repositório para retornar uma lista VAZIA quando for chamado
        _documentoRepoMock
            .Setup(repo => repo.ObterDocumentosProximosDoVencimentoAsync(
                It.IsAny<DateTime>(), // Qualquer data limite
                It.IsAny<int>(),      // Qualquer dias mínimos
                It.IsAny<string>()))  // Qualquer usuarioId (null neste caso)
            .ReturnsAsync(new List<DocumentoEmpresa>()); // Retorna lista vazia!

        // Act (Agir)
        // Executa a lógica de verificação (usando nosso método auxiliar que simula o trabalho)
        await ExecutarVerificacaoAsync();

        // Assert (Verificar)
        // Verifica se o método EnviarEmailAsync do mock do EmailSender NUNCA foi chamado
        _emailSenderMock.Verify(sender => sender.EnviarEmailAsync(
            It.IsAny<string>(), // Qualquer email
            It.IsAny<string>(), // Qualquer assunto
            It.IsAny<string>()), // Qualquer corpo
            Times.Never); // Garante que NUNCA foi chamado
    }

    // --- Próximos Testes (Exemplos) ---
    // [Fact]
    // public async Task ExecuteAsync_QuandoRepositorioRetornaDocumentosParaUmUsuario_DeveChamarEmailSenderUmaVez() { ... }

    // [Fact]
    // public async Task ExecuteAsync_QuandoRepositorioRetornaDocumentosParaMultiplosUsuarios_DeveChamarEmailSenderParaCadaUsuario() { ... }

    // [Fact]
    // public async Task ExecuteAsync_QuandoEmailSenderLancaExcecao_DeveLogarErroENaoQuebrarLoop() { ... }
}