using AutoMapper;
using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Requests.Games;
using KSE.GameStore.DataAccess.Entities;

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
        CreateMap<Genre,     GenreDTO>();
        CreateMap<Platform,  PlatformDTO>();
        CreateMap<Region,    RegionDTO>();
        CreateMap<GamePrice, GamePriceDTO>();

        // Game → GameDTO
        CreateMap<Game, GameDTO>()
            // scalars: Id, Title, Description
            // Publisher → PublisherDTO
            .ForMember(d => d.Publisher, 
                       o => o.MapFrom(s => s.Publisher))
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
            .ForMember(d => d.Genres,            o => o.Ignore())
            .ForMember(d => d.Platforms,         o => o.Ignore())
            .ForMember(d => d.RegionPermissions, o => o.Ignore())
            .ForMember(d => d.Prices,            o => o.Ignore());

        // CreateGamePriceRequest → GamePrice (if you ever map it directly)
        CreateMap<CreateGamePriceRequest, GamePrice>();

        // UpdateGameRequest → Game
        CreateMap<UpdateGameRequest, Game>()
            // scalars: Title, Description, PublisherId
            .ForMember(d => d.Genres,            o => o.Ignore())
            .ForMember(d => d.Platforms,         o => o.Ignore())
            .ForMember(d => d.RegionPermissions, o => o.Ignore())
            .ForMember(d => d.Prices,            o => o.Ignore());

        // UpdateGamePriceRequest → GamePrice
        CreateMap<UpdateGamePriceRequest, GamePrice>();
    }
}
