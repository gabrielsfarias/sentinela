using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SentinelaDocumentos.Application.DTOs.Documento;
using SentinelaDocumentos.Application.Interfaces;
using SentinelaDocumentos.Domain.Entities;
using System.Threading.Tasks;

namespace SentinelaDocumentos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentoAppService _documentoAppService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentosController(IDocumentoAppService documentoAppService, UserManager<ApplicationUser> userManager)
        {
            _documentoAppService = documentoAppService;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            var userId = _userManager.GetUserId(User);
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
            var result = await _documentoAppService.AdicionarDocumentoAsync(documentoDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            var result = await _documentoAppService.ListarDocumentosAsync(userId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var userId = GetUserId();
            var result = await _documentoAppService.ObterDetalhesDocumentoAsync(id, userId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] AtualizarDocumentoDto documentoDto)
        {
            var userId = GetUserId();
            await _documentoAppService.AtualizarDocumentoAsync(documentoDto, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = GetUserId();
            await _documentoAppService.DesativarDocumentoAsync(id, userId);
            return NoContent();
        }
    }
}