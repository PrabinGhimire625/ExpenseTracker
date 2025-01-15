public class CurrencyService
{
    private readonly Dictionary<string, decimal> _conversionRates = new()
    {
        { "USD", 1.0m },
        { "NPR", 120.0m }
    };

    private readonly Dictionary<string, string> _currencySymbols = new()
    {
        { "USD", "$" },
        { "NPR", "₨" }
    };

    public string SelectedCurrency { get; private set; } = "USD"; // Default currency
    public string SelectedCurrencySymbol => _currencySymbols.ContainsKey(SelectedCurrency)
        ? _currencySymbols[SelectedCurrency]
        : string.Empty;

    public decimal ConversionRateToUSD => _conversionRates[SelectedCurrency];

    public decimal GetConversionRate(string currency)
    {
        return _conversionRates.ContainsKey(currency) ? _conversionRates[currency] : 1.0m;
    }

    public void SetCurrency(string currency)
    {
        if (_conversionRates.ContainsKey(currency))
        {
            SelectedCurrency = currency;
        }
    }
}
