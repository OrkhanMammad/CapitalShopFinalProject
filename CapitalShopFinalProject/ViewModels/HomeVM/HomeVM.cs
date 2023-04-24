using CapitalShopFinalProject.Models;

namespace CapitalShopFinalProject.ViewModels.HomeVM
{
    public class HomeVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set;}

        public IEnumerable<Testimonial> Testimonials { get; set; }

        public IEnumerable<News> News { get; set; }

    }
}
