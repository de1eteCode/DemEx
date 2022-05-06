namespace DemExApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductAgent")]
    public partial class ProductAgent : BaseEntity
    {
        public Guid AgentUID { get; set; }

        public Guid ProductUID { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public int Count { get; set; }

        public virtual Agent Agent { get; set; }

        public virtual Product Product { get; set; }
    }
}
