using AutoMapper;
using MediatR;
using SmartCart.Catalog.Application.DTOs;
using SmartCart.Catalog.Application.Interfaces;
using SmartCart.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Catalog.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsByNameAsync(request.request.Name);

        if (exists)
        {
            throw new InvalidOperationException("Category already exists.");
        }

        var category = _mapper.Map<Category>(request.request);

        category.Id = Guid.NewGuid();
        category.CreatedAt = DateTime.UtcNow;

        await _repository.AddAsync(category);

        await _repository.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }
}
