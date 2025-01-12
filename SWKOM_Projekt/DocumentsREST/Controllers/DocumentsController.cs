#region

using System.Text;
using AutoMapper;
using DocumentsREST.BL.DTOs;
using DocumentsREST.BL.Services;
using DocumentsREST.DAL.Models;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DocumentsREST.Controllers;

[Route("/")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(DocumentsController));
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;
    private readonly IValidator<DocumentDto> _validator;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IMinioService _minioService;

    public DocumentsController(
        IDocumentService documentService,
        IMapper mapper,
        IValidator<DocumentDto> validator,
        IRabbitMqService rabbitMqService,
        IMinioService minioService)
    {
        _documentService = documentService;
        _mapper = mapper;
        _validator = validator;
        _rabbitMqService = rabbitMqService;
        _minioService = minioService;
    }

    [HttpGet("documents")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> Get()
    {
        var documents = await _documentService.GetAllDocumentsAsync();
        if (documents == null || !documents.Any())
        {
            return NotFound();
        }

        var documentDtos = _mapper.Map<IEnumerable<DocumentDto>>(documents);
        return Ok(documentDtos); // Ensure OkObjectResult is returned
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
        
        var documentDto = _mapper.Map<DocumentDto>(document);
        Log.Info($"Document with ID: {id} retrieved successfully.");
        return Ok(documentDto);
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

        const long maxFileSize = 20 * 1024 * 1024; // 20 MB
        if (file.Length > maxFileSize)
        {
            Log.Warn($"File size {file.Length} exceeds the maximum limit of {maxFileSize} bytes.");
            return BadRequest("File size exceeds the maximum limit of 20 MB.");
        }

        var fileName = Path.GetFileName(file.FileName);
        await using var fileStream = file.OpenReadStream();

        // Upload file to MinIO via service
        await _minioService.UploadFileAsync(fileName, fileStream, file.Length, file.ContentType);
        Log.Info($"File '{fileName}' uploaded successfully.");

        // Save metadata in the database
        var documentEntity = new Document
        {
            Title = Path.GetFileNameWithoutExtension(file.FileName),
            Metadata = "Example Metadata",
            Description = description
        };

        var createdDocument = await _documentService.AddDocumentAsync(documentEntity);
        var createdDocumentDto = _mapper.Map<DocumentDto>(createdDocument);

        // Send message to RabbitMQ via service
        var message = $"{createdDocument.Id}|{fileName}";
        await _rabbitMqService.PublishMessageAsync(message);
        Log.Info($"Document sent to RabbitMQ queue: {createdDocument.Id} - {createdDocument.Title}");

        Log.Info($"Document with ID: {createdDocumentDto.Id} created successfully.");
        return CreatedAtAction(nameof(Get), new { id = createdDocumentDto.Id }, createdDocumentDto);
    }

    [HttpPut("documents/{id}")]
    public async Task<ActionResult> Put(long id, [FromBody] DocumentDto updatedDocumentDto)
    {
        Log.Info($"Updating document with ID: {id}");

        var result = await _validator.ValidateAsync(updatedDocumentDto);
        if (!result.IsValid)
        {
            Log.Warn("Invalid document data.");
            return BadRequest(result.Errors);
        }

        var documentEntity = _mapper.Map<Document>(updatedDocumentDto);
        documentEntity.Id = id;

        var updateSuccess = await _documentService.UpdateDocumentAsync(documentEntity);
        if (!updateSuccess)
        {
            Log.Warn($"Document with ID: {id} not found for update.");
            return NotFound();
        }

        Log.Info($"Document with ID: {id} updated successfully.");
        return Ok();
    }

    [HttpDelete("documents/{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        Log.Info($"Deleting document with ID: {id}");

        // 1. Check if the document exists in the DB
        var document = await _documentService.GetDocumentByIdAsync(id);
        if (document == null)
        {
            Log.Warn($"Document with ID: {id} not found for deletion.");
            return NotFound();
        }
        // 2. Remove the file from MinIO (use the same unique name used in Upload)
        try
        {
            await _minioService.DeleteFileAsync(document.Title);
            Log.Info($"File '{document.Title}' deleted from MinIO successfully.");
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            Log.Warn($"File '{document.Title}' was not found in MinIO and could not be deleted.");
        }
        catch (Exception ex)
        {
            Log.Error($"Error deleting file '{document.Title}' from MinIO: {ex.Message}");
        }

        // 3. Delete from the DB
        await _documentService.DeleteDocumentAsync(id);
        Log.Info($"Document with ID: {id} deleted from the database successfully.");

        return Ok();
    }

    
    [HttpGet("documents/{id}/download")]
    public async Task<IActionResult> DownloadDocument(long id)
    {
        // Fetch the document from the database
        var document = await _documentService.GetDocumentByIdAsync(id);
        if (document == null)
        {
            return NotFound(new { message = "Document not found." });
        }

        try
        {
            // Download the file from MinIO using its unique title
            var fileStream = await _minioService.DownloadFileAsync(document.Title);

            // Use the document title as the file name with .pdf extension
            var fileName = $"{document.Title}.pdf";

            // Return the file with the correct MIME type and file name
            return File(fileStream, "application/pdf", fileName);
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            return NotFound(new { message = "File not found in storage." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while processing the request.", details = ex.Message });
        }
    }
}
