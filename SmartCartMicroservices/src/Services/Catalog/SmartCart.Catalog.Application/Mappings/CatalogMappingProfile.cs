using AutoMapper;
using SmartCart.Catalog.Application.DTOs;
using SmartCart.Catalog.Domain.Entities;

namespace SmartCart.Catalog.Application.Mappings;

public class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        CreateMap<CreateCategoryRequest, Category>();

        CreateMap<Category, CategoryDto>();

        CreateMap<CreateProductRequest, Product>();

        CreateMap<UpdateProductRequest, Product>();

        CreateMap<Product, ProductDto>()
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(src =>
                    src.Category != null
                        ? src.Category.Name
                        : string.Empty));
    }
}