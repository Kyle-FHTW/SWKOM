using DocumentsREST.DAL.Models;
using DocumentsREST.DAL.Repositories;
using log4net; // Import log4net
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentsREST.BL.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private static readonly ILog Log = LogManager.GetLogger(typeof(DocumentService)); // Initialize logger

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<Document> GetDocumentByIdAsync(long id)
        {
            Log.Info($"Fetching document with ID: {id}");
            var document = await _documentRepository.GetDocumentByIdAsync(id);

            if (document == null)
            {
                Log.Warn($"Document with ID: {id} not found.");
            }
            else
            {
                Log.Info($"Document with ID: {id} retrieved successfully.");
            }

            return document;
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            Log.Info("Fetching all documents.");
            var documents = await _documentRepository.GetAllDocumentsAsync();
            Log.Info($"Retrieved {documents.Count()} documents.");
            return documents;
        }

        public async Task<Document> AddDocumentAsync(Document document)
        {
            Log.Info($"Adding document: {document.Title}");

            await _documentRepository.AddDocumentAsync(document);
            Log.Info($"Document added successfully: {document.Id} - {document.Title}");
            return document;  // Return the document with its generated ID.
        }

        public async Task<bool> UpdateDocumentAsync(Document document)
        {
            Log.Info($"Updating document with ID: {document.Id}");

            var existingDocument = await _documentRepository.GetDocumentByIdAsync(document.Id);
            if (existingDocument == null)
            {
                Log.Warn($"Document with ID: {document.Id} not found for update.");
                return false;  // Document does not exist.
            }

            // Update fields
            existingDocument.Title = document.Title;
            existingDocument.Description = document.Description;
            existingDocument.Metadata = document.Metadata;

            await _documentRepository.UpdateDocumentAsync(existingDocument);
            Log.Info($"Document with ID: {document.Id} updated successfully.");
            return true;  // Indicate the update was successful.
        }

        public async Task DeleteDocumentAsync(long id)
        {
            Log.Info($"Deleting document with ID: {id}");

            var document = await _documentRepository.GetDocumentByIdAsync(id);
            if (document == null)
            {
                Log.Warn($"Document with ID: {id} not found for deletion.");
                return; // Document not found, exit without action
            }

            await _documentRepository.DeleteDocumentAsync(id);
            Log.Info($"Document with ID: {id} deleted successfully.");
        }
    }
}
