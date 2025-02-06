using System.Text.Json;

class CurrencyExchangeBot
{
    static async Task Main(string[] args)
    {
        string apiKey = Environment.GetEnvironmentVariable("ALPHAVANTAGE_API_KEY"); 
        string smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER");
        int smtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out int port) ? port : 587;
        string emailUser = Environment.GetEnvironmentVariable("EMAIL_USER");
        string emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        string recipientEmail = Environment.GetEnvironmentVariable("RECIPIENT_EMAIL");

        var currencyPairs = LoadCurrencyPairs("currencies.json");

        SMTPMailManager smtpManager = new SMTPMailManager(smtpServer, smtpPort, emailUser, emailPassword);
        AlphaVantage alphaVantage = new AlphaVantage(apiKey, currencyPairs, smtpManager, recipientEmail);

        var exchangeRates = await alphaVantage.GetAllExchangeRates();

        string emailBody = alphaVantage.FormatExchangeRatesForEmail(exchangeRates);
        smtpManager.SendEmail("Exchange Rates Report", emailBody, recipientEmail);
    }

    static Dictionary<string, string> LoadCurrencyPairs(string filePath)
        => JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(filePath));
    
}