using AutoMapper;
using DocumentsREST.BL.DTOs;
using DocumentsREST.DAL.Models;

namespace DocumentsREST.Mappings
{
    public class DocumentMappingProfile : Profile
    {
        public DocumentMappingProfile()
        {
            // Mapping from Document to DocumentDto (for GET requests, etc.)
            CreateMap<Document, DocumentDto>();

            // Mapping from DocumentDto to Document (for POST requests)
            // Ignore the Id field to let PostgreSQL auto-generate it
            CreateMap<DocumentDto, Document>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
