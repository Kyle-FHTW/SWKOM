#region

using FluentValidation;

#endregion

namespace DocumentsREST.BL.DTOs;

public class DocumentDtoValidator : AbstractValidator<DocumentDto>
{
    public DocumentDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Metadata).NotEmpty().WithMessage("Metadata is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
    }
}