using Newtonsoft.Json;

namespace SMSActivate_Net.API.JsonResponses
{
    public class GetActiveActivations
    {
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("activeActivations")]
        public List<ActiveActivation>? ActiveActivations { get; set; }
    }
    public class ActiveActivation
    {
        [JsonProperty("activationId")]
        public string ActivationId { get; set; } = string.Empty;

        [JsonProperty("serviceCode")]
        public string ServiceCode { get; set; } = string.Empty;

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonProperty("activationCost")]
        public string ActivationCost { get; set; } = string.Empty;

        [JsonProperty("activationStatus")]
        public string ActivationStatus { get; set; } = string.Empty;

        [JsonProperty("smsCode")]
        public string? SmsCode { get; set; } = string.Empty;

        [JsonProperty("smsText")]
        public string? SmsText { get; set; } = string.Empty;

        [JsonProperty("activationTime")]
        public string ActivationTime { get; set; } = string.Empty;

        [JsonProperty("discount")]
        public string Discount { get; set; } = string.Empty;

        [JsonProperty("repeated")]
        public string Repeated { get; set; } = string.Empty;

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; } = string.Empty;

        [JsonProperty("countryName")]
        public string CountryName { get; set; } = string.Empty;

        [JsonProperty("canGetAnotherSms")]
        public string CanGetAnotherSms { get; set; } = string.Empty;
    }
}
