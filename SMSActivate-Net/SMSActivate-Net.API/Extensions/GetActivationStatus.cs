namespace SMSActivate_Net.API.Extensions
{
    public enum GetActivationStatus
    {
        STATUS_WAIT_CODE,
        STATUS_WAIT_RETRY,
        STATUS_WAIT_RESEND,
        STATUS_CANCEL,
        STATUS_OK,
    }
}
