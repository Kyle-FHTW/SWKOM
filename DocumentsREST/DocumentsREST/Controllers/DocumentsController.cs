using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DocumentsREST.Models;

namespace DocumentsREST.Controllers
{
    [Route("/")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private static List<Document> documents = new List<Document>
        {
            new Document { Id = 1, Title = "Document 1", Metadata = "Metadata 1", Description = "Description 1" },
            new Document { Id = 2, Title = "Document 2", Metadata = "Metadata 2", Description = "Description 2" }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Document>> Get()
        {
            return Ok(documents);
        }

        [HttpGet("documents/{id}")]
        public ActionResult<Document> Get(int id)
        {
            var document = documents.Find(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }
            return Ok(document);
        }

        [HttpPost("documents")]
        public ActionResult Post([FromBody] Document document)
        {
            documents.Add(document);
            return CreatedAtAction(nameof(Get), new { id = document.Id }, document);
        }

        [HttpPut("documents/{id}")]
        public ActionResult Put(int id, [FromBody] Document updatedDocument)
        {
            var document = documents.Find(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }
            document.Title = updatedDocument.Title;
            document.Metadata = updatedDocument.Metadata;
            document.Description = updatedDocument.Description;
            return Ok();
        }

        [HttpDelete("documents/{id}")]
        public ActionResult Delete(int id)
        {
            var document = documents.Find(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }
            documents.Remove(document);
            return Ok();
        }
    }
}