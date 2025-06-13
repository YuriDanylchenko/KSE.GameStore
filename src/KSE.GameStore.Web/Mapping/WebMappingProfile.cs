using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.Web.Requests.Games;

namespace KSE.GameStore.Web.Mapping;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        // ─── WRITE MAPPINGS ──────────────────────────────────────────────────────────

        // CreateGameRequest → GameDTO
        CreateMap<CreateGameRequest, GameDTO>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => new PublisherDTO { Id = src.PublisherId }))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                src.GenreIds.Select(id => new GenreDTO { Id = id })))
            .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src =>
                src.PlatformIds.Select(id => new PlatformDTO { Id = id })))
            .ForMember(dest => dest.RegionPermissions, opt => opt.MapFrom(src =>
                src.RegionPermissionIds != null
                    ? src.RegionPermissionIds.Select(id => new RegionDTO(id, null, null)).ToList()
                    : null))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

        // CreateGamePriceRequest → GamePriceDTO
        CreateMap<CreateGamePriceRequest, GamePriceDTO>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // UpdateGameRequest → GameDTO
        CreateMap<UpdateGameRequest, GameDTO>()
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => new PublisherDTO { Id = src.PublisherId }))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                src.GenreIds.Select(id => new GenreDTO { Id = id })))
            .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src =>
                src.PlatformIds.Select(id => new PlatformDTO { Id = id })))
            .ForMember(dest => dest.RegionPermissions, opt => opt.MapFrom(src =>
                src.RegionPermissionIds != null
                    ? src.RegionPermissionIds.Select(id => new RegionDTO(id, null, null)).ToList()
                    : null))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

        // UpdateGamePriceRequest → GamePriceDTO
        CreateMap<UpdateGamePriceRequest, GamePriceDTO>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}