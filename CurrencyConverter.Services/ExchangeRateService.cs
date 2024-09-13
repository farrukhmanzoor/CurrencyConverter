using CurrencyConverter.Contracts;
using CurrencyConverter.Models;
using System.Net.Http.Json;
using System.Threading;

namespace CurrencyConverter.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly ResiliencePipelineProvider _retryPolicy;

        public ExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _retryPolicy = new ResiliencePipelineProvider();
        }

        public async Task<ExchangeRateModel> GetLatestRatesAsync(string baseCurrency)
        {
            var response = await _retryPolicy.ExecuteAsync(
           async (ct) => await _httpClient.GetAsync($"https://api.frankfurter.app/latest?base={baseCurrency}", ct)
           , CancellationToken.None);

            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<ExchangeRateModel>();
            return data;
        }

        public async Task<ExchangeRateModel> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            // Exclude specific currencies for conversion
            var excludedCurrencies = new[] { "TRY", "PLN", "THB", "MXN" };
            if (excludedCurrencies.Contains(toCurrency.ToUpper()) || excludedCurrencies.Contains(fromCurrency.ToUpper()))
            {
                throw new InvalidOperationException("An error has occured."); // Handle Bad Request scenario
            }

            var response = await _retryPolicy.ExecuteAsync(
            async (ct) => await _httpClient.GetAsync($"https://api.frankfurter.app/latest?from={fromCurrency}&to={toCurrency}&amount={amount}", ct),
            CancellationToken.None);

            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<ExchangeRateModel>();
           
            return data;
        }

        public async Task<ExchangeRateHistoryModel> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            var response = await _retryPolicy.ExecuteAsync(
            async (ct) => await _httpClient.GetAsync($"https://api.frankfurter.app/{startDate.ToString("yyyy-MM-dd")}..{endDate.ToString("yyyy-MM-dd")}?base={baseCurrency}", ct)
            ,CancellationToken.None);

            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<ExchangeRateHistoryModel>();

            // Implement pagination logic
            data.Rates = data?.Rates.Skip((page - 1) * pageSize).Take(pageSize).ToDictionary();
            return data;
        }
    }
}
