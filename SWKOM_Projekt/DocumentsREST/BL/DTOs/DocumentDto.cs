namespace DocumentsREST.BL.DTOs;

public class DocumentDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Metadata { get; set; }
    public string Description { get; set; }
}