using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SentinelaDocumentos.Application.DTOs.Documento;
using SentinelaDocumentos.Application.Interfaces;
using SentinelaDocumentos.Domain.Entities;
using SentinelaDocumentos.Domain.Interfaces;
using SentinelaDocumentos.Application.Services.Utils;

namespace SentinelaDocumentos.Application.Services
{
    public class DocumentoAppService : IDocumentoAppService
    {
        private readonly IDocumentoEmpresaRepository _docRepo;
        private readonly ITipoDocumentoRepository _tipoRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public DocumentoAppService(IDocumentoEmpresaRepository docRepo, ITipoDocumentoRepository tipoRepo, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _docRepo = docRepo;
            _tipoRepo = tipoRepo;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<DocumentoDto> AdicionarDocumentoAsync(CriarDocumentoDto dto, string usuarioId)
        {
            // Validação do DTO e verificação do TipoDocumentoId
            var tipoDocumento = await _tipoRepo.ObterPorIdAsync(dto.TipoDocumentoId);
            if (tipoDocumento == null)
                throw new Exception("Tipo de documento inválido.");

            // Criação da entidade DocumentoEmpresa
            var documento = _mapper.Map<DocumentoEmpresa>(dto);
            documento.ApplicationUserId = usuarioId;

            // Adicionar ao repositório
            await _docRepo.AdicionarAsync(documento);

            // Mapear entidade salva para DTO e retornar
            return _mapper.Map<DocumentoDto>(documento);
        }

        public async Task<IEnumerable<DocumentoDto>> ListarDocumentosAsync(string usuarioId, int page, int pageSize)
        {
            var documentos = await _docRepo.ListarPorUsuarioAsync(usuarioId);
            // Paginação manual
            documentos = documentos.Skip((page - 1) * pageSize).Take(pageSize);

            var dtos = _mapper.Map<IEnumerable<DocumentoDto>>(documentos);

            foreach (var dto in dtos)
            {
                dto.DiasParaVencer = (dto.DataValidade - DateTime.UtcNow).Days;
                dto.Status = DocumentoUtils.CalcularStatusDocumento(dto.DataValidade);
            }

            return dtos;
        }

        public async Task<DocumentoDto> ObterDetalhesDocumentoAsync(long id, string usuarioId)
        {
            var documento = await _docRepo.ObterPorIdEUsuarioAsync(id, usuarioId);
            if (documento == null)
                throw new Exception("Documento não encontrado.");

            var dto = _mapper.Map<DocumentoDto>(documento);
            dto.DiasParaVencer = (dto.DataValidade - DateTime.UtcNow).Days;
            dto.Status = DocumentoUtils.CalcularStatusDocumento(dto.DataValidade);

            return dto;
        }

        public async Task AtualizarDocumentoAsync(AtualizarDocumentoDto dto, string usuarioId)
        {
            var documento = await _docRepo.ObterPorIdEUsuarioAsync(dto.Id, usuarioId);
            if (documento == null)
                throw new Exception("Documento não encontrado ou não pertence ao usuário.");

            _mapper.Map(dto, documento);
            await _docRepo.AtualizarAsync(documento);
        }

        public async Task DesativarDocumentoAsync(long id, string usuarioId)
        {
            var documento = await _docRepo.ObterPorIdEUsuarioAsync(id, usuarioId);
            if (documento == null)
                throw new Exception("Documento não encontrado ou não pertence ao usuário.");

            await _docRepo.DesativarAsync(documento);
        }

        Task<DocumentoDto> IDocumentoAppService.AdicionarDocumentoAsync(CriarDocumentoDto dto, string usuarioId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<DocumentoDto>> IDocumentoAppService.ListarDocumentosAsync(string usuarioId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        Task<DocumentoDto> IDocumentoAppService.ObterDetalhesDocumentoAsync(long id, string usuarioId)
        {
            throw new NotImplementedException();
        }

        Task IDocumentoAppService.AtualizarDocumentoAsync(AtualizarDocumentoDto dto, string usuarioId)
        {
            throw new NotImplementedException();
        }

        Task IDocumentoAppService.DesativarDocumentoAsync(long id, string usuarioId)
        {
            throw new NotImplementedException();
        }

        Task IDocumentoAppService.AdicionarDocumentoAsync(DocumentoDto documentoDto, string? userId)
        {
            throw new NotImplementedException();
        }
    }
}