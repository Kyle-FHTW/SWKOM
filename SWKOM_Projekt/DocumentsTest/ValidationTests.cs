namespace DocumentsTest;

[TestFixture]
public class ValidationTests
{
    [Test]
    public void Should_Throw_ValidationException_When_Title_Is_Empty()
    {
        // Arrange
        var document = new Document
        {
            Title = null,  // Invalid title
            Metadata = "Metadata",
            Description = "Description"
        };

        var validationContext = new ValidationContext(document);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(document, validationContext, validationResults, true);

        // Assert
        Assert.That(isValid, Is.False);  // Assert that validation fails
        Assert.That(validationResults, Has.One.Matches<ValidationResult>(v => v.ErrorMessage == "Title is required"));
    }

}