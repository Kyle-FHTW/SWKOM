namespace DocumentsTest;

[TestFixture]
public class DocumentsControllerTests
{
    [SetUp]
    public void Setup()
    {
        _mockDocumentService = new Mock<IDocumentService>();
        _mockMapper = new Mock<IMapper>();
        _mockValidator = new Mock<IValidator<DocumentDto>>();
        _mockRabbitMqService = new Mock<IRabbitMqService>();
        _mockMinioService = new Mock<IMinioService>();

        _controller = new DocumentsController(
            _mockDocumentService.Object,
            _mockMapper.Object,
            _mockValidator.Object,
            _mockRabbitMqService.Object,
            _mockMinioService.Object
        );
    }

    private Mock<IDocumentService> _mockDocumentService;
    private Mock<IMapper> _mockMapper;
    private Mock<IValidator<DocumentDto>> _mockValidator;
    private Mock<IRabbitMqService> _mockRabbitMqService;
    private Mock<IMinioService> _mockMinioService;
    private DocumentsController _controller;

    [Test]
    public async Task Get_AllDocuments_ReturnsOkWithDocuments()
    {
        // Arrange
        _mockDocumentService
            .Setup(s => s.GetAllDocumentsAsync())
            .ReturnsAsync(new List<Document>
            {
                new() { Id = 1, Title = "Doc1" },
                new() { Id = 2, Title = "Doc2" }
            });

        _mockMapper
            .Setup(m => m.Map<IEnumerable<DocumentDto>>(It.IsAny<IEnumerable<Document>>()))
            .Returns(new List<DocumentDto>
            {
                new() { Id = 1, Title = "Doc1" },
                new() { Id = 2, Title = "Doc2" }
            });

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.That(result, Is.Not.Null);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var returnedDocuments = okResult.Value as IEnumerable<DocumentDto>;
        Assert.That(returnedDocuments, Is.Not.Null);
        Assert.That(returnedDocuments.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task Get_ById_DocumentExists_ReturnsOkWithDocument()
    {
        // Arrange
        var document = new Document { Id = 1, Title = "Doc1" };
        var documentDto = new DocumentDto { Id = 1, Title = "Doc1" };

        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1)).ReturnsAsync(document);
        _mockMapper.Setup(m => m.Map<DocumentDto>(document)).Returns(documentDto);

        // Act
        var result = await _controller.Get(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var returnedDocument = okResult.Value as DocumentDto;
        Assert.That(returnedDocument, Is.Not.Null);
        Assert.That(returnedDocument.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task Get_ById_DocumentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockDocumentService
            .Setup(s => s.GetDocumentByIdAsync(It.IsAny<long>()))
            .ReturnsAsync((Document)null);

        // Act
        var result = await _controller.Get(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task UploadDocument_ValidFile_ReturnsCreated()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("test.txt");
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        var document = new Document { Id = 1, Title = "test" };
        var documentDto = new DocumentDto { Id = 1, Title = "test" };

        _mockDocumentService.Setup(s => s.AddDocumentAsync(It.IsAny<Document>())).ReturnsAsync(document);
        _mockMapper.Setup(m => m.Map<DocumentDto>(document)).Returns(documentDto);

        // Act
        var result = await _controller.UploadDocument("Test description", fileMock.Object) as CreatedAtActionResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(201));
        var returnedDocument = result.Value as DocumentDto;
        Assert.That(returnedDocument, Is.Not.Null);
        Assert.That(returnedDocument.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task Put_ValidData_ReturnsOk()
    {
        // Arrange
        var updatedDto = new DocumentDto { Id = 1, Title = "Updated Doc" };
        var documentEntity = new Document { Id = 1, Title = "Updated Doc" };
        var validationResult = new ValidationResult(); // Represents a valid object

        _mockValidator
            .Setup(v => v.ValidateAsync(updatedDto, default))
            .ReturnsAsync(validationResult);

        _mockMapper
            .Setup(m => m.Map<Document>(updatedDto))
            .Returns(documentEntity);

        _mockDocumentService
            .Setup(s => s.UpdateDocumentAsync(documentEntity))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Put(1, updatedDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    public async Task Put_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var updatedDto = new DocumentDto { Id = 1, Title = "Invalid Doc" };
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new("Title", "Title is required")
        });

        _mockValidator.Setup(v => v.ValidateAsync(updatedDto, default)).ReturnsAsync(validationResult);

        // Act
        var result = await _controller.Put(1, updatedDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Delete_DocumentExists_ReturnsOk()
    {
        // Arrange
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1)).ReturnsAsync(new Document { Id = 1 });
        _mockDocumentService.Setup(s => s.DeleteDocumentAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    public async Task Delete_DocumentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1)).ReturnsAsync((Document)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
