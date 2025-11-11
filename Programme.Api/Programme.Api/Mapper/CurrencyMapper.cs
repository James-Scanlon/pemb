using AutoMapper;
using System;
using Programme.Api.Domain;
using Programme.Api.Repositories;

namespace Programme.Api.Mapper
{
    public class CurrencyMapper(ICurrencyRepository currencyRepository) : IValueConverter<string, Currency>
    {
        private readonly ICurrencyRepository _currencyRepository = currencyRepository ?? throw new ArgumentNullException(nameof(currencyRepository));

        public Currency Convert(string sourceMember, ResolutionContext context)
        {
            return _currencyRepository.GetByCodeAsync(sourceMember).Result;
        }
    }
}
