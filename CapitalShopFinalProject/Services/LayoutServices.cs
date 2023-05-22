using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Interfaces;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.BasketVM;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CapitalShopFinalProject.Services
{
    public class LayoutServices : ILayoutService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutServices(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<IEnumerable<BasketVM>> GetBaskets()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BasketVM>> GetBasket()
        {
            string basket = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = new List<BasketVM>();


            if (!string.IsNullOrEmpty(basket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _appDbContext.Products
                        .FirstOrDefaultAsync(p => p.ID == basketVM.Id && p.IsDeleted == false);
                    if (product != null)
                    {
                       
                        basketVM.DiscountedPrice = product.DiscountedPrice;
                        basketVM.Title = product.Title;
                        basketVM.Image = product.MainImage;
                        basketVM.Id = basketVM.Id;

                    }
                }
            }


            return basketVMs;
        }

    }
}
