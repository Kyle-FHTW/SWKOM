using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentsREST.BL.Services;
using DocumentsREST.BL.DTOs;
using DocumentsREST.DAL.Models;

namespace DocumentsREST.Controllers
{
    [Route("/")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;  // Injected service to interact with DB
        private readonly IMapper _mapper;  // AutoMapper for mapping Document <-> DocumentDto

        public DocumentsController(IDocumentService documentService, IMapper mapper)
        {
            _documentService = documentService;
            _mapper = mapper;
        }

        [HttpGet("documents")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> Get()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            return Ok(documents);
        }

        [HttpGet("documents/{id}")]
        public async Task<ActionResult<DocumentDto>> Get(long id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return Ok(document);
        }

        [HttpPost("documents")]
        public async Task<ActionResult<DocumentDto>> Post([FromBody] DocumentDto documentDto)
        {
            // Validate the input (optional)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map DTO to the entity
            var documentEntity = _mapper.Map<Document>(documentDto);

            // Add the new document to the database
            await _documentService.AddDocumentAsync(documentDto);

            // Retrieve the newly added document with its generated Id
            var createdDocumentEntity = _mapper.Map<Document>(documentDto);
            var createdDocumentDto = _mapper.Map<DocumentDto>(createdDocumentEntity);

            // Return the document with the generated Id
            return CreatedAtAction(nameof(Get), new { id = createdDocumentDto.Id }, createdDocumentDto);
        }

        [HttpPut("documents/{id}")]
        public async Task<ActionResult> Put(long id, [FromBody] DocumentDto updatedDocumentDto)
        {
            var existingDocument = await _documentService.GetDocumentByIdAsync(id);
            if (existingDocument == null)
            {
                return NotFound();
            }

            await _documentService.UpdateDocumentAsync(id, updatedDocumentDto);
            return Ok();
        }

        [HttpDelete("documents/{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            await _documentService.DeleteDocumentAsync(id);
            return Ok();
        }
    }
}
