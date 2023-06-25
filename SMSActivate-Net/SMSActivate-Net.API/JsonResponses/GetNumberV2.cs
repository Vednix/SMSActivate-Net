using Newtonsoft.Json;

namespace SMSActivate_Net.API.JsonResponses
{
    public class GetNumberV2
    {
        [JsonProperty("activationId")]
        public string ActivationId { get; set; } = string.Empty;

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonProperty("activationCost")]
        public string ActivationCost { get; set; } = string.Empty;

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; } = string.Empty;

        [JsonProperty("canGetAnotherSms")]
        public bool CanGetAnotherSms { get; set; }

        [JsonProperty("activationTime")]
        public string ActivationTime { get; set; } = string.Empty;

        [JsonProperty("activationOperator")]
        public string ActivationOperator { get; set; } = string.Empty;
    }
}
