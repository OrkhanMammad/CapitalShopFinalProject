using System.ComponentModel.DataAnnotations;

namespace CapitalShopFinalProject.Models
{
    public class Testimonial : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(4000)]

        public string Content { get; set; }
        [StringLength(300)]

        public string Image { get; set; }
        [StringLength(100)]
        public string Profeciency { get; set; }




    }
}
