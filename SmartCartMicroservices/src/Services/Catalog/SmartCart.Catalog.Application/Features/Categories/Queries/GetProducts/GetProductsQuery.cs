using MediatR;
using SmartCart.Catalog.Application.DTOs;
using SmartCart.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Features.Categories.Queries.GetProducts;

public record GetProductsQuery(ProductListRequest Request) : IRequest<List<ProductDto>>;


