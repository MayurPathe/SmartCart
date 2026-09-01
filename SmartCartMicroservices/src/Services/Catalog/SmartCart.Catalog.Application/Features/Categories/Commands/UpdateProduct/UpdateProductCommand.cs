using MediatR;
using SmartCart.Catalog.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Features.Categories.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    UpdateProductRequest Request) : IRequest<ProductDto>;
