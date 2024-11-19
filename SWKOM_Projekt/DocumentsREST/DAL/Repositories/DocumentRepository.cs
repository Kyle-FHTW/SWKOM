#region

using DocumentsREST.DAL.Models;
using log4net;
using Microsoft.EntityFrameworkCore;
// Import log4net

#endregion

namespace DocumentsREST.DAL.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(DocumentRepository)); // Initialize logger
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetDocumentByIdAsync(long id)
    {
        Log.Info($"Fetching document with ID: {id}");
        var document = await _context.Documents.FindAsync(id);

        if (document == null)
            Log.Warn($"Document with ID: {id} not found.");
        else
            Log.Info($"Document with ID: {id} retrieved successfully.");

        return document;
    }

    public async Task<List<Document?>> GetAllDocumentsAsync()
    {
        Log.Info("Fetching all documents.");
        var documents = await _context.Documents.ToListAsync();
        Log.Info($"Retrieved {documents.Count} documents.");
        return documents;
    }

    public async Task AddDocumentAsync(Document? document)
    {
        Log.Info($"Adding document: {document?.Title}");

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        Log.Info($"Document added successfully: {document?.Id} - {document?.Title}");
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        Log.Info($"Updating document with ID: {document.Id}");

        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
        Log.Info($"Document with ID: {document.Id} updated successfully.");
    }

    public async Task DeleteDocumentAsync(long id)
    {
        Log.Info($"Deleting document with ID: {id}");

        var document = await _context.Documents.FindAsync(id);
        if (document == null)
        {
            Log.Warn($"Document with ID: {id} not found for deletion.");
            return; // Document not found, exit without action
        }

        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
        Log.Info($"Document with ID: {id} deleted successfully.");
    }
}