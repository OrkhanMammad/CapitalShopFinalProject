using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CapitalShopFinalProject.Models
{
    public class Product : BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }

        [Column(TypeName = "money")]
        public double? Price { get; set; }

        [Column(TypeName = "money")]
        public double DiscountedPrice { get; set; }

        public int Count { get; set; }
        [StringLength(5000)]
        public string? Description { get; set; }
        [StringLength(1000)]
        public string? ShortDescription { get; set; }


        public IEnumerable<ProductImage> ProductImages { get; set; }

        public string MainImage { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int ProductTypeId { get; set; }

        public ProductType ProductType { get; set; }

        public bool? IsTrending { get; set; }

        public IEnumerable<Review>? Reviews { get; set; }

        [NotMapped]
        public List<IFormFile>? ProductFiles { get; set; }

        [NotMapped]
        public IFormFile? MainFile { get; set; }
    }
}
