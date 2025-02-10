using AutoMapper;
using FinPriceFeed.Domain.Model;
using FinPriceFeed.ExternalProviders.TwelveData.DTOs;

namespace FinPriceFeed.Core.Mapping
{
    public class FinInstrumentProfile : Profile
    {
        public FinInstrumentProfile()
        {
            CreateMap<TDData, FinInstrument>()
                .ForMember(dest => dest.Ticker, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.QuoteCurrency, opt => opt.MapFrom(src => src.CurrencyQuote))
                .ForMember(dest => dest.BaseCurrency, opt => opt.MapFrom(src => src.CurrencyBase));
        }
    }
}
