using System.ComponentModel.DataAnnotations;

namespace CapitalShopFinalProject.Models
{
    public class BaseEntity
    {

        public int ID { get; set; }
        [StringLength(255)]
        public bool? IsDeleted { get; set; }

        [StringLength(255)]
        public string? CreatedBy { get; set; }

        public Nullable<DateTime> CreatedAt { get; set; }

        [StringLength(255)]
        public string? DeletedBy { get; set; }

        public Nullable<DateTime> DeletedAt { get; set; }
        [StringLength(255)]
        public string? UpdatedBy { get; set; }

        public Nullable<DateTime> UpdatedAt { get; set; }
    }
}
