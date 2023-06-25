using SMSActivate_Net.API.Extensions;
using SMSActivate_Net.API.JsonResponses;

namespace SMSActivate_Net.API
{
    internal class Examples
    {
        static async Task Main(string[] args)
        {
            var api = new API("apikey");
            Console.WriteLine($"LibraryVersion: {api._LibraryVersion}");
            try
            {
                Console.WriteLine($"Balance: {await api.GetBalanceAsync()}");

                var testService = ServiceItemHelper.FindSimilarServiceName("telegram");

                var GetServiceCountryPrices = await api.GetServiceCountryPricesAsync(testService);
                Console.WriteLine($"GetServiceCountryPrices: {GetServiceCountryPrices.Count} items on list.");

                try
                {
                    var GetActiveActivations = await api.GetActiveActivationsAsync();
                    Console.WriteLine($"GetActiveActivations: {GetActiveActivations.Status}");
                }
                catch (Exceptions.NoActivationsException)
                {
                    Console.WriteLine($"GetActiveActivations: NoActivationsException");
                }

                var GetNumbersStatus = await api.GetNumbersStatusAsync(Country.USA);
                Console.WriteLine($"GetNumbersStatus: {GetNumbersStatus.Count} items on list.");
#pragma warning disable CS8604 
                var GetTopCountriesByService = await api.GetTopCountriesByServiceAsync(testService);
#pragma warning restore CS8604
                Console.WriteLine($"GetTopCountriesByService: {GetTopCountriesByService.Count} items on list.");

                var GetCountryOperators = await api.GetCountryOperatorsAsync(Country.USA);
                Console.WriteLine($"GetCountryOperators: {GetCountryOperators.CountryOperators.Count} items on list.");

                var GetNumberV2 = await api.GetNumberV2Async(testService, Country.USA);
                Console.WriteLine($"GetNumberV2: +{GetNumberV2.PhoneNumber} ({GetNumberV2.ActivationOperator})");
                var numberActivationId = long.Parse(GetNumberV2.ActivationId);
                try
                {
                    var GetActivationStatusAsync = await api.GetActivationStatusAsync(numberActivationId);
                    Console.WriteLine($"GetActivationStatusAsync: {GetActivationStatusAsync.status}{(GetActivationStatusAsync.code == null ? null : $":{GetActivationStatusAsync.code}")}");
                }
                catch (Exceptions.WrongActivationIdException)
                {
                    Console.WriteLine($"GetActivationStatusAsync: WrongActivationIdException");
                }

                var SetActivationStatus = await api.SetActivationStatusAsync(Extensions.SetActivationStatus.CancelActivation, numberActivationId);
                Console.WriteLine($"SetActivationStatus: {SetActivationStatus}.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("Example succeeded!");
            }
            Console.ReadKey();
        }
    }
}