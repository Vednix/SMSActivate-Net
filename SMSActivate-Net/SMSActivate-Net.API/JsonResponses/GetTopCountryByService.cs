using SMSActivate_Net.API.Extensions;

namespace SMSActivate_Net.API.JsonResponses
{
    public class GetTopCountryByService
    {
        public Country Country { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public decimal RetailPrice { get; set; }
    }
}
