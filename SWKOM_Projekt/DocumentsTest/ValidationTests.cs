namespace DocumentsTest;

[TestFixture]
public class ValidationTests
{
    [SetUp]
    public void Setup()
    {
        _validator = new DocumentDtoValidator();
    }

    private IValidator<DocumentDto> _validator;

    [Test]
    public void Should_Throw_ValidationException_When_Title_Is_Empty()
    {
        // Arrange
        var documentDto = new DocumentDto
        {
            Title = null, // Invalid title
            Metadata = "Metadata",
            Description = "Description"
        };

        // Act
        var result = _validator.Validate(documentDto);

        // Assert
        Assert.That(result.IsValid, Is.False); // Assert that validation fails
        Assert.That(result.Errors, Has.One.Matches<ValidationFailure>(v => v.ErrorMessage == "Title is required."));
    }

    [Test]
    public void Should_Throw_ValidationException_When_Metadata_Is_Empty()
    {
        // Arrange
        var documentDto = new DocumentDto
        {
            Title = "Title",
            Metadata = null, // Invalid metadata
            Description = "Description"
        };

        // Act
        var result = _validator.Validate(documentDto);

        // Assert
        Assert.That(result.IsValid, Is.False); // Assert that validation fails
        Assert.That(result.Errors, Has.One.Matches<ValidationFailure>(v => v.ErrorMessage == "Metadata is required."));
    }

    [Test]
    public void Should_Throw_ValidationException_When_Description_Is_Empty()
    {
        // Arrange
        var documentDto = new DocumentDto
        {
            Title = "Title",
            Metadata = "Metadata",
            Description = null // Invalid description
        };

        // Act
        var result = _validator.Validate(documentDto);

        // Assert
        Assert.That(result.IsValid, Is.False); // Assert that validation fails
        Assert.That(result.Errors,
            Has.One.Matches<ValidationFailure>(v => v.ErrorMessage == "Description is required."));
    }
}