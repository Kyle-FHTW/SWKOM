namespace DocumentsTest;

public class DocumentRepositoryTests
{
    private DbContextOptions<AppDbContext> GetInMemoryDbContextOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique database name per test
            .Options;
    }

    // CREATE (Add) Operation Test
    [Test]
    public async Task Should_Add_Document_Successfully()
    {
        // Arrange
        var options = GetInMemoryDbContextOptions();
        using (var context = new AppDbContext(options))
        {
            var repository = new DocumentRepository(context);
            var document = new Document
            {
                Title = "Test Document",
                Metadata = "Metadata",
                Description = "Description"
            };

            // Act
            await repository.AddDocumentAsync(document);
            await context.SaveChangesAsync();

            // Assert
            var retrievedDocument = await context.Documents.FindAsync(document.Id);
            Assert.That(retrievedDocument, Is.Not.Null);
            Assert.That(retrievedDocument.Title, Is.EqualTo("Test Document"));
        }
    }

    // READ (Retrieve by ID) Operation Test
    [Test]
    public async Task Should_Retrieve_Document_By_Id()
    {
        // Arrange
        var options = GetInMemoryDbContextOptions();
        using (var context = new AppDbContext(options))
        {
            var repository = new DocumentRepository(context);
            var document = new Document
            {
                Title = "Test Document",
                Metadata = "Test Metadata",
                Description = "Test Description"
            };

            // Act
            await repository.AddDocumentAsync(document);
            await context.SaveChangesAsync();

            var retrievedDocument = await repository.GetDocumentByIdAsync(document.Id);

            // Assert
            Assert.That(retrievedDocument, Is.Not.Null);
            Assert.That(retrievedDocument.Title, Is.EqualTo("Test Document"));
            Assert.That(retrievedDocument.Id, Is.EqualTo(document.Id));
        }
    }

    // UPDATE Operation Test
    [Test]
    public async Task Should_Update_Document_Successfully()
    {
        // Arrange
        var options = GetInMemoryDbContextOptions();
        using (var context = new AppDbContext(options))
        {
            var repository = new DocumentRepository(context);
            var document = new Document
            {
                Title = "Original Title",
                Metadata = "Original Metadata",
                Description = "Original Description"
            };

            await repository.AddDocumentAsync(document);
            await context.SaveChangesAsync();

            // Act - Update the document
            document.Title = "Updated Title";
            document.Metadata = "Updated Metadata";
            await repository.UpdateDocumentAsync(document);
            await context.SaveChangesAsync();

            // Assert
            var updatedDocument = await context.Documents.FindAsync(document.Id);
            Assert.That(updatedDocument, Is.Not.Null);
            Assert.That(updatedDocument.Title, Is.EqualTo("Updated Title"));
            Assert.That(updatedDocument.Metadata, Is.EqualTo("Updated Metadata"));
        }
    }

    // DELETE Operation Test
    [Test]
    public async Task Should_Delete_Document_Successfully()
    {
        // Arrange
        var options = GetInMemoryDbContextOptions();
        using (var context = new AppDbContext(options))
        {
            var repository = new DocumentRepository(context);
            var document = new Document
            {
                Title = "Test Document",
                Metadata = "Test Metadata",
                Description = "Test Description"
            };

            await repository.AddDocumentAsync(document);
            await context.SaveChangesAsync();

            // Act - Delete the document
            await repository.DeleteDocumentAsync(document.Id);
            await context.SaveChangesAsync();

            // Assert
            var deletedDocument = await context.Documents.FindAsync(document.Id);
            Assert.That(deletedDocument, Is.Null); // Document should no longer exist
        }
    }

    // READ (Retrieve All) Operation Test
    [Test]
    public async Task Should_Retrieve_All_Documents()
    {
        // Arrange
        var options = GetInMemoryDbContextOptions();
        using (var context = new AppDbContext(options))
        {
            var repository = new DocumentRepository(context);
            var document1 = new Document
            {
                Title = "Test Document 1",
                Metadata = "Test Metadata 1",
                Description = "Test Description 1"
            };

            var document2 = new Document
            {
                Title = "Test Document 2",
                Metadata = "Test Metadata 2",
                Description = "Test Description 2"
            };

            await repository.AddDocumentAsync(document1);
            await repository.AddDocumentAsync(document2);
            await context.SaveChangesAsync();

            // Act
            var documents = await repository.GetAllDocumentsAsync();

            // Assert
            Assert.That(documents.Count, Is.EqualTo(2));
            Assert.That(documents, Has.Exactly(1).Matches<Document>(d => d.Title == "Test Document 1"));
            Assert.That(documents, Has.Exactly(1).Matches<Document>(d => d.Title == "Test Document 2"));
        }
    }
}