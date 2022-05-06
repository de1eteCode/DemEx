namespace DemExApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            ProductAgent = new HashSet<ProductAgent>();
        }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(12)]
        public string Article { get; set; }

        public int CountForMaintance { get; set; }

        public int NumberCeh { get; set; }

        [Column(TypeName = "money")]
        public decimal MinPriceForAgent { get; set; }

        public Guid TypeUID { get; set; }

        public virtual TypeProduct TypeProduct { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductAgent> ProductAgent { get; set; }
    }
}
