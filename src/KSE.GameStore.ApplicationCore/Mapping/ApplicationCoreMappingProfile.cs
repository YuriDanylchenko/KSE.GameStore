using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;

namespace KSE.GameStore.ApplicationCore.Mapping;

public class ApplicationCoreMappingProfile : Profile
{
    public ApplicationCoreMappingProfile()
    {
        // ─── READ MAPPINGS ───────────────────────────────────────────────────────────

        // Game → GameDTO
        CreateMap<Game, GameDTO>()
            .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src =>
                    src.Prices.FirstOrDefault(p => p.EndDate == null)));

        // User → UserDTO
        CreateMap<User, UserDTO>()
            .ConstructUsing((src, context) => new UserDTO(
                src.Id,
                src.Email,
                src.HashedPassword,
                src.PasswordSalt,
                src.Region != null ? context.Mapper.Map<RegionDTO>(src.Region) : null,
                src.UserRoles != null
                    ? src.UserRoles.Select(ur => context.Mapper.Map<RoleDTO>(ur.Role)).ToList()
                    : []
            ))
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        // sub-DTOs
        CreateMap<Publisher, PublisherDTO>();
        CreateMap<Genre, GenreDTO>();
        CreateMap<Platform, PlatformDTO>();
        CreateMap<Region, RegionDTO>();
        CreateMap<GamePrice, GamePriceDTO>();
        CreateMap<Role, RoleDTO>();

        // ─── WRITE MAPPINGS ──────────────────────────────────────────────────────────

        // GameDTO → Game
        CreateMap<GameDTO, Game>()
            .ForMember(dest => dest.Genres, opt => opt.Ignore())
            .ForMember(dest => dest.Platforms, opt => opt.Ignore())
            .ForMember(dest => dest.RegionPermissions, opt => opt.Ignore())
            .ForMember(dest => dest.Prices, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Publisher, opt => opt.Ignore());

        // GamePriceDTO → GamePrice
        CreateMap<GamePriceDTO, GamePrice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(_ => (DateTime?)null))
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.GameId, opt => opt.Ignore());
    }
}