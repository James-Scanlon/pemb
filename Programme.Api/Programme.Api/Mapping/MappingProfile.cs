using AutoMapper;
using Programme.Api.Domain;
using Programme.Api.Dto;
using Wcf.Accounts.ApiClient.Dto;

namespace Programme.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProgrammeAccountDto, ProgrammeAccount>().ReverseMap();
            CreateMap<ProgrammeDto, Domain.Programme>().ReverseMap();
            CreateMap<RatesFormatDto, Domain.RatesFormat>().ReverseMap();
            CreateMap<CurrencyDto, Currency>().ReverseMap();
            CreateMap<ProgrammeStateDto, ProgrammeState>().ReverseMap();
            CreateMap<Dto.EntityTypeDto, EntityType>().ReverseMap();
            CreateMap<Dto.ProgrammeLimitParentDto, ProgrammeLimitParent>().ReverseMap();
            CreateMap<Dto.ProgrammeLimitParentMappingDto, ProgrammeLimitParentMapping>().ReverseMap();
            CreateMap<Dto.ProgrammeLimitMappingHeadDto, ProgrammeLimitParentMappingHead>().ReverseMap();

         

            CreateMap<Dto.ProgrammeSupplierCodeDto, ProgrammeSupplierCode>().ReverseMap();


            CreateMap<ProgrammeLimit, ProgrammeLimitDto>()
                .ForMember(dest => dest.ProgrammeId, opt => opt.MapFrom(src => src.ProgrammeId))
                .ForMember(dest => dest.IsLimitBreach, opt => opt.MapFrom(src => src.IsLimitBreach))
                .ForMember(dest => dest.LimitText, opt => opt.MapFrom(src => src.LimitText))
                .ReverseMap()
                .ForMember(dest => dest.Limit, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.BreachAnalysisMethod, opt => opt.Ignore())
                .ForMember(dest => dest.IsEvergreen, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPurchasesInProgrammeCurrency, opt => opt.Ignore())
                .ForMember(dest => dest.ReserveAmount, opt => opt.Ignore())
                .ForMember(dest => dest.ParentGroup, opt => opt.Ignore())
                .ForMember(dest => dest.ParentConversionRate, opt => opt.Ignore());
            
            CreateMap<EvergreenPositionData, EvergreenPositionDataDto>().ReverseMap();
            CreateMap<AutoPaymentData, Programme.Api.Dto.AutoPaymentDataDto>().ReverseMap();
            CreateMap<AutoPaymentSetting, Programme.Api.Dto.MappedProgrammeSupplierCodeDto>().ReverseMap();
            CreateMap<AutoPaymentSettingsRequest, AutoPaymentSettingsRequestDto>().ReverseMap();
        }
    }
}
