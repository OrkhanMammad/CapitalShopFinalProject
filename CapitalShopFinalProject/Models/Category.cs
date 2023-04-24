namespace CapitalShopFinalProject.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public IEnumerable<Product> products { get; set; }

        public bool? IsMain { get; set; }
    }
}
