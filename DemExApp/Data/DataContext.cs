using System.Data.Entity;

namespace DemExApp.Data {
    public partial class DataContext : DbContext {
        private static DataContext _context;
        private static object _sync = new object();

        public static DataContext Instance() {
            if (_context == null) {
                lock (_sync) {
                    if (_context == null) {
                        _context = new DataContext();
                    }
                }
            }
            return _context;
        }

        public DataContext()
            : base("name=DataContext") {
            this.Configuration.LazyLoadingEnabled = true;
        }

        public virtual DbSet<Agent> Agent { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductAgent> ProductAgent { get; set; }
        public virtual DbSet<TypeAgent> TypeAgent { get; set; }
        public virtual DbSet<TypeProduct> TypeProduct { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Agent>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Agent>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Agent>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Agent>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<Agent>()
                .Property(e => e.Director)
                .IsUnicode(false);

            modelBuilder.Entity<Agent>()
                .Property(e => e.INN)
                .IsUnicode(false);

            modelBuilder.Entity<Agent>()
                .Property(e => e.KPP)
                .IsUnicode(false);

            modelBuilder.Entity<Agent>()
                .Property(e => e.LogoURL)
                .IsUnicode(false);

            modelBuilder.Entity<Agent>()
                .HasMany(e => e.ProductAgent)
                .WithRequired(e => e.Agent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Article)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.MinPriceForAgent)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductAgent)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TypeAgent>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<TypeAgent>()
                .HasMany(e => e.Agent)
                .WithRequired(e => e.TypeAgent)
                .HasForeignKey(e => e.TypeUID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TypeProduct>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<TypeProduct>()
                .HasMany(e => e.Product)
                .WithRequired(e => e.TypeProduct)
                .HasForeignKey(e => e.TypeUID)
                .WillCascadeOnDelete(false);
        }
    }
}
