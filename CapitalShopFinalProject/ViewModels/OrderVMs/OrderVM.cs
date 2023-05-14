using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.BasketVM;

namespace CapitalShopFinalProject.ViewModels.OrderVMs

{
    public class OrderVM
    {
        public List<BasketVM.BasketVM>? BasketVMs { get; set; }

        public Order Order { get; set; }

    }
}
