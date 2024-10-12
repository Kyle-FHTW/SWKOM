using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentsREST.BL.Services;
using DocumentsREST.BL.DTOs;
using DocumentsREST.DAL.Models;
using log4net; // Import log4net

namespace DocumentsREST.Controllers
{
    [Route("/")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;  // Injected service to interact with DB
        private readonly IMapper _mapper;  // AutoMapper for mapping Document <-> DocumentDto
        private static readonly ILog Log = LogManager.GetLogger(typeof(DocumentsController)); // Initialize logger

        public DocumentsController(IDocumentService documentService, IMapper mapper)
        {
            _documentService = documentService;
            _mapper = mapper;
        }

        [HttpGet("documents")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> Get()
        {
            Log.Info("Fetching all documents.");
            var documents = await _documentService.GetAllDocumentsAsync();
            Log.Info($"Retrieved {documents.Count()} documents.");
            return Ok(documents);
        }

        [HttpGet("documents/{id}")]
        public async Task<ActionResult<DocumentDto>> Get(long id)
        {
            Log.Info($"Fetching document with ID: {id}");
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
            {
                Log.Warn($"Document with ID: {id} not found.");
                return NotFound();
            }
            Log.Info($"Document with ID: {id} retrieved successfully.");
            return Ok(document);
        }

        [HttpPost("documents")]
        public async Task<ActionResult<DocumentDto>> Post([FromBody] DocumentDto documentDto)
        {
            Log.Info("Adding a new document.");

            if (!ModelState.IsValid)
            {
                Log.Warn("Invalid model state for document.");
                return BadRequest(ModelState);
            }

            // Map DTO to entity
            var documentEntity = _mapper.Map<Document>(documentDto);

            // Call service to add the document
            var createdDocument = await _documentService.AddDocumentAsync(documentEntity);

            // Map the created entity to DTO (with ID)
            var createdDocumentDto = _mapper.Map<DocumentDto>(createdDocument);

            Log.Info($"Document with ID: {createdDocumentDto.Id} created successfully.");
            return CreatedAtAction(nameof(Get), new { id = createdDocumentDto.Id }, createdDocumentDto);
        }

        [HttpPost("documents/upload")]
        public async Task<IActionResult> UploadDocument([FromForm] string description, [FromForm] IFormFile file)
        {
            Log.Info("UploadDocument called");

            if (file == null || file.Length == 0)
            {
                Log.Warn("File is required for upload.");
                return BadRequest("File is required.");
            }

            // Set your max file size limit (e.g., 10 MB)
            const long maxFileSize = 20 * 1024 * 1024; // 10 MB

            if (file.Length > maxFileSize)
            {
                Log.Warn($"File size {file.Length} exceeds the maximum limit of {maxFileSize} bytes.");
                return BadRequest("File size exceeds the maximum limit of 10 MB.");
            }

            // Automatically use the original filename
            var fileName = file.FileName;

            // Automatically use the original filename (without extension) as the title
            var title = Path.GetFileNameWithoutExtension(file.FileName);

            // Create a new document entity and set the title from the filename
            var documentEntity = new Document
            {
                Title = title,  // Automatically set title from file name
                Metadata = "Example Metadata",  // Placeholder for metadata
                Description = description
            };

            // Add the document to the database using the service
            var createdDocument = await _documentService.AddDocumentAsync(documentEntity);

            Log.Info($"Document with ID: {createdDocument.Id} uploaded successfully.");
            // Return a response with the newly created document
            return CreatedAtAction(nameof(Get), new { id = createdDocument.Id }, createdDocument);
        }


        [HttpPut("documents/{id}")]
        public async Task<ActionResult> Put(long id, [FromBody] DocumentDto updatedDocumentDto)
        {
            Log.Info($"Updating document with ID: {id}");

            var documentEntity = _mapper.Map<Document>(updatedDocumentDto);
            documentEntity.Id = id;  // Ensure the ID is set from the route.

            var updateSuccess = await _documentService.UpdateDocumentAsync(documentEntity);
            if (!updateSuccess)
            {
                Log.Warn($"Document with ID: {id} not found for update.");
                return NotFound();  // If the document doesn't exist, return 404.
            }

            Log.Info($"Document with ID: {id} updated successfully.");
            return Ok();
        }

        [HttpDelete("documents/{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            Log.Info($"Deleting document with ID: {id}");

            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
            {
                Log.Warn($"Document with ID: {id} not found for deletion.");
                return NotFound();
            }

            await _documentService.DeleteDocumentAsync(id);
            Log.Info($"Document with ID: {id} deleted successfully.");
            return Ok();
        }
    }
}
