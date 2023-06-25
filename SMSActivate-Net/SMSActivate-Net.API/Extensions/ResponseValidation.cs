using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SMSActivate_Net.API.Extensions
{
    public class ResponseValidation
    {
        public enum DefinitionTypes
        {
            JSON = 0,

            ACCESS_BALANCE,
            BAD_KEY,
            ERROR_SQL,
            OPERATORS_NOT_FOUND,
            NO_ACTIVATIONS,
            BAD_ACTION,
            NO_NUMBERS,
            WRONG_MAX_PRICE,
            BAD_SERVICE,
            BANNED, //BANNED:'YYYY-m-d H-i-s' - time for which the account is blocked
            WRONG_EXCEPTION_PHONE,
            NO_BALANCE_FORWARD,
            NO_BALANCE,
            ACCESS_CANCEL,
            WRONG_ACTIVATION_ID,

            /* getStatus (API.GetActivationStatus) */
            STATUS_WAIT_CODE,
            STATUS_WAIT_RETRY,
            STATUS_WAIT_RESEND,
            STATUS_CANCEL,
            STATUS_OK,

            /* setStatus (API.SetActivationStatus) */
            BAD_STATUS,
            ACCESS_READY,
            ACCESS_RETRY_GET, 
            ACCESS_ACTIVATION,

            WHATSAPP_NOT_AVAILABLE,
            SERVICE_NOT_AVAILABLE_THIS_COUNTRY,

            UNKNOW = 666
        }
        public static (DefinitionTypes type, string? message) GetValidResponse(string r)
        {
            var response = r.ToUpper();

            switch (response)
            {
                case var _ when response.Contains(DefinitionTypes.ACCESS_BALANCE.ToString()):
                    return (DefinitionTypes.ACCESS_BALANCE, $"{response.Split(':')[1].Replace(".", ",")}");

                case var _ when response.Contains(DefinitionTypes.BAD_KEY.ToString()):
                    return (DefinitionTypes.BAD_KEY, "invalid API key");

                case var _ when response.Contains(DefinitionTypes.ERROR_SQL.ToString()):
                    return (DefinitionTypes.ERROR_SQL, "sql-server error");

                case var _ when response.Contains(DefinitionTypes.OPERATORS_NOT_FOUND.ToString()):
                    return (DefinitionTypes.OPERATORS_NOT_FOUND, "no records found (e.g. non-existent country transferred)");

                case var _ when response.Contains(DefinitionTypes.NO_ACTIVATIONS.ToString()):
                    return (DefinitionTypes.NO_ACTIVATIONS, "entries not found (no active activations)");

                case var _ when response.Contains(DefinitionTypes.BAD_ACTION.ToString()):
                    return (DefinitionTypes.BAD_ACTION, "incorrect action, check for requested action query");

                case var _ when response.Contains(DefinitionTypes.NO_NUMBERS.ToString()):
                    return (DefinitionTypes.NO_NUMBERS, "no numbers available at the moment, try again later");

                case var _ when response.Contains(DefinitionTypes.WRONG_MAX_PRICE.ToString()):
                    if (Extra.IsValidJson(r))
                    {
                        var minAmount = JObject.Parse(r)["info"]?["min"]?.Value<int>();
                        var pMinAmount = $"{(minAmount == 0 ? "UNKNOW" : minAmount)}";
                        return (DefinitionTypes.WRONG_MAX_PRICE, $"wrong max price (min: {pMinAmount})");
                    }
                    return (DefinitionTypes.WRONG_MAX_PRICE, "wrong max price");

                case var _ when response.Contains(DefinitionTypes.BAD_SERVICE.ToString()):
                    return (DefinitionTypes.BAD_SERVICE, "incorrect service name");

                case var _ when response.Contains(DefinitionTypes.BANNED.ToString()):
                    return (DefinitionTypes.BANNED, $"{r.Split(':')[1]}");

                case var _ when response.Contains(DefinitionTypes.WRONG_EXCEPTION_PHONE.ToString()):
                    return (DefinitionTypes.WRONG_EXCEPTION_PHONE, "incorrect exclusion prefixes");

                case var _ when response.Contains(DefinitionTypes.NO_BALANCE_FORWARD.ToString()):
                    return (DefinitionTypes.NO_BALANCE_FORWARD, "not enough funds to buy call forwarding");

                case var _ when response.Contains(DefinitionTypes.NO_BALANCE.ToString()):
                    return (DefinitionTypes.NO_BALANCE, "balance has ended");

                case var _ when response.Contains(DefinitionTypes.ACCESS_CANCEL.ToString()):
                    return (DefinitionTypes.ACCESS_CANCEL, "activation cancel success");

                case var _ when response.Contains(DefinitionTypes.WRONG_ACTIVATION_ID.ToString()):
                    return (DefinitionTypes.WRONG_ACTIVATION_ID, "wrong activation id");

                case var _ when response.Contains(DefinitionTypes.STATUS_WAIT_CODE.ToString()):
                    return (DefinitionTypes.STATUS_WAIT_CODE, "waiting code");

                case var _ when response.Contains(DefinitionTypes.STATUS_WAIT_RESEND.ToString()):
                    return (DefinitionTypes.STATUS_WAIT_RESEND, "waiting code");

                case var _ when response.Contains(DefinitionTypes.STATUS_WAIT_RETRY.ToString()):
                    return (DefinitionTypes.STATUS_WAIT_RETRY, $"{r.Split(':')[1]}#past code, waiting for new code");

                case var _ when response.Contains(DefinitionTypes.STATUS_OK.ToString()):
                    return (DefinitionTypes.STATUS_OK, $"{r.Split(':')[1]}");

                case var _ when response.Contains(DefinitionTypes.STATUS_CANCEL.ToString()):
                    return (DefinitionTypes.STATUS_CANCEL, "activation canceled");

                case var _ when response.Contains(DefinitionTypes.BAD_STATUS.ToString()):
                    return (DefinitionTypes.BAD_STATUS, "incorrect or incompatible status");

                case var _ when response.Contains(DefinitionTypes.ACCESS_READY.ToString()):
                    return (DefinitionTypes.ACCESS_READY, "waiting code");

                case var _ when response.Contains(DefinitionTypes.ACCESS_RETRY_GET.ToString()):
                    return (DefinitionTypes.ACCESS_RETRY_GET, "waiting new code");

                case var _ when response.Contains("NOT_AVAILABLE"):
                    return (DefinitionTypes.SERVICE_NOT_AVAILABLE_THIS_COUNTRY, "service not available");

                case var _ when response.Contains(DefinitionTypes.WHATSAPP_NOT_AVAILABLE.ToString()):
                    return (DefinitionTypes.WHATSAPP_NOT_AVAILABLE, "whatsapp service not available this country");

                case var _ when response.Contains(DefinitionTypes.ACCESS_ACTIVATION.ToString()):
                    return (DefinitionTypes.ACCESS_ACTIVATION, "number disposed, code success");

                case var _ when Extra.IsValidJson(r):
                    return (DefinitionTypes.JSON, JsonConvert.SerializeObject(r, Formatting.Indented));

                default:
                    return (DefinitionTypes.UNKNOW, "response no match know results");
            }
        }
    }
}
