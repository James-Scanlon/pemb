using Wcf.Accounts.ApiClient.Dto;

namespace Programme.ApiClient.Dto
{
    public class ProgrammeDto
    {
        public int ProgrammeId { get; set; }

        public string Name { get; set; }

        public string CurrencyIsoCode { get; set; }

        public bool IsEvergreen { get; set; }

        public bool IsActive { get; set; }

        public bool CanAllocate { get; set; }

        public decimal InsuredPercent { get; set; }

        public decimal UninsuredPercent { get; set; }

        public ProgrammeAccountDto Account { get; set; }

        public string ProgrammeName { get; set; }

        public decimal Limit { get; set; }

        public string AccountNumber { get; set; }

        public string BnyMapping { get; set; }

        public string Comment { get; set; }

        public string UpdatedBy { get; set; }

        public int? MaxTenor { get; set; }

        public string Guarantor { get; set; }

        public string ProgrammeType { get; set; }

        public bool IsBatchCalculationPrincipal { get; set; }

        public int? ProgrammeTypeId { get; set; }

        public bool? EnableForAllocation { get; set; }

        public int? BranchId { get; set; }

        public bool? TradeUpload { get; set; }

        public bool? HasSplits { get; set; }

        public string BranchName { get; set; }

        public int? OriginationEntityId { get; set; }

        public int? ProgrammeLimitParentId { get; set; }

        public decimal? GroupedLimit { get; set; }

        public string BreachAnalysisMethod { get; set; }

        public string Provider { get; set; }

        public bool AutoPaymentEnabled { get; set; }

        public RatesFormatDto RatesFormat { get; set; }

        public bool UseAutoPaymentSingle { get; set; }
    }
}

