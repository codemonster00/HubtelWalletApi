using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HubTelWalletApi.Models
{
    public class AppUser
    {
   
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; }= string.Empty;

        public string Phone { get; set; } = string.Empty;


        
        public virtual ICollection<Wallet>? Wallets { get; set; }





    }
}
