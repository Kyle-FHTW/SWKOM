using DocumentsREST.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentsREST.DAL.Repositories;

public class DocumentRepository(AppDbContext context) : IDocumentRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Document?> GetDocumentByIdAsync(long id)
    {
        return await _context.Documents.FindAsync(id);
    }

    public async Task<List<Document?>> GetAllDocumentsAsync()
    {
        return await _context.Documents.ToListAsync();
    }

    public async Task AddDocumentAsync(Document? document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(long id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document != null)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}