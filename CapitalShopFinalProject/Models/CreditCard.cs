using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace CapitalShopFinalProject.Models
{
    public class CreditCard
    {
        
        public int Id { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        [StringLength(16)]
        
        public string CardNumber { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string CardHolder { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string ExpDate { get; set; }
        [System.ComponentModel.DataAnnotations.Required]

        public string CVV { get; set; }
        
        public double? Balance { get; set; }

        public IEnumerable<Order>? Orders { get; set; }





    }
}
