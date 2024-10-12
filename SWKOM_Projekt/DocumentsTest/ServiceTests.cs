namespace DocumentsTest;

[TestFixture]
public class ServiceTests
{
    private Mock<IDocumentRepository> _mockRepository;
        private Mock<IMapper> _mockMapper; // Mock the IMapper
        private DocumentService _documentService;

        [SetUp]
        public void Setup()
        {
            // Create a new Mock for IDocumentRepository
            _mockRepository = new Mock<IDocumentRepository>();
            
            // Create a new Mock for IMapper
            _mockMapper = new Mock<IMapper>();
            
            // Initialize the DocumentService with the mocked repository and mapper
            _documentService = new DocumentService(_mockRepository.Object, _mockMapper.Object);
        }

        // CREATE Operation Test
        [Test]
        public async Task Should_Call_AddDocumentAsync_Once_When_Adding_Document()
        {
            // Arrange
            var documentDto = new DocumentDto
            {
                Title = "New Document",
                Metadata = "Metadata",
                Description = "Description"
            };

            var documentEntity = new Document
            {
                Title = "New Document",
                Metadata = "Metadata",
                Description = "Description"
            };

            // Setup the mapping from DocumentDto to Document
            _mockMapper.Setup(m => m.Map<Document>(documentDto))
                       .Returns(documentEntity);

            // Act
            await _documentService.AddDocumentAsync(documentDto);

            // Assert
            _mockRepository.Verify(repo => repo.AddDocumentAsync(documentEntity), Times.Once);
        }

        // READ Operation Test (Get by ID)
        [Test]
        public async Task Should_Return_DocumentDto_When_Document_Exists()
        {
            // Arrange
            var documentEntity = new Document
            {
                Id = 1,
                Title = "Test Document",
                Metadata = "Metadata",
                Description = "Description"
            };

            var documentDto = new DocumentDto
            {
                Id = 1,
                Title = "Test Document",
                Metadata = "Metadata",
                Description = "Description"
            };

            _mockRepository.Setup(repo => repo.GetDocumentByIdAsync(1))
                           .ReturnsAsync(documentEntity);

            // Setup the mapping from Document to DocumentDto
            _mockMapper.Setup(m => m.Map<DocumentDto>(documentEntity))
                       .Returns(documentDto);

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
                new Document { Id = 1, Title = "Document 1", Metadata = "Metadata 1", Description = "Description 1" },
                new Document { Id = 2, Title = "Document 2", Metadata = "Metadata 2", Description = "Description 2" }
            };

            var documentDtos = new List<DocumentDto>
            {
                new DocumentDto { Id = 1, Title = "Document 1", Metadata = "Metadata 1", Description = "Description 1" },
                new DocumentDto { Id = 2, Title = "Document 2", Metadata = "Metadata 2", Description = "Description 2" }
            };

            _mockRepository.Setup(repo => repo.GetAllDocumentsAsync())
                           .ReturnsAsync(documentEntities);

            // Setup the mapping from List<Document> to List<DocumentDto>
            _mockMapper.Setup(m => m.Map<IEnumerable<DocumentDto>>(documentEntities))
                       .Returns(documentDtos);

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
            var documentDto = new DocumentDto
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

            // Setup the mapping from DocumentDto to Document (for update)
            _mockMapper.Setup(m => m.Map(documentDto, existingDocumentEntity));

            // Act
            await _documentService.UpdateDocumentAsync(1, documentDto);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateDocumentAsync(existingDocumentEntity), Times.Once);
        }

        // DELETE Operation Test
        [Test]
        public async Task Should_Call_DeleteDocumentAsync_Once_When_Deleting_Document()
        {
            // Act
            await _documentService.DeleteDocumentAsync(1);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteDocumentAsync(1), Times.Once);
        }
}