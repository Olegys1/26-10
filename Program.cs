using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var exchangeRates = new Dictionary<string, Dictionary<string, double>>
{
    {
        "USD", new Dictionary<string, double>
        {
            { "USD", 1.0 },
            { "EUR", 0.92 },
            { "UAH", 41.20 },
            { "GBP", 0.77 }
        }
    },
    {
        "EUR", new Dictionary<string, double>
        {
            { "USD", 1.09 },
            { "EUR", 1.0 },
            { "UAH", 44.78 },
            { "GBP", 0.84 }
        }
    },
    {
        "UAH", new Dictionary<string, double>
        {
            { "USD", 0.024 },
            { "EUR", 0.022 },
            { "UAH", 1.0 },
            { "GBP", 0.019 }
        }
    },
    {
        "GBP", new Dictionary<string, double>
        {
            { "USD", 1.30 },
            { "EUR", 1.19 },
            { "UAH", 53.50 },
            { "GBP", 1.0 }
        }
    }
};

app.MapGet("/currencies", () => 
    Results.Ok(exchangeRates.Keys));

app.MapGet("/exchangeRate/{from}/{to}", (string from, string to) =>
{
    var fromCurrency = from.ToUpper();
    var toCurrency = to.ToUpper();

    if (!exchangeRates.ContainsKey(fromCurrency))
    {
        return Results.BadRequest(new { error = $"Currency '{fromCurrency}' is not supported." });
    }

    if (!exchangeRates[fromCurrency].ContainsKey(toCurrency))
    {
        return Results.BadRequest(new { error = $"Exchange rate from '{fromCurrency}' to '{toCurrency}' is not available." });
    }

    var rate = exchangeRates[fromCurrency][toCurrency];
    return Results.Ok(new { from = fromCurrency, to = toCurrency, rate });
});

app.MapGet("/convertCurrency/{from}/{to}/{amount:double}", (string from, string to, double amount) =>
{
    var fromCurrency = from.ToUpper();
    var toCurrency = to.ToUpper();

    if (amount < 0)
    {
        return Results.BadRequest(new { error = "Amount cannot be negative." });
    }

    if (!exchangeRates.ContainsKey(fromCurrency))
    {
        return Results.BadRequest(new { error = $"Currency '{fromCurrency}' is not supported." });
    }

    if (!exchangeRates[fromCurrency].ContainsKey(toCurrency))
    {
        return Results.BadRequest(new { error = $"Exchange rate from '{fromCurrency}' to '{toCurrency}' is not available." });
    }

    var rate = exchangeRates[fromCurrency][toCurrency];
    var convertedAmount = amount * rate;

    return Results.Ok(new 
    { 
        from = fromCurrency, 
        to = toCurrency, 
        amount, 
        rate, 
        convertedAmount 
    });
});

app.Run();
