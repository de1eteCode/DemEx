namespace DemExApp.Data {
    using DemExApp.Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;

    [Table("Agent")]
    public partial class Agent : BaseEntity {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Agent() {
            ProductAgent = new HashSet<ProductAgent>();
        }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [StringLength(250)]
        public string Address { get; set; }

        public int Priority { get; set; }

        [Required]
        [StringLength(250)]
        public string Director { get; set; }

        [Required]
        [StringLength(30)]
        public string INN { get; set; }

        [Required]
        [StringLength(30)]
        public string KPP { get; set; }

        public Guid TypeUID { get; set; }

        [Column(TypeName = "image")]
        public byte[] Logo { get; set; }

        [StringLength(200)]
        public string LogoURL { get; set; }

        public virtual TypeAgent TypeAgent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductAgent> ProductAgent { get; set; }

        [NotMapped]
        public string TypeString {
            get {
                return TypeAgent.Name;
            }
        }

        [NotMapped]
        public int CountSales {
            get {
                return ProductAgent.Where(e => e.Date >= e.Date.AddDays(-365)).Count();
            }
        }

        [NotMapped]
        public int Discount {
            get {
                var sum = ProductAgent.Sum(e => e.Product.MinPriceForAgent * e.Count);
                if (sum < 10000) {
                    return 0;
                }
                if (sum < 50000) {
                    return 5;
                }
                if (sum < 150000) {
                    return 10;
                }
                if (sum < 500000) {
                    return 20;
                }
                return 25;
            }
        }

        [NotMapped]
        public byte[] ImageShow {
            get {
                if (Logo is null) {
                    var uri = new Uri("Resources/Pictures/EmptyLogo.png", UriKind.Relative);
                    var resourceInfo = Application.GetResourceStream(uri);
                    using (var ms = new MemoryStream()) {
                        resourceInfo.Stream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
                return Logo;
            }
            set {
                Logo = value;
            }
        }
    }
}
