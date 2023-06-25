using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace SMSActivate_Net.API.Extensions
{
    public class Extra
    {
        public static CultureInfo CoinCulture = new("ru-RU"); //russian rubles
        public static bool IsValidJson(string jsonString)
        {
            try
            {
                JToken.Parse(jsonString);
                return true;
            }
            catch (JsonReaderException) { }
            return false;
        }
    }
}
