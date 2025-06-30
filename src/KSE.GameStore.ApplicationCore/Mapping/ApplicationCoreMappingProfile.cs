using AutoMapper;
using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Models.Output;

using KSE.GameStore.DataAccess.Entities;
using PaymentMethod = KSE.GameStore.DataAccess.Entities.PaymentMethod;

namespace KSE.GameStore.ApplicationCore.Mapping;

public class ApplicationCoreMappingProfile : Profile
{
    public ApplicationCoreMappingProfile()
    {
        // ─── READ MAPPINGS ───────────────────────────────────────────────────────────

        // Game → GameDTO
        CreateMap<Game, GameDTO>()
            .ConstructUsing(src => new GameDTO(
                src.Id,
                src.Title,
                src.Description,
                null!, // Will be mapped below
                null!, // Will be mapped below
                null!, // Will be mapped below
                null!, // Will be mapped below
                null!  // Will be mapped below
            ))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
            .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src => src.Platforms))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Prices.FirstOrDefault(p => p.EndDate == null)))
            .ForMember(dest => dest.RegionPermissions, opt => opt.MapFrom(src => src.RegionPermissions));

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

        CreateMap<Role, RoleDTO>();
        
        // sub-DTOs
        CreateMap<Publisher, PublisherDTO>()
            .ConstructUsing(src => new PublisherDTO(
                src.Id,
                src.Name,
                src.WebsiteUrl,
                src.Description
            ));

        CreateMap<Genre, GenreDTO>()
            .ConstructUsing(src => new GenreDTO(
                src.Id,
                src.Name
            ));

        CreateMap<Platform, PlatformDTO>()
            .ConstructUsing(src => new PlatformDTO(
                src.Id,
                src.Name
            ));

        CreateMap<Region, RegionDTO>()
            .ConstructUsing(src => new RegionDTO(
                src.Id,
                src.Code,
                src.Name
            ));

        CreateMap<GamePrice, GamePriceDTO>()
            .ConstructUsing(src => new GamePriceDTO(
                src.Id,
                src.Value,
                src.Stock
            ));

        CreateMap<OrderItem, CartItemDto>()
            .ConstructUsing(src => new CartItemDto(
                src.Id,
                src.GameId,
                src.Game.Title,
                src.Price,
                src.Quantity))
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Game.Title));

        CreateMap<Payment, PaymentDTO>()
            .ConstructUsing(src => new PaymentDTO(
                src.Id,
                src.OrderId,
                src.Confirmed,
                src.PayedAt,
                null!
            ))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod));

        CreateMap<PaymentMethod, PaymentMethodDTO>();

        CreateMap<Order, OrderDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<OrderItem, OrderItemDTO>();

        // ─── WRITE MAPPINGS ──────────────────────────────────────────────────────────

        // CreateGameDTO → Game
        CreateMap<CreateGameDTO, Game>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id, will be set by EF
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Publisher, opt => opt.Ignore()) // Will be set via PublisherId
            .ForMember(dest => dest.Genres, opt => opt.Ignore()) // Will be handled in service layer
            .ForMember(dest => dest.Platforms, opt => opt.Ignore()) // Will be handled in service layer
            .ForMember(dest => dest.Prices, opt => opt.Ignore()) // Will be handled in service layer
            .ForMember(dest => dest.RegionPermissions, opt => opt.Ignore())
            .ForMember(dest => dest.Customers, opt => opt.Ignore()); // Will be handled in service layer

        // CreateGamePriceDTO → GamePrice
        CreateMap<CreateGamePriceDTO, GamePrice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(_ => (DateTime?)null))
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.GameId, opt => opt.Ignore());

        // CreatePublisherDTO → Publisher
        CreateMap<CreatePublisherDTO, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id, will be set by EF
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.WebsiteUrl, opt => opt.MapFrom(src => src.WebsiteUrl))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Games, opt => opt.Ignore());
        
        // CreatePaymentDTO → Payment
        CreateMap<CreatePaymentDTO, Payment>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.Confirmed, opt => opt.Ignore())
            .ForMember(dest => dest.PayedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        
        // UpdateGameDTO → Game
        CreateMap<UpdateGameDTO, Game>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Preserve existing
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Publisher, opt => opt.Ignore()) // Will be handled separately
            .ForMember(dest => dest.Genres, opt => opt.Ignore()) // Will be handled separately
            .ForMember(dest => dest.Platforms, opt => opt.Ignore()) // Will be handled separately
            .ForMember(dest => dest.Prices, opt => opt.Ignore()) // Will be handled separately
            .ForMember(dest => dest.RegionPermissions, opt => opt.Ignore())
            .ForMember(dest => dest.Customers, opt => opt.Ignore()); // Will be handled separately

        // UpdateGamePriceDTO → GamePrice
        CreateMap<UpdateGamePriceDTO, GamePrice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(_ => (DateTime?)null))
            .ForMember(dest => dest.Game, opt => opt.Ignore())
            .ForMember(dest => dest.GameId, opt => opt.Ignore());

        // UpdatePublisherDTO → Publisher
        CreateMap<UpdatePublisherDTO, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.WebsiteUrl, opt => opt.MapFrom(src => src.WebsiteUrl))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Games, opt => opt.Ignore());
        
        // UpdatePaymentDTO → Payment
        CreateMap<UpdatePaymentDTO, Payment>()
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.Confirmed, opt => opt.Ignore())
            .ForMember(dest => dest.PayedAt, opt => opt.MapFrom(src => src.PayedAt))
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}