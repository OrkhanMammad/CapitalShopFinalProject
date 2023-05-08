using CapitalShopFinalProject.Enums;

namespace CapitalShopFinalProject.Models
{
    public class Order : BaseEntity
    {
        public int? No { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? UserId { get; set; }

        public AppUser? User { get; set; }

        public string? Country { get; set;}

        public string? State { get; set; }

        public string? City { get; set; }

        public string? PostalCode { get; set; }

        public string? AddressLine { get; set; }

        public OrderType? Status { get; set; }

        public string? Comment { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set;}


    }
}
