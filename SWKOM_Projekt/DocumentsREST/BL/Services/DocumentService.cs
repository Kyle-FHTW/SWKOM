#region

using AutoMapper;
using DocumentsREST.BL.DTOs;
using DocumentsREST.DAL.Models;
using DocumentsREST.DAL.Repositories;

#endregion

namespace DocumentsREST.BL.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public DocumentService(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentDto> GetDocumentByIdAsync(long id)
    {
        var document = await _documentRepository.GetDocumentByIdAsync(id);
        if (document == null) return null;

        // Use AutoMapper to map Document to DocumentDto
        return _mapper.Map<DocumentDto>(document);
    }

    public async Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync()
    {
        var documents = await _documentRepository.GetAllDocumentsAsync();

        // Use AutoMapper to map List<Document> to List<DocumentDto>
        return _mapper.Map<IEnumerable<DocumentDto>>(documents);
    }

    public async Task AddDocumentAsync(DocumentDto documentDto)
    {
        // Use AutoMapper to map DocumentDto to Document entity
        var document = _mapper.Map<Document>(documentDto);
        await _documentRepository.AddDocumentAsync(document);

        // After adding, update the DTO's Id with the generated Id
        documentDto.Id = document.Id;
    }

    public async Task UpdateDocumentAsync(long id, DocumentDto documentDto)
    {
        var document = await _documentRepository.GetDocumentByIdAsync(id);
        if (document != null)
        {
            // Use AutoMapper to map updated values from DocumentDto to Document
            _mapper.Map(documentDto, document);
            await _documentRepository.UpdateDocumentAsync(document);
        }
    }

    public async Task DeleteDocumentAsync(long id)
    {
        await _documentRepository.DeleteDocumentAsync(id);
    }
}
