namespace SMSActivate_Net.API.Extensions
{
    public class Exceptions
    {
        public class BadKeyException : Exception
        {
            public BadKeyException() : base("Provided ApiKey is invalid.") { }
        }
        public class ErrorSqlException : Exception
        {
            public ErrorSqlException() : base("Api report sql-server error.") { }
        }
        public class ServiceNotAvailableException : Exception
        {
            public ServiceNotAvailableException(ServiceItem service) : base($"Service '{service.Name} ({service.Code})' not available at this country.") { }
        }
        public class InvalidJsonException : Exception
        {
            public InvalidJsonException() : base("Parsed string is not a valid JSON.") { }
        }
        public class NoNumbersException : Exception
        {
            public NoNumbersException() : base("No numbers, try again later.") { }
        }
        public class OperatorsNotFoundException : Exception
        {
            public OperatorsNotFoundException() : base("no records found (e.g. non-existent country transferred)") { }
        }
        public class WrongActivationIdException : Exception
        {
            public WrongActivationIdException() : base("wrong activation id") { }
        }
        public class BadStatusException : Exception
        {
            public BadStatusException() : base("invalid or incorrect 'status' has been sent to Api") { }
        }
        public class NoActivationsException : Exception
        {
            public NoActivationsException() : base("entries not found (no active activations)") { }
        }
        public class NoBalanceException : Exception
        {
            public NoBalanceException() : base("balance is low") { }
        }
        public class UnexpectedResponseTypeException : Exception
        {
            public UnexpectedResponseTypeException(ResponseValidation.DefinitionTypes type) : base($"Type '{type}' was not expected.") { }
        }




        //public class ApiKeyInvalidException : Exception
        //{
        //    public ApiKeyInvalidException() { }
        //    public ApiKeyInvalidException(string msg) : base(msg) { }
        //    public ApiKeyInvalidException(string msg, Exception innerException) : base(msg, innerException) { }
        //}
    }
}
