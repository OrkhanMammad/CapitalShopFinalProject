namespace CapitalShopFinalProject.Models
{
    public class ProductType : BaseEntity
    {
        public string Name { get; set; }

        public bool? IsMain { get; set; }

        public IEnumerable<Product> products { get; set; }
    }
}
