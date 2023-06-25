namespace SMSActivate_Net.API.Extensions
{
    /// <summary>
    /// API Chronology Actions:
    /// 1 - Inform about the readiness of the number (SMS sent to the number)
    /// 3 - Request another code (free)
    /// 6 - Complete activation *
    /// 8 - Inform that the number has been used and cancel the activation
    /// 
    /// 
    /// Simple logic of API chronology:
    ///
    /// Getting a number using the getNumber method, then the following actions are available:
    /// 8 - Cancel the activation (if the number does not match you)
    /// 1 - Report that SMS has been sent (optional)
    ///
    /// To activation with status 1:
    /// 8 - Cancel activation
    ///
    /// Immediately after receiving the code:
    /// 3 - Request another SMS
    /// 6 - Confirm SMS code and complete activation
    ///
    /// To activation with status 3:
    /// 6 - Confirm SMS code and complete activation
    /// </summary>
    public enum SetActivationStatus
    {
        ClientSmsSent = 1,
        RequestAnotherSmsFree = 3,
        CompleteActivation = 6,
        CancelActivation = 8
    }

    public enum SetActivationStatusResponse
    {
        BAD_STATUS,
        ACCESS_READY, //ClientSmsSent called, waiting for code
        ACCESS_CANCEL, //CancelActivation called, number canceled
        ACCESS_RETRY_GET, //RequestAnotherSmsFree called, waiting for new code
        ACCESS_ACTIVATION, //CompleteActivation called, number ended
    }
}
