using CapitalShopFinalProject.Models;

namespace CapitalShopFinalProject.ViewModels.ShopVM
{
    public class ShopVM
    {
      public  IEnumerable<Product> Products { get; set; }
       public IEnumerable<Category> Categories { get; set; }

      public  IEnumerable<ProductType> ProductTypes { get; set; }



    }
}
