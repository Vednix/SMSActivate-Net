using SMSActivate_Net.API.Extensions;

namespace SMSActivate_Net.API.JsonResponses
{
    public class GetCountryOperators
    {
        public string Status { get; set; } = string.Empty;
        public List<CountryOperators> CountryOperators { get; set; } = new();
    }
    public class CountryOperators
    {
        public Country Country { get; set; }
        public List<string> Operators { get; set; } = new();
    }
}
