using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DocumentsREST.BL.Services;
using DocumentsREST.BL.DTOs;
using DocumentsREST.DAL.Models;
using log4net;
using FluentValidation;
using FluentValidation.Results;
using RabbitMQ.Client; // added messageQues
using System.Text;

namespace DocumentsREST.Controllers
{
    [Route("/")]
    [ApiController]
    public class DocumentsController : ControllerBase, IDisposable
    {
        private readonly IDocumentService _documentService;
        private readonly IMapper _mapper;
        private readonly IValidator<DocumentDto> _validator;
        private static readonly ILog Log = LogManager.GetLogger(typeof(DocumentsController));
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "document_queue";

        public DocumentsController(IDocumentService documentService, IMapper mapper, IValidator<DocumentDto> validator)
        {
            _documentService = documentService;
            _mapper = mapper;
            _validator = validator;

            var factory = new ConnectionFactory()
            {
                HostName = "DocumentsRabbitMQ", // Use the RabbitMQ container name
                UserName = "admin",
                Password = "admin"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
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

            ValidationResult result = await _validator.ValidateAsync(documentDto);
            if (!result.IsValid)
            {
                Log.Warn("Invalid document data.");
                return BadRequest(result.Errors);
            }

            var documentEntity = _mapper.Map<Document>(documentDto);
            var createdDocument = await _documentService.AddDocumentAsync(documentEntity);
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

            const long maxFileSize = 20 * 1024 * 1024; // 20 MB

            if (file.Length > maxFileSize)
            {
                Log.Warn($"File size {file.Length} exceeds the maximum limit of {maxFileSize} bytes.");
                return BadRequest("File size exceeds the maximum limit of 20 MB.");
            }

            var fileName = file.FileName;
            var title = Path.GetFileNameWithoutExtension(file.FileName);
            var uploadPath = Path.Combine("/app/uploads", fileName);

            // Save the file to the specified path
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var documentEntity = new Document
            {
                Title = title,
                Metadata = "Example Metadata",
                Description = description
            };

            var createdDocument = await _documentService.AddDocumentAsync(documentEntity);
            var createdDocumentDto = _mapper.Map<DocumentDto>(createdDocument);

            // Format the message to include the document ID and file path
            var message = $"{createdDocument.Id}|{uploadPath}";
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            Log.Info($"Document sent to RabbitMQ queue: {createdDocument.Id} - {createdDocument.Title}");

            Log.Info($"Document with ID: {createdDocumentDto.Id} created successfully.");
            return CreatedAtAction(nameof(Get), new { id = createdDocumentDto.Id }, createdDocumentDto);
        }

        [HttpPut("documents/{id}")]
        public async Task<ActionResult> Put(long id, [FromBody] DocumentDto updatedDocumentDto)
        {
            Log.Info($"Updating document with ID: {id}");

            ValidationResult result = await _validator.ValidateAsync(updatedDocumentDto);
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

        private void SendToMessageQueue(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            Log.Info($"[x] Sent {message}");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
