using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SentinelaDocumentos.Application.DTOs.Documento;
using SentinelaDocumentos.Application.Interfaces;
using SentinelaDocumentos.Domain.Entities;

namespace SentinelaDocumentos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentosController(IDocumentoAppService documentoAppService, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private string GetUserId()
        {
            var userId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }
            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriarDocumentoDto documentoDto)
        {
            var userId = GetUserId();
            var result = await documentoAppService.AdicionarDocumentoAsync(documentoDto, userId);
            var createdDocument = result.FirstOrDefault();
            if (createdDocument == null)
            {
                return BadRequest("Documento não pôde ser criado.");
            }
            return CreatedAtAction(nameof(GetById), new { id = createdDocument.Id }, createdDocument);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            var result = await documentoAppService.ListarDocumentosAsync(userId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var userId = GetUserId();
            var result = await documentoAppService.ObterDetalhesDocumentoAsync(id, userId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] AtualizarDocumentoDto documentoDto)
        {
            var userId = GetUserId();
            await documentoAppService.AtualizarDocumentoAsync(documentoDto, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = GetUserId();
            await documentoAppService.DesativarDocumentoAsync(id, userId);
            return NoContent();
        }
    }
}