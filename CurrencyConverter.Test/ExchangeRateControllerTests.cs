using Microsoft.AspNetCore.Mvc;
using Moq;
using CurrencyConverter.Contracts;
using CurrencyConverter.Models;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter.Tests
{
    [TestClass]
    public class ExchangeRateControllerTests
    {
        private Mock<IExchangeRateService> _mockExchangeRateService;
        private Mock<ILogger<ExchangeRateController>> _mockLogger;
        private ExchangeRateController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockExchangeRateService = new Mock<IExchangeRateService>();
            _mockLogger = new Mock<ILogger<ExchangeRateController>>();
            _controller = new ExchangeRateController(_mockLogger.Object,_mockExchangeRateService.Object);
        }

        [TestMethod]
        public async Task GetLatestRates_ReturnsOkResult_WithExchangeRateModel()
        {
            // Arrange
            var baseCurrency = "USD";
            var exchangeRateModel = new ExchangeRateModel
            {
                Base = "USD",
                Date = DateTime.Now,
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } }
            };
            _mockExchangeRateService.Setup(s => s.GetLatestRatesAsync(baseCurrency))
                                    .ReturnsAsync(exchangeRateModel);

            // Act
            var result = await _controller.GetLatestRates(baseCurrency);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(exchangeRateModel, okResult.Value);
        }

        [TestMethod]
        public async Task ConvertCurrency_ReturnsOkResult_WithConversionResult()
        {
            // Arrange
            var fromCurrency = "USD";
            var toCurrency = "EUR";
            var amount = 100m;
            var conversionResult =  new ExchangeRateModel
            {
                Base = "USD",
                Date = DateTime.Now,
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } }
            };
            _mockExchangeRateService.Setup(s => s.ConvertCurrencyAsync(fromCurrency, toCurrency, amount))
                                    .ReturnsAsync(conversionResult);

            // Act
            var result = await _controller.ConvertCurrency(fromCurrency, toCurrency, amount);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(conversionResult, okResult.Value);
        }

        [TestMethod]
        public async Task ConvertCurrency_ReturnsBadRequest_WhenConversionIsNotAllowed()
        {
            // Arrange
            var fromCurrency = "USD";
            var toCurrency = "TRY";
            var amount = 100m;
            _mockExchangeRateService.Setup(s => s.ConvertCurrencyAsync(fromCurrency, toCurrency, amount))
                                    .ReturnsAsync((ExchangeRateModel)null);

            // Act
            var result = await _controller.ConvertCurrency(fromCurrency, toCurrency, amount);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNull(badRequestResult);
            
        }

        [TestMethod]
        public async Task GetHistoricalRates_ReturnsOkResult_WithExchangeRateHistoryModel()
        {
            // Arrange
            var baseCurrency = "USD";
            var startDate = DateTime.Now.AddDays(-10);
            var endDate = DateTime.Now;
            var page = 1;
            var pageSize = 10;
            var historicalRatesModel = new ExchangeRateHistoryModel
            {
                Rates = new Dictionary<string, Dictionary<string, decimal>>
                {
                    { DateTime.Now.Date.ToString(), new Dictionary<string, decimal> { { "EUR", 0.85m } } }
                }
            };
            _mockExchangeRateService.Setup(s => s.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, page, pageSize))
                                    .ReturnsAsync(historicalRatesModel);

            // Act
            var result = await _controller.GetHistoricalRates(baseCurrency, startDate, endDate, page, pageSize);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(historicalRatesModel, okResult.Value);
        }
    }
}
