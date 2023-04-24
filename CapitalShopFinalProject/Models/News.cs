using System.ComponentModel.DataAnnotations;

namespace CapitalShopFinalProject.Models
{
    public class News : BaseEntity
    {
        [StringLength(255)]
        public string? Title { get; set; }

        [StringLength(255)]
        public string? Image { get; set; }

        [StringLength(10000)]
        public string? Content { get; set; }
        [StringLength(100)]
        public string? ShortContent { get; set; }





    }
}
