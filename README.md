# CurrencyExchangeRateBot

This project fetches currency exchange rates from the Alpha Vantage API and sends exchange rate reports via Gmail SMTP.

## Features
* Fetches currency exchange rates from Alpha Vantage

* Sends email with Exchange Rate Reports

* Automated execution via GitHub Actions (3 times per day)

## Configuration
Configure the following environment variables (e.g., in .env or GitHub Secrets):
ALPHAVANTAGE_API_KEY: Alpha Vantage API key

SMTP_SERVER: SMTP server address

SMTP_PORT: SMTP server port

EMAIL_USER: Email sender address

EMAIL_PASSWORD: Email sender password

RECIPIENT_EMAIL: Alert recipient email

Edit currencies.json to specify the currency pairs to track:
```json
{
  "USD": "PLN",
  "EUR": "USD"
}
```
