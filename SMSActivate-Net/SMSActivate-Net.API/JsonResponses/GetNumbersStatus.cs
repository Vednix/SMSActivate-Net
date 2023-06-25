using SMSActivate_Net.API.Extensions;

namespace SMSActivate_Net.API.JsonResponses
{
    public class GetNumbersStatus
    {
        public ServiceItem? Service { get; set; }
        public bool Forwarding { get; set; }
        public int Quantity { get; set; }
    }
}
