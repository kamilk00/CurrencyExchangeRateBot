using System.Globalization;
using System.Text;
using System.Text.Json;

public class AlphaVantage
{

    private const string API_URL = "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency={0}&to_currency={1}&apikey={2}";
    private readonly string apiKey;
    private readonly Dictionary<string, string> currencies;
    private readonly SMTPMailManager smtpManager;
    private readonly string recipientEmail;

    public AlphaVantage(string apiKey, Dictionary<string, string> currencies, SMTPMailManager smtpManager, string recipientEmail)
    {
        this.apiKey = apiKey;
        this.currencies = currencies;
        this.smtpManager = smtpManager;
        this.recipientEmail = recipientEmail;
    }

    public async Task<Dictionary<string, (decimal Rate, DateTime DateTime)>> GetAllExchangeRates()
    {
        Dictionary<string, (decimal, DateTime)> results = new();

        using HttpClient client = new HttpClient();
        foreach (var pair in currencies)
        {
            string url = string.Format(API_URL, pair.Key, pair.Value, apiKey);
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var parsedData = ParseExchangeRateJson(json);
                if (parsedData != null)
                    results[pair.Key] = parsedData.Value;
            }
        }

        return results;
    }

    private (decimal, DateTime)? ParseExchangeRateJson(string json)
    {
        using JsonDocument doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("Realtime Currency Exchange Rate", out JsonElement root))
        {
            decimal rate = decimal.Parse(root.GetProperty("5. Exchange Rate").GetString(), CultureInfo.InvariantCulture);
            string dateTimeString = root.GetProperty("6. Last Refreshed").GetString();

            if (DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                return (rate, dateTime);
        }

        return null;
    }

    public string FormatExchangeRatesForEmail(Dictionary<string, (decimal rate, DateTime date)> exchangeRates)
    {
        StringBuilder emailBody = new StringBuilder();
        emailBody.AppendLine("Exchange Rates Report\n");
        emailBody.AppendLine("Exchange rates:");

        foreach (var pair in exchangeRates)
        {
            string fromCurrency = pair.Key;
            string toCurrency = currencies[fromCurrency];
            decimal rate = pair.Value.rate;
            DateTime date = pair.Value.date;

            emailBody.AppendLine($"{fromCurrency} = {rate:F2} {toCurrency} ({date:yyyy-MM-dd HH:mm:ss})");
        }

        return emailBody.ToString();
    }

}