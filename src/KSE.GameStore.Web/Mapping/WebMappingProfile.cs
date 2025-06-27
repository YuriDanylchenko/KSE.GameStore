using KSE.GameStore.Web.Requests.Games;
using AutoMapper;
using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.Web.Requests.Payments;
using KSE.GameStore.Web.Requests.Publishers;

namespace KSE.GameStore.Web.Mapping;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        // ─── WRITE MAPPINGS ──────────────────────────────────────────────────────────

        // CreateGameRequest → CreateGameDTO
        CreateMap<CreateGameRequest, CreateGameDTO>()
            .ConstructUsing(src => new CreateGameDTO(
                src.Title,
                src.Description,
                src.PublisherId,
                src.GenreIds,
                src.PlatformIds,
                null!, // will be mapped below
                src.RegionPermissionIds
            ))
            .ForMember(dest => dest.PriceDto, opt => opt.MapFrom(src => new CreateGamePriceDTO(src.Price.Value, src.Price.Stock)));

        // UpdateGameRequest → UpdateGameDTO
        CreateMap<UpdateGameRequest, UpdateGameDTO>()
            .ConstructUsing(src => new UpdateGameDTO(
                src.Id,
                src.Title,
                src.Description,
                src.PublisherId,
                src.GenreIds,
                src.PlatformIds,
                null!, // will be mapped below
                src.RegionPermissionIds
            ))
            .ForMember(dest => dest.PriceDto, opt => opt.MapFrom(src => new UpdateGamePriceDTO(src.Price.Value, src.Price.Stock)));

        // CreateGamePriceRequest → CreateGamePriceDTO
        CreateMap<CreateGamePriceRequest, CreateGamePriceDTO>()
            .ConstructUsing(src => new CreateGamePriceDTO(src.Value, src.Stock));

        // UpdateGamePriceRequest → UpdateGamePriceDTO
        CreateMap<UpdateGamePriceRequest, UpdateGamePriceDTO>()
            .ConstructUsing(src => new UpdateGamePriceDTO(src.Value, src.Stock));

        // CreatePublisherRequest → CreatePublisherDTO
        CreateMap<CreatePublisherRequest, CreatePublisherDTO>()
            .ConstructUsing(src => new CreatePublisherDTO(
                src.Name,
                src.WebsiteUrl,
                src.Description
            ));

        // UpdatePublisherRequest → UpdatePublisherDTO
        CreateMap<UpdatePublisherRequest, UpdatePublisherDTO>()
            .ConstructUsing(src => new UpdatePublisherDTO(
                src.Id,
                src.Name,
                src.WebsiteUrl,
                src.Description
            ));
        
        // CreatePaymentRequest → CreatePaymentDTO
        CreateMap<CreatePaymentRequest, CreatePaymentDTO>()
            .ConstructUsing(src => new CreatePaymentDTO(
                src.OrderId,
                (PaymentMethodDTO)src.PaymentMethod
            ));
        
        // UpdatePaymentRequest → UpdatePaymentDTO
        CreateMap<UpdatePaymentRequest, UpdatePaymentDTO>()
            .ConstructUsing(src => new UpdatePaymentDTO(
                src.Id,
                src.PayedAt,
                (PaymentMethodDTO)src.PaymentMethod
            ));
    }
}