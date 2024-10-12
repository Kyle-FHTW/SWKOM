using DocumentsREST.BL.DTOs;
using DocumentsREST.DAL.Models;

namespace DocumentsREST.BL.Services;

public interface IDocumentService
{
    Task<Document> GetDocumentByIdAsync(long id);
    Task<IEnumerable<Document>> GetAllDocumentsAsync();
    Task<Document> AddDocumentAsync(Document document);
    Task<bool> UpdateDocumentAsync(Document document);
    Task DeleteDocumentAsync(long id);
}