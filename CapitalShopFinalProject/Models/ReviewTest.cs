using System.ComponentModel.DataAnnotations;

namespace CapitalShopFinalProject.Models
{
    public class ReviewTest : BaseEntity
    {
        [StringLength(2000)]
        public string Content { get; set; }

        [Range(0, 5)]
        public int Star { get; set; }

        public int? ProductId { get; set; }

        public Product? Product { get; set; }

        public string? AppUserId { get; set; }

        public AppUser? AppUser { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(100)]
        public string Name { get; set; }


        



    }
}
