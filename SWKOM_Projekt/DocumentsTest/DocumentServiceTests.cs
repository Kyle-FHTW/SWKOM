namespace DocumentsTest;

[TestFixture]
public class DocumentServiceTests
{
    [SetUp]
    public void Setup()
    {
        // Create a new Mock for IDocumentRepository
        _mockRepository = new Mock<IDocumentRepository>();

        // Initialize the DocumentService with the mocked repository
        _documentService = new DocumentService(_mockRepository.Object);
    }

    private Mock<IDocumentRepository> _mockRepository;
    private DocumentService _documentService;

    // CREATE Operation Test
    [Test]
    public async Task Should_Call_AddDocumentAsync_Once_When_Adding_Document()
    {
        // Arrange
        var documentEntity = new Document
        {
            Title = "New Document",
            Metadata = "Metadata",
            Description = "Description"
        };

        // Act
        await _documentService.AddDocumentAsync(documentEntity);

        // Assert
        _mockRepository.Verify(repo => repo.AddDocumentAsync(documentEntity), Times.Once);
    }

    // READ Operation Test (Get by ID)
    [Test]
    public async Task Should_Return_Document_When_Document_Exists()
    {
        // Arrange
        var documentEntity = new Document
        {
            Id = 1,
            Title = "Test Document",
            Metadata = "Metadata",
            Description = "Description"
        };

        _mockRepository.Setup(repo => repo.GetDocumentByIdAsync(1))
            .ReturnsAsync(documentEntity);

        // Act
        var result = await _documentService.GetDocumentByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("Test Document"));
        Assert.That(result.Id, Is.EqualTo(1));
    }

    // READ Operation Test (Get All)
    [Test]
    public async Task Should_Return_All_Documents_From_Repository()
    {
        // Arrange
        var documentEntities = new List<Document>
        {
            new() { Id = 1, Title = "Document 1", Metadata = "Metadata 1", Description = "Description 1" },
            new() { Id = 2, Title = "Document 2", Metadata = "Metadata 2", Description = "Description 2" }
        };

        _mockRepository.Setup(repo => repo.GetAllDocumentsAsync())!
            .ReturnsAsync(documentEntities);

        // Act
        var result = await _documentService.GetAllDocumentsAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.First().Title, Is.EqualTo("Document 1"));
        Assert.That(result.Last().Title, Is.EqualTo("Document 2"));
    }

    // UPDATE Operation Test
    [Test]
    public async Task Should_Call_UpdateDocumentAsync_Once_When_Updating_Document()
    {
        // Arrange
        var updatedDocument = new Document
        {
            Id = 1,
            Title = "Updated Title",
            Metadata = "Updated Metadata",
            Description = "Updated Description"
        };

        var existingDocumentEntity = new Document
        {
            Id = 1,
            Title = "Old Title",
            Metadata = "Old Metadata",
            Description = "Old Description"
        };

        _mockRepository.Setup(repo => repo.GetDocumentByIdAsync(1))
            .ReturnsAsync(existingDocumentEntity);

        // Act
        var updateSuccess = await _documentService.UpdateDocumentAsync(updatedDocument);

        // Assert
        Assert.That(updateSuccess, Is.True);
        _mockRepository.Verify(repo => repo.UpdateDocumentAsync(existingDocumentEntity), Times.Once);
    }

    // DELETE Operation Test
    [Test]
    public async Task Should_Call_DeleteDocumentAsync_Once_When_Deleting_Document()
    {
        // Arrange
        var mockDocument = new Document { Id = 1, Title = "Sample Document" };

        // Set up the repository to return the document when GetDocumentByIdAsync is called
        _mockRepository.Setup(repo => repo.GetDocumentByIdAsync(1)).ReturnsAsync(mockDocument);

        // Act
        await _documentService.DeleteDocumentAsync(1);

        // Assert
        _mockRepository.Verify(repo => repo.DeleteDocumentAsync(1), Times.Once);
    }
}