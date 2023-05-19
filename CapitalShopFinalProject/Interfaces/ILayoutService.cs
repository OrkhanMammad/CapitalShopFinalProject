using CapitalShopFinalProject.DataAccessLayer;
using CapitalShopFinalProject.Models;
using CapitalShopFinalProject.ViewModels.BasketVM;
using Newtonsoft.Json;

namespace CapitalShopFinalProject.Interfaces
{
    public interface ILayoutService
    {
        Task<IEnumerable<BasketVM>> GetBasket();
    }
}