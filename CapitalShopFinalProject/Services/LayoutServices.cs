using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.BasketVM;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CapitalShopFinalProject.Services
{
    public class LayoutServices
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutServices(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<BasketVM>> GetBaskets()
        {
            string basket = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;


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
