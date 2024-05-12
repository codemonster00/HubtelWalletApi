namespace HubTelWalletApi.Models
{
    public class OtpData
    {

        public string RequestId { get; set; } = string.Empty;

        public string Otp { get; set; }= string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}