namespace DocumentsTest;

[TestFixture]
public class MappingTests
{
    [SetUp]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DocumentMappingProfile()); // Your actual mapping profile
        });

        _mapper = config.CreateMapper();
    }

    private IMapper _mapper;

    [Test]
    public void Should_Ignore_Id_When_Mapping_DocumentDto_To_Document()
    {
        // Arrange
        var documentDto = new DocumentDto
        {
            Id = 1, // This Id should be ignored in the mapping
            Title = "Test Title",
            Metadata = "Test Metadata",
            Description = "Test Description"
        };

        // Act
        var document = _mapper.Map<Document>(documentDto);

        // Assert
        Assert.That(document.Id, Is.EqualTo(0)); // Since the Id is ignored, it should be 0 (default for long)
        Assert.That(document.Title, Is.EqualTo(documentDto.Title));
        Assert.That(document.Metadata, Is.EqualTo(documentDto.Metadata));
        Assert.That(document.Description, Is.EqualTo(documentDto.Description));
    }

    [Test]
    public void Should_Map_Document_To_DocumentDto_Including_Id()
    {
        // Arrange
        var document = new Document
        {
            Id = 1, // This Id comes from the database
            Title = "Test Title",
            Metadata = "Test Metadata",
            Description = "Test Description"
        };

        // Act
        var documentDto = _mapper.Map<DocumentDto>(document);

        // Assert
        Assert.That(documentDto.Id, Is.EqualTo(document.Id)); // The Id should be mapped correctly
        Assert.That(documentDto.Title, Is.EqualTo(document.Title));
        Assert.That(documentDto.Metadata, Is.EqualTo(document.Metadata));
        Assert.That(documentDto.Description, Is.EqualTo(document.Description));
    }
}