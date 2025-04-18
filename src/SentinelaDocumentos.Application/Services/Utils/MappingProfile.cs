using AutoMapper;
using SentinelaDocumentos.Application.DTOs.Documento;
using SentinelaDocumentos.Domain.Entities;

namespace SentinelaDocumentos.Application.Services.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CriarDocumentoDto, DocumentoEmpresa>()
                .ForMember(dest => dest.TipoDocumentoId, opt => opt.MapFrom(src => src.TipoDocumentoId));

            CreateMap<AtualizarDocumentoDto, DocumentoEmpresa>()
                .ForMember(dest => dest.TipoDocumentoId, opt => opt.MapFrom(src => src.TipoDocumentoId));
                
            CreateMap<DocumentoEmpresa, DocumentoDto>()
                .ForMember(dest => dest.NomeTipoDocumento, 
                    opt => opt.MapFrom(src => src.TipoDocumento != null ? src.TipoDocumento.Nome : string.Empty));
                    
            CreateMap<TipoDocumento, TipoDocumentoDto>();
        }
    }
}