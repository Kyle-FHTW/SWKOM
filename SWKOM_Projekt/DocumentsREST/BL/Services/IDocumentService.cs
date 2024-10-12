using DocumentsREST.BL.DTOs;
namespace DocumentsREST.BL.Services;

public interface IDocumentService
{
    Task<DocumentDto> GetDocumentByIdAsync(long id);
    Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync();
    Task AddDocumentAsync(DocumentDto documentDto);
    Task UpdateDocumentAsync(long id, DocumentDto documentDto);
    Task DeleteDocumentAsync(long id);
}