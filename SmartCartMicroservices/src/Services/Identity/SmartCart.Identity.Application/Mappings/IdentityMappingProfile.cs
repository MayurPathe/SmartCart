using AutoMapper;
using SmartCart.Identity.Application.DTOs;
using SmartCart.Identity.Domain.Entities;

namespace SmartCart.Identity.Application.Mappings;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<User, UserProfileDto>()
            .ForMember(
                destination => destination.UserId,
                option => option.MapFrom(source => source.Id))
            .ForMember(
                destination => destination.Roles,
                option => option.MapFrom(source =>
                    source.UserRoles
                        .Select(userRole => userRole.Role.Name)
                        .ToList()
                 )
            );
    }
}