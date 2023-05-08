using System.ComponentModel.DataAnnotations;

namespace CapitalShopFinalProject.Models
{
    public class Review : BaseEntity
    {
        public int? ProductId { get; set; }

        public Product? Product { get; set; }

        public string? AppUserId { get; set; }

        public AppUser? AppUser { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(1000)]
        [Required]
        public string Description { get; set; }

        [Range(0, 5)]
        public int Star { get; set; }

    }
}
