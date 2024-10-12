using DocumentsREST.DAL.Models;

namespace DocumentsREST.DAL.Repositories;

public interface IDocumentRepository
{
    Task<Document?> GetDocumentByIdAsync(long id);
    Task<List<Document?>> GetAllDocumentsAsync();
    Task AddDocumentAsync(Document? document);
    Task UpdateDocumentAsync(Document document);
    Task DeleteDocumentAsync(long id);
}