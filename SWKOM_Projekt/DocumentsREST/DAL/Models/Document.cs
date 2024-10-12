using System.ComponentModel.DataAnnotations;

namespace DocumentsREST.DAL.Models
{
    public class Document
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }
        public string? Metadata { get; set; }
        public string? Description { get; set; }
    }
}