using MediatR;
using SmartCart.Catalog.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Features.Categories.Queries.GetProductById;

public record GetProductByIdQuery(
    Guid Id) : IRequest<ProductDto>;
