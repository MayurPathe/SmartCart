using FluentValidation;

namespace SmartCart.Catalog.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.request.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.request.Description).NotEmpty().MaximumLength(500);
    }
}
