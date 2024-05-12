using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HubTelWalletApi.Models
{
    public class Wallet
    {

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }


        [ForeignKey("Owner")]
        public int OwnerId {  get; set; }
        [JsonIgnore]
        public virtual AppUser? Owner { get; set; }

        public WalletType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string AccountNumber { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        public AccountScheme Scheme { get; set; }

      //  public WalletType Type { get; set; }
        public void SoftDelete()
        {
            IsDeleted = true;
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WalletType
    {
        [Description("Card")]
        Card,

        [Description("Momo")]
        Momo
       
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountScheme
    {
        [Description("Visa")]
        Visa,
        [Description("Mastercard")]
        Mastercard,
        [Description("Mtn")]
        Mtn,
        [Description("Vodafone")]
        Vodafone,
        [Description("Airteltigo")]
        AirtelTigo
    }


}