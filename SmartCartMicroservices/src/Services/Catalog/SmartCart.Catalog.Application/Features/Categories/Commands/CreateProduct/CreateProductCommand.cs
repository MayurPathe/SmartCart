using MediatR;
using SmartCart.Catalog.Application.DTOs;

namespace SmartCart.Catalog.Application.Features.Categories.Commands.CreateProduct;

public record CreateProductCommand(CreateProductRequest Request) : IRequest<ProductDto>;

