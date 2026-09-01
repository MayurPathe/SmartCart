using FluentValidation;

namespace SmartCart.Catalog.Application.Features.Categories.Commands.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Request.CategoryId).NotEmpty();

        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Request.Sku).NotEmpty().MaximumLength(50);

        RuleFor(x => x.Request.Price).GreaterThan(0);
    }
}

