using AutoMapper;
using Programme.Api.Domain;
using Programme.Api.Repositories.Row;

namespace Programme.Api.Repositories;

public class DatabaseMappingProfile : Profile
{
    public DatabaseMappingProfile()
    {
        CreateMap<ProgrammeRow, Domain.Programme>()
            .ForMember(dest => dest.CanAllocate, opt => opt.MapFrom(src => src.EnableForAllocation))
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ReverseMap();

       
        CreateMap<ProgrammeLimitRow, ProgrammeLimit>()
            .ForMember(dest => dest.TotalPurchasesInProgrammeCurrency, opt => opt.MapFrom(src => src.TotalPurchasePrice))
            .ForMember(dest => dest.ParentGroup, opt => opt.Ignore())
            .ForMember(dest => dest.ParentConversionRate, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<ParentLimitRow, LimitGroup>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProgrammeLimitParentId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProgrammeLimitParentName))
            .ForMember(dest => dest.Limit, opt => opt.MapFrom(src => src.GroupLimit))
            .ForMember(dest => dest.Members, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CurrencyRow, Currency>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Currency))
            .ReverseMap();
    }
}