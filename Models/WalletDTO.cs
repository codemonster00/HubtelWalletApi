using System.ComponentModel.DataAnnotations;

namespace HubTelWalletApi.Models
{
    public class WalletDTO
    {

    public int Id { get; set; }
    public string Name { get; set; }
    
    public int? OwnerId { get; set; }
    [Required]
    public WalletType Type { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    [Required]
    public string AccountNumber { get; set; }
    [Required]
    public AccountScheme Scheme { get; set; }
    }
}
