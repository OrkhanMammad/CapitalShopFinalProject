namespace CapitalShopFinalProject.Models
{
    public class ProductImage : BaseEntity
    {
        public string Image { get; set; }

        public Product product { get; set; }

        public int ProductId { get; set; }

    }
}
