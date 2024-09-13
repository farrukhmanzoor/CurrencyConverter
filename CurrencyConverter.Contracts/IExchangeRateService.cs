using CurrencyConverter.Models;
using System.Runtime.CompilerServices;

namespace CurrencyConverter.Contracts
{
    public interface IExchangeRateService
    {
        Task<ExchangeRateModel> GetLatestRatesAsync(string baseCurrency);
        Task<ExchangeRateModel> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount);
        Task<ExchangeRateHistoryModel> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int page, int pageSize);
    }


}

namespace CurrencyConverter.Models
{
    public class ExchangeRateModel
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
        public Dictionary<DateOnly,Dictionary<string,decimal>>   RateHistory
        { get; set; }

        public decimal Amount { get; set; }
    }


    public class ExchangeRateHistoryModel
    {
        public double Amount { get; set; }
        public string Base { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
    }
}
