using firstCoreAttemp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace firstCoreAttemp.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/documents")]

    [ApiController]
    public class DocumentsController : ControllerBase
    {
        // GET: api/<ValuesController>
        List<Document> myArrayList = new List<Document>();

        public DocumentsController() {
            Document doc1 = new Document
            {
                Id = 1,
                Title = "Test",
                Metadata ="dfsfd",
                Description = "Test",

            };
            Document doc2 = new Document();
            myArrayList.Add(doc1);
            myArrayList.Add(doc2);
        
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {

            // return new string[] { doc1.Title, "task2" };
            List<string> resultList = new List<string>();

            foreach (Document doc in myArrayList)
            {
                resultList.Add(doc.ToString()); 
               
            }
            return resultList.ToArray();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            foreach (var item in myArrayList)
            {

                if (item.Id == id)
                {
                    return item.ToString();
                }
            }    
             return id.ToString();
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            // insert 

        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
