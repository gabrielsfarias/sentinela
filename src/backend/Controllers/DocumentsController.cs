using System.Security.Claims;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Authorize] // Todos os endpoints aqui requerem autenticação
[ApiController]
[Route("documentos")] // Define o prefixo da rota para este controller
public class DocumentsController(
    ApplicationDbContext context,
    UserManager<IdentityUser> userManager,
    ILogger<DocumentsController> logger
) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly ILogger<DocumentsController> _logger = logger;

    private string? GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    // GET: /documentos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(); // Deveria ser pego pelo [Authorize], mas uma checagem extra
        }

        var documents = await _context
            .Documents.Where(d => d.UserId == userId)
            .OrderByDescending(d => d.UpdatedAt) // Ou CreatedAt, ou por ExpiryDate
            .Select(d => new DocumentDto // Mapear para DTO
            {
                Id = d.Id,
                OriginalFileName = d.OriginalFileName,
                OriginalFileSize = d.OriginalFileSize,
                OriginalFileLastModified = d.OriginalFileLastModified,
                DisplayName = d.DisplayName,
                ExpiryDate = d.ExpiryDate,
                Notes = d.Notes,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
            })
            .ToListAsync();

        return Ok(documents);
    }

    // POST: /documentos
    // Este endpoint aceitará uma lista de metadados de documentos para criação em lote.
    [HttpPost]
    public async Task<ActionResult<List<DocumentDto>>> CreateDocuments(
        [FromBody] List<CreateDocumentDto> createDtos
    )
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        if (createDtos == null || !createDtos.Any())
        {
            return BadRequest("Nenhum dado de documento fornecido.");
        }

        var createdDocuments = new List<Document>();
        var now = DateTime.UtcNow;

        foreach (var dto in createDtos)
        {
            // Validação adicional do DTO se necessário (embora DataAnnotations já façam parte)
            if (string.IsNullOrWhiteSpace(dto.OriginalFileName))
            {
                // Pode optar por pular este ou retornar um erro para o lote inteiro.
                // Por simplicidade, vamos pular se o nome do arquivo for inválido.
                _logger.LogWarning(
                    "Documento sem OriginalFileName no lote para o usuário {UserId}. Pulando.",
                    userId
                );
                continue;
            }

            var document = new Document
            {
                UserId = userId,
                OriginalFileName = dto.OriginalFileName,
                OriginalFileSize = dto.OriginalFileSize,
                OriginalFileLastModified = dto.OriginalFileLastModified,
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName)
                    ? dto.OriginalFileName
                    : dto.DisplayName,
                ExpiryDate = dto.ExpiryDate,
                Notes = dto.Notes,
                CreatedAt = now,
                UpdatedAt = now,
            };
            createdDocuments.Add(document);
        }

        if (createdDocuments.Count == 0)
        {
            return BadRequest("Nenhum documento válido para criação no lote.");
        }

        _context.Documents.AddRange(createdDocuments);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "{Count} documentos criados para o usuário {UserId}.",
            createdDocuments.Count,
            userId
        );

        // Mapear para DTOs para a resposta
        var resultDtos = createdDocuments
            .Select(d => new DocumentDto
            {
                Id = d.Id,
                OriginalFileName = d.OriginalFileName,
                OriginalFileSize = d.OriginalFileSize,
                OriginalFileLastModified = d.OriginalFileLastModified,
                DisplayName = d.DisplayName,
                ExpiryDate = d.ExpiryDate,
                Notes = d.Notes,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
            })
            .ToList();

        // Retorna 201 Created com a localização do primeiro recurso criado (opcional)
        // ou simplesmente 200 OK com a lista de DTOs.
        // Para simplicidade com lote, 200 OK é mais fácil.
        return Ok(resultDtos);
    }

    // Outros endpoints (PATCH, DELETE) virão depois.
}
