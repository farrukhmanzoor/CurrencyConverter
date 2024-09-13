using CurrencyConverter.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

[ApiController]
[Route("api/[controller]")]
public class ExchangeRateController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;
    private ILogger _log;
    public ExchangeRateController(ILogger<ExchangeRateController> logger, IExchangeRateService exchangeRateService)
    {
        this._exchangeRateService = exchangeRateService;
        this._log = logger;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
    {
        return await MethodExecutorAsync(
            async () =>
            {
                var rates = await _exchangeRateService.GetLatestRatesAsync(baseCurrency);
                return Ok(rates);
            });
    }

    [HttpGet("convert")]
    public async Task<IActionResult> ConvertCurrency([FromQuery] string fromCurrency, [FromQuery] string toCurrency, [FromQuery] decimal amount)
    {
        return await MethodExecutorAsync(
            async () =>
            {
                var result = await _exchangeRateService.ConvertCurrencyAsync(fromCurrency, toCurrency, amount);
                return Ok(result);
            });
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistoricalRates([FromQuery] string baseCurrency, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {

        return await MethodExecutorAsync(
            async () =>
            {
                var result = await _exchangeRateService.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, page, pageSize);
                return Ok(result);
            });
    }




    #region Private Methods 
    //to centerlize exception handing and response creation


    private async Task<IActionResult> MethodExecutorAsync(Func<Task<IActionResult>> methodHandle)
    {
        try
        {
            return await methodHandle.Invoke();
        }

        catch (InvalidOperationException op)
        {

            LogException(op);
            return BadRequest(op.Message);

        }
        catch (HttpRequestException http)
        {
            LogException(http);
            return BadRequest(http.Message);
        }
        catch (ArgumentNullException arg)
        {
            LogException(arg);
            return BadRequest(arg.Message);
        }

        catch (Exception ex)
        {
            LogException(ex);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Content = "An error occurred while processing the request.",
                ContentType = "text/plain"
            };

        }

    }



    private void LogException(Exception ox)
    {
        _log.LogDebug("Exception:");
        _log.LogDebug("Exception Message: " + ox.Message);
        _log.LogDebug("Inner Exception: " + ox.InnerException);
        _log.LogDebug("Exception Object: " + ox);
    }

    #endregion
}