namespace DocumentsREST.Models
{
    
    public class Document
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Metadata { get; set; }
        public string? Description { get; set; }
    }
}