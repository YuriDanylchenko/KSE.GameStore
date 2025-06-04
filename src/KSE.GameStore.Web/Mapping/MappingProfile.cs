using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.Web.Requests.Games;

namespace KSE.GameStore.Web.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //
        // ─── READ MAPPINGS ───────────────────────────────────────────────────────────
        //

        // sub-DTOs
        CreateMap<Publisher, PublisherDTO>();
        CreateMap<Genre, GenreDTO>();
        CreateMap<Platform, PlatformDTO>();
        CreateMap<Region, RegionDTO>();
        CreateMap<GamePrice, GamePriceDTO>();

        // Game → GameDTO
        CreateMap<Game, GameDTO>()
            // scalars: Id, Title, Description
            // Publisher → PublisherDTO
            .ForMember(d => d.Publisher,
                o => o.MapFrom(s => s.Publisher.Name))
            // full GenreDTO list
            .ForMember(d => d.Genres,
                o => o.MapFrom(s => s.Genres))
            // full PlatformDTO list
            .ForMember(d => d.Platforms,
                o => o.MapFrom(s => s.Platforms))
            // pick *the* current price (EndDate == null)
            .ForMember(d => d.Price,
                o => o.MapFrom(s =>
                    s.Prices.FirstOrDefault(p => p.EndDate == null)))
            // full RegionDTO list
            .ForMember(d => d.RegionPermissions,
                o => o.MapFrom(s => s.RegionPermissions));

        //
        // ─── WRITE MAPPINGS ──────────────────────────────────────────────────────────
        //

        // CreateGameRequest → Game
        CreateMap<CreateGameRequest, Game>()
            // scalars: Title, Description, PublisherId
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Genres, o => o.Ignore())
            .ForMember(d => d.Platforms, o => o.Ignore())
            .ForMember(d => d.RegionPermissions, o => o.Ignore())
            .ForMember(d => d.Prices, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.Publisher, o => o.Ignore());

        CreateMap<CreateGamePriceRequest, GamePrice>()
            .ForMember(d => d.StartDate, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.EndDate, o => o.MapFrom(_ => (DateTime?)null))
            .ForMember(d => d.GameId, o => o.Ignore())
            .ForMember(d => d.Game, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore());

        // UpdateGameRequest → Game
        CreateMap<UpdateGameRequest, Game>()
            // scalars: Title, Description, PublisherId
            .ForMember(d => d.Genres, o => o.Ignore())
            .ForMember(d => d.Platforms, o => o.Ignore())
            .ForMember(d => d.RegionPermissions, o => o.Ignore())
            .ForMember(d => d.Prices, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.Publisher, o => o.Ignore());

        // UpdateGamePriceRequest → GamePrice
        CreateMap<UpdateGamePriceRequest, GamePrice>()
            .ForMember(d => d.StartDate, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.EndDate, o => o.Ignore())
            .ForMember(d => d.GameId, o => o.Ignore())
            .ForMember(d => d.Game, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore());
    }
}