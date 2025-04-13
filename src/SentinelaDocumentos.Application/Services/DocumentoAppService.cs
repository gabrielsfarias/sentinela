using AutoMapper;
using SentinelaDocumentos.Application.DTOs.Documento;
using SentinelaDocumentos.Application.Interfaces;
using SentinelaDocumentos.Domain.Entities;
using SentinelaDocumentos.Domain.Interfaces;
using SentinelaDocumentos.Application.Services.Utils;

namespace SentinelaDocumentos.Application.Services
{
    public class DocumentoAppService(IDocumentoEmpresaRepository docRepo, ITipoDocumentoRepository tipoRepo, IMapper mapper) : IDocumentoAppService
    {
        public async Task<IEnumerable<DocumentoDto>> AdicionarDocumentoAsync(CriarDocumentoDto dto, string usuarioId)
        {
            // Validação do DTO e verificação do TipoDocumentoId diretamente
            if (await tipoRepo.ObterPorIdAsync(dto.TipoDocumentoId) == null)
                throw new Exception("Tipo de documento inválido.");

            // Criação da entidade DocumentoEmpresa
            var documento = mapper.Map<DocumentoEmpresa>(dto);
            documento.ApplicationUserId = usuarioId;

            // Adicionar ao repositório
            await docRepo.AdicionarAsync(documento);

            // Mapear entidade salva para DTO e retornar
            return (IEnumerable<DocumentoDto>)mapper.Map<DocumentoDto>(documento);
        }

        public async Task<IEnumerable<DocumentoDto>> ListarDocumentosAsync(string usuarioId, int page, int pageSize)
        {
            var documentos = await docRepo.ListarPorUsuarioAsync(usuarioId);
            documentos = documentos.Skip((page - 1) * pageSize).Take(pageSize);

            var dtos = mapper.Map<IEnumerable<DocumentoDto>>(documentos);

            foreach (var dto in dtos)
            {
                DocumentoUtils.CalcularDetalhes(dto);
            }

            return dtos;
        }

        public async Task<DocumentoDto> ObterDetalhesDocumentoAsync(long id, string usuarioId)
        {
            var documento = await docRepo.ObterPorIdEUsuarioAsync(id, usuarioId) 
                ?? throw new Exception("Documento não encontrado.");
            
            var dto = mapper.Map<DocumentoDto>(documento);
            DocumentoUtils.CalcularDetalhes(dto);

            return dto;
        }

        public async Task AtualizarDocumentoAsync(AtualizarDocumentoDto dto, string usuarioId)
        {
            var documento = await docRepo.ObterPorIdEUsuarioAsync(dto.Id, usuarioId) ?? throw new Exception("Documento não encontrado ou não pertence ao usuário.");
            mapper.Map(dto, documento);
            await docRepo.AtualizarAsync(documento);
        }

        public async Task DesativarDocumentoAsync(long id, string usuarioId)
        {
            var documento = await docRepo.ObterPorIdEUsuarioAsync(id, usuarioId) ?? throw new Exception("Documento não encontrado ou não pertence ao usuário.");
            await docRepo.DesativarAsync(documento);
        }

        Task<IEnumerable<DocumentoDto>> IDocumentoAppService.ListarDocumentosAsync(string usuarioId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<DocumentoDto>> IDocumentoAppService.ObterDetalhesDocumentoAsync(long id, string usuarioId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<DocumentoDto>> IDocumentoAppService.AtualizarDocumentoAsync(AtualizarDocumentoDto dto, string usuarioId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<DocumentoDto>> IDocumentoAppService.DesativarDocumentoAsync(long id, string usuarioId)
        {
            throw new NotImplementedException();
        }
    }
}