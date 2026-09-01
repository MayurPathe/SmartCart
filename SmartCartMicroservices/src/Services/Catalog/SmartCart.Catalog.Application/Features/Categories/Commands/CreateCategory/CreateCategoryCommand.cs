using MediatR;
using SmartCart.Catalog.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(CreateCategoryRequest request) : IRequest<CategoryDto>;
