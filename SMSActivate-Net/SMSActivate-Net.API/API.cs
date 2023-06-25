using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SMSActivate_Net.API.Extensions;
using SMSActivate_Net.API.JsonResponses;
using System.Net;
using static SMSActivate_Net.API.Extensions.Exceptions;

namespace SMSActivate_Net.API
{
    public class API
    {
        #region Not Supported API Calls
        /* Not Supported
       
        # Any api request related to call, rental, or forwarding #

        getMultiServiceNumber => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getMultiServiceNumber&multiService=$service&multiForward=$forward&operator=$operator&ref=$ref&country=$country
        getIncomingCallStatus => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getIncomingCallStatus&activationId=$id
        getPricesVerification => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getPricesVerification&service=$service
        getCountries => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getCountries => Extensions.Country
        getAdditionalService => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getAdditionalService&service=$service&id=$id
        getExtraActivation => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getExtraActivation&activationId=$activationId
        checkExtraActivation => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=checkExtraActivation&activationId=$activationId
        createTaskForCall => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=createTaskForCall&activationId=$activationId&phone=$phone
        getOutgoingCalls => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getOutgoingCalls&date=$date&activationId=$activationId
        getRentServicesAndCountries => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getRentServicesAndCountries&rent_time=$time&operator=$operator&country=$country => Extensions.ServiceItemHelper
        getRentNumber => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getRentNumber&service=$service&rent_time=$time&operator=$operator&country=$country&url=$url
        getRentStatus => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getRentStatus&id=$id
        setRentStatus => https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=setRentStatus&id=$id&status=$status
        getRentList => http://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getRentList
        continueRentNumber => http://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=continueRentNumber&id=$id&rent_time=$time
        getContinueRentPriceNumber => http://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getContinueRentPriceNumber&id=$id
         */
        #endregion

        public static readonly Version LibraryVersion = new(1, 0, 0, 0);
        public Version _LibraryVersion => LibraryVersion;

        private string? ApiKey { get; set; }
        private RestClient restClient { get; set; } = null!;
        public API(string apiKey, WebProxy? proxy = null, TimeSpan? timeout = null)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            restClient = new(new RestClientOptions()
            {
                BaseUrl = new Uri("https://api.sms-activate.org/stubs/handler_api.php"),
                MaxTimeout = (int)(timeout?.TotalMilliseconds ?? TimeSpan.FromSeconds(15).TotalMilliseconds),
                Proxy = proxy
            });
        }

        public Task<decimal> GetBalanceAsync() => getBalanceAsync(false);
        public Task<decimal> GetBalanceAndCashBackAsync() => getBalanceAsync(true);
        private async Task<decimal> getBalanceAsync(bool cashback = false)
        {
            var action = cashback ? "getBalanceAndCashBack" : "getBalance";
            var request = new RestRequest();
            request.AddQueryParameter("api_key", ApiKey);
            request.AddQueryParameter("action", action);

            var rest = await restClient.ExecuteGetAsync(request);
            var (type, message) = ResponseValidation.GetValidResponse(rest.Content!);

            switch (type)
            {
                case ResponseValidation.DefinitionTypes.ACCESS_BALANCE:
                    var bal = decimal.Parse(message!, Extra.CoinCulture);
                    return bal;

                case ResponseValidation.DefinitionTypes.BAD_KEY:
                    throw new BadKeyException();

                case ResponseValidation.DefinitionTypes.ERROR_SQL:
                    throw new ErrorSqlException();

                default:
                    throw new UnexpectedResponseTypeException(type);
            }
        }

        public async Task<GetActiveActivations> GetActiveActivationsAsync()
        {
            var action = "getActiveActivations";
            var request = new RestRequest();
            request.AddQueryParameter("api_key", ApiKey);
            request.AddQueryParameter("action", action);

            var rest = await restClient.ExecuteGetAsync(request);
            var (type, _) = ResponseValidation.GetValidResponse(rest.Content!);

            switch (type)
            {
                case ResponseValidation.DefinitionTypes.JSON:
                    var json = JsonConvert.DeserializeObject<GetActiveActivations>(rest.Content!)!;
                    return json;

                case ResponseValidation.DefinitionTypes.NO_ACTIVATIONS:
                    throw new NoActivationsException();

                case ResponseValidation.DefinitionTypes.BAD_KEY:
                    throw new BadKeyException();

                case ResponseValidation.DefinitionTypes.ERROR_SQL:
                    throw new ErrorSqlException();

                default:
                    throw new UnexpectedResponseTypeException(type);
            }
        }

        public async Task<List<GetNumbersStatus>> GetNumbersStatusAsync(Country country = Country.Russia)
        {
            var action = "getNumbersStatus";
            var request = new RestRequest();
            request.AddQueryParameter("api_key", ApiKey);
            request.AddQueryParameter("action", action);
            request.AddQueryParameter("country", (int)country);

            var rest = await restClient.ExecuteGetAsync(request);
            var (type, _) = ResponseValidation.GetValidResponse(rest.Content!);

            switch (type)
            {
                case ResponseValidation.DefinitionTypes.JSON:
                    var parsedJson = ServiceItemHelper.ParseServiceNumberStatus(rest.Content!)!;
                    return parsedJson;

                case ResponseValidation.DefinitionTypes.BAD_KEY:
                    throw new BadKeyException();

                case ResponseValidation.DefinitionTypes.ERROR_SQL:
                    throw new ErrorSqlException();

                default:
                    throw new UnexpectedResponseTypeException(type);
            }
        }

        public Task<List<GetTopCountryByService>> GetTopCountriesByServiceAsync(ServiceItem service) => GetTopCountriesByServiceAsync(service.Code);
        public async Task<List<GetTopCountryByService>> GetTopCountriesByServiceAsync(string serviceCode)
        {
            var action = "getTopCountriesByService";
            var request = new RestRequest();
            request.AddQueryParameter("api_key", ApiKey);
            request.AddQueryParameter("action", action);
            request.AddQueryParameter("service", serviceCode);

            var rest = await restClient.ExecuteGetAsync(request);
            var (type, _) = ResponseValidation.GetValidResponse(rest.Content!);

            switch (type)
            {
                case ResponseValidation.DefinitionTypes.JSON:
                    var parsedList = new List<GetTopCountryByService>();
                    var jsonObject = JObject.Parse(rest.Content!);
                    foreach (var property in jsonObject.Properties())
                    {
                        var itemObject = property.Value;
                        var item = itemObject.ToObject<GetTopCountryByService>()!;

#pragma warning disable CS8602
#pragma warning disable CS8604
                        item.Price = decimal.Parse(itemObject["price"].Value<string>().Replace(".", ","), Extra.CoinCulture);
                        item.RetailPrice = decimal.Parse(itemObject["retail_price"].Value<string>().Replace(".", ","), Extra.CoinCulture);
#pragma warning restore CS8602
#pragma warning restore CS8604

                        parsedList.Add(item);
                    }
                    return parsedList;

                case ResponseValidation.DefinitionTypes.BAD_KEY:
                    throw new BadKeyException();

                case ResponseValidation.DefinitionTypes.ERROR_SQL:
                    throw new ErrorSqlException();

                default:
                    throw new UnexpectedResponseTypeException(type);
            }
        }

        public async Task<GetCountryOperators> GetCountryOperatorsAsync(Country? country = null)
        {
            var action = "getOperators";
            var request = new RestRequest();
            request.AddQueryParameter("api_key", ApiKey);
            request.AddQueryParameter("action", action);
            request.AddQueryParameter("country", $"{(int?)(country ?? null)}");

            var rest = await restClient.ExecuteGetAsync(request);
            var (type, _) = ResponseValidation.GetValidResponse(rest.Content!);

            switch (type)
            {
                case ResponseValidation.DefinitionTypes.JSON:
#pragma warning disable CS8602
#pragma warning disable CS8604
                    var rJson = new GetCountryOperators()
                    {
                        Status = JObject.Parse(rest.Content)["status"].Value<string>()!
                    };

                    if (country != null)
                    {
                        var countryOperatorsJson = JObject.Parse(rest.Content)["countryOperators"][$"{(int)country}"] as JArray;
                        if (countryOperatorsJson != null)
                        {
                            var countryOperators = new CountryOperators
                            {
                                Country = country ?? throw new ArgumentException("This should never occurs"),
                                Operators = new(),
                            };
                            foreach (var item in countryOperatorsJson)
                                countryOperators.Operators.Add(item.Value<string>());
                            rJson.CountryOperators.Add(countryOperators);
                        }
                        return rJson;
                    }
                    var parsedJson = JObject.Parse(rest.Content)["countryOperators"] as JObject;

                    foreach (var item in parsedJson)
                    {
                        var countryOperators = new CountryOperators
                        {
                            Country = (Country)Enum.Parse(typeof(Country), item.Key),
                            Operators = item.Value.ToObject<List<string>>() ?? new List<string>()
                        };
                        rJson.CountryOperators.Add(countryOperators);
                    }
#pragma warning restore CS8602
#pragma warning restore CS8604
                    return rJson;

                case ResponseValidation.DefinitionTypes.BAD_KEY:
                    throw new BadKeyException();

                case ResponseValidation.DefinitionTypes.ERROR_SQL:
                    throw new ErrorSqlException();

                case ResponseValidation.DefinitionTypes.OPERATORS_NOT_FOUND:
                    throw new OperatorsNotFoundException();

                default:
                    throw new UnexpectedResponseTypeException(type);
            }
        }


        public Task<GetNumberV2> GetNumberV2Async(ServiceItem service, Country country, int maxPrice = 200) => GetNumberV2Async(service.Code, country, maxPrice);
        public async Task<GetNumberV2> GetNumberV2Async(string serviceCode, Country country, int maxPrice = 200, string? _operator = null, string? _ref = null)
        {
            var action = "getNumberV2";
            var request = new RestRequest((string?)null, Method.Get);
            request.AddQueryParameter("api_key", ApiKey);
            request.AddQueryParameter("action", action);
            request.AddQueryParameter("country", (int)country);
            request.AddQueryParameter("service", serviceCode);
            request.AddQueryParameter("maxPrice", maxPrice);
            request.AddQueryParameter("operator", _operator);
            request.AddQueryParameter("ref", _ref);

            var rest = await restClient.ExecuteGetAsync(request);
            var (type, _) = ResponseValidation.GetValidResponse(rest.Content!);

            switch (type)
            {
                case ResponseValidation.DefinitionTypes.JSON:
                    var parsedJson = JsonConvert.DeserializeObject<GetNumberV2>(rest.Content!)!;
                    return parsedJson;

                case ResponseValidation.DefinitionTypes.NO_BALANCE:
                case ResponseValidation.DefinitionTypes.NO_BALANCE_FORWARD:
                case ResponseValidation.DefinitionTypes.ACCESS_BALANCE:
                    throw new NoBalanceException();

                case ResponseValidation.DefinitionTypes.BAD_KEY:
                    throw new BadKeyException();

                case ResponseValidation.DefinitionTypes.WHATSAPP_NOT_AVAILABLE:
                case ResponseValidation.DefinitionTypes.SERVICE_NOT_AVAILABLE_THIS_COUNTRY:
                    throw new ServiceNotAvailableException(ServiceItemHelper.GetService(serviceCode)!);

                case ResponseValidation.DefinitionTypes.NO_NUMBERS:
                    throw new NoNumbersException();

                default:
                    throw new UnexpectedResponseTypeException(type);
            }
        }

        public async Task<SetActivationStatusResponse> SetActivationStatusAsync(SetActivationStatus status, long activationId)
        {
            var action = "setStatus"; //https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=setStatus&status=$status&id=$id&forward=$forward
            var request = new RestRequest();
            request.AddQueryParameter("api_key", ApiKey);
            request.AddQueryParameter("action", action);
            request.AddQueryParameter("status", (int)status);
            request.AddQueryParameter("id", activationId);

            var rest = await restClient.ExecuteGetAsync(request);
            var response = ResponseValidation.GetValidResponse(rest.Content!);

            switch (response.type)
            {
                case ResponseValidation.DefinitionTypes.ACCESS_RETRY_GET:
                    return SetActivationStatusResponse.ACCESS_RETRY_GET;

                case ResponseValidation.DefinitionTypes.ACCESS_READY:
                    return SetActivationStatusResponse.ACCESS_READY;

                case ResponseValidation.DefinitionTypes.ACCESS_CANCEL:
                    return SetActivationStatusResponse.ACCESS_CANCEL;

                case ResponseValidation.DefinitionTypes.ACCESS_ACTIVATION:
                    return SetActivationStatusResponse.ACCESS_ACTIVATION;

                case ResponseValidation.DefinitionTypes.BAD_STATUS:
                    throw new BadStatusException();

                case ResponseValidation.DefinitionTypes.WRONG_ACTIVATION_ID:
                    throw new WrongActivationIdException();

                case ResponseValidation.DefinitionTypes.BAD_KEY:
                    throw new BadKeyException();

                case ResponseValidation.DefinitionTypes.ERROR_SQL:
                    throw new ErrorSqlException();

                default:
                    throw new UnexpectedResponseTypeException(response.type);
            }
        }

        public Task<Dictionary<Country, List<ServiceItemPrice>>> GetServiceCountryPricesAsync(ServiceItem? service = null, Country? country = null) => GetServiceCountryPricesAsync(service?.Code, country);
        public async Task<Dictionary<Country, List<ServiceItemPrice>>> GetServiceCountryPricesAsync(string? serviceCode = null, Country? country = null)
        {
            var action = "getPrices"; //https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getPrices&service=$service&country=$country
            var request = new RestRequest();
            request.AddQueryParameter("api_key", ApiKey);
            request.AddQueryParameter("action", action);
            request.AddQueryParameter("service", serviceCode);
            request.AddQueryParameter("country", $"{(int?)(country ?? null)}");

            var rest = await restClient.ExecuteGetAsync(request);
            var response = ResponseValidation.GetValidResponse(rest.Content!);

            switch (response.type)
            {
                case ResponseValidation.DefinitionTypes.JSON:
                    var jsonObject = JObject.Parse(rest.Content!);
                    var getPrices = new Dictionary<Country, List<ServiceItemPrice>>();

                    foreach (var property in jsonObject.Properties())
                    {
                        var _country = (Country)int.Parse(property.Name);
                        var serviceItemPrices = property.Value.ToObject<Dictionary<string, ServiceItemPrice>>();

                        if (serviceItemPrices != null)
                            foreach (var serviceItemPriceEntry in serviceItemPrices)
                            {
                                var serviceItem = ServiceItemHelper.GetService(serviceItemPriceEntry.Key);
                                var serviceItemPrice = serviceItemPriceEntry.Value;
                                serviceItemPrice.Code = serviceItem?.Code ?? "NULL";
                                serviceItemPrice.Name = serviceItem?.Name ?? "NULL";

                                if (!getPrices.ContainsKey(_country))
                                    getPrices[_country] = new List<ServiceItemPrice>();

                                getPrices[_country].Add(serviceItemPrice);
                            }
                    }
                    return getPrices;

                case ResponseValidation.DefinitionTypes.BAD_KEY:
                    throw new BadKeyException();

                case ResponseValidation.DefinitionTypes.ERROR_SQL:
                    throw new ErrorSqlException();

                default:
                    throw new UnexpectedResponseTypeException(response.type);
            }
        }

        public async Task<(GetActivationStatus status, string? code)> GetActivationStatusAsync(long activationId)
        {
            var action = "getStatus"; //https://api.sms-activate.org/stubs/handler_api.php?api_key=$api_key&action=getStatus&id=$id
            var request = new RestRequest();
            request.AddQueryParameter("api_key", ApiKey);
            request.AddQueryParameter("action", action);
            request.AddQueryParameter("id", activationId);

            var rest = await restClient.ExecuteGetAsync(request);
            var response = ResponseValidation.GetValidResponse(rest.Content!);

            switch (response.type)
            {
                case ResponseValidation.DefinitionTypes.STATUS_WAIT_CODE:
                    return (GetActivationStatus.STATUS_WAIT_CODE, response.message);

                case ResponseValidation.DefinitionTypes.STATUS_WAIT_RESEND:
                    return (GetActivationStatus.STATUS_WAIT_RESEND, response.message);

                case ResponseValidation.DefinitionTypes.STATUS_WAIT_RETRY:
                    return (GetActivationStatus.STATUS_WAIT_RETRY, response.message);

                case ResponseValidation.DefinitionTypes.STATUS_OK: //success
                    return (GetActivationStatus.STATUS_OK, response.message);

                case ResponseValidation.DefinitionTypes.STATUS_CANCEL:
                    return (GetActivationStatus.STATUS_CANCEL, response.message);

                case ResponseValidation.DefinitionTypes.WRONG_ACTIVATION_ID:
                    throw new WrongActivationIdException();

                case ResponseValidation.DefinitionTypes.BAD_KEY:
                    throw new BadKeyException();

                case ResponseValidation.DefinitionTypes.ERROR_SQL:
                    throw new ErrorSqlException();

                default:
                    throw new UnexpectedResponseTypeException(response.type);
            }
        }
    }
}
