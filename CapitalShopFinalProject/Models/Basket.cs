namespace CapitalShopFinalProject.Models
{
    public class Basket : BaseEntity
    {
        
        public string? Image { get; set; }

        public string? Title { get; set; }

        public double? DiscountedPrice { get; set; }
       public int Count { get; set; }

        public int? ProductId { get; set; }

        public Product? Product{ get; set; }

        public string? UserId { get; set; }

        public AppUser? User { get; set; }




    }
}
