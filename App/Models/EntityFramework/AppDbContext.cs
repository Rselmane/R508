using Microsoft.EntityFrameworkCore;

namespace App.Models.EntityFramework;

public partial class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<TypeProduct> TypeProducts { get; internal set; }

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.IdProduct);
            
            e.HasOne(p => p.NavigationBrand)
                .WithMany(m => m.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_product_brand");
            
            e.HasOne(p => p.NavigationTypeProduct)
                .WithMany(m => m.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_product_type");
        });

        modelBuilder.Entity<TypeProduct>(e =>
        {
            e.HasKey(p => p.IdTypeProduct);

            e.HasMany(p => p.Products)
                .WithOne(m => m.NavigationTypeProduct)
                .OnDelete(DeleteBehavior.ClientSetNull);

            e.HasData(
                new TypeProduct { IdTypeProduct = 1, TypeProductName = "Home" },
                new TypeProduct { IdTypeProduct = 2, TypeProductName = "Toys" },
                new TypeProduct { IdTypeProduct = 3, TypeProductName = "Electronics" },
                new TypeProduct { IdTypeProduct = 4, TypeProductName = "Clothes" },
                new TypeProduct { IdTypeProduct = 5, TypeProductName = "Other" }
            );
        });
        
        modelBuilder.Entity<Brand>(e =>
        {
            e.HasKey(p => p.IdBrand);

            e.HasMany(p => p.Products)
                .WithOne(m => m.NavigationBrand)
                .OnDelete(DeleteBehavior.ClientSetNull);

            e.HasData(
                new Brand { IdBrand = 1, BrandName = "Ikea" },
                new Brand { IdBrand = 2, BrandName = "Mobalpa" },
                new Brand { IdBrand = 3, BrandName = "LeRoyMerlin" },

                new Brand { IdBrand = 4, BrandName = "Lego" },
                new Brand { IdBrand = 5, BrandName = "Playmobil" },
                new Brand { IdBrand = 6, BrandName = "Hasbro" },

                new Brand { IdBrand = 7, BrandName = "Apple" },
                new Brand { IdBrand = 8, BrandName = "Samsung" },
                new Brand { IdBrand = 9, BrandName = "Sony" },

                new Brand { IdBrand = 10, BrandName = "Nike" },
                new Brand { IdBrand = 11, BrandName = "Adidas" },
                new Brand { IdBrand = 12, BrandName = "Puma" },

                new Brand { IdBrand = 13, BrandName = "Paypal" }
            );
        });
        
        modelBuilder.Entity<Product>(e =>
        {
            e.HasData(
                // Ikea
                new Product { IdProduct = 1, ProductName = "Corner Sofa", Description = "A comfortable corner sofa for your living room.", PhotoName = "corner_sofa.jpg", PhotoUri = "/images/corner_sofa.jpg", IdTypeProduct = 1, IdBrand = 1, ActualStock = 10, MinStock = 2, MaxStock = 20 },
                new Product { IdProduct = 2, ProductName = "Coffee Table", Description = "Coffee table made of solid wood.", PhotoName = "coffee_table.jpg", PhotoUri = "/images/coffee_table.jpg", IdTypeProduct = 1, IdBrand = 1, ActualStock = 15, MinStock = 3, MaxStock = 30 },

                // Mobalpa
                new Product { IdProduct = 3, ProductName = "Fitted Kitchen", Description = "Modern kitchen with built-in storage.", PhotoName = "kitchen.jpg", PhotoUri = "/images/kitchen.jpg", IdTypeProduct = 1, IdBrand = 2, ActualStock = 5, MinStock = 1, MaxStock = 10 },

                // LeRoyMerlin
                new Product { IdProduct = 4, ProductName = "Cordless Drill", Description = "Durable drill with long-lasting battery.", PhotoName = "drill.jpg", PhotoUri = "/images/drill.jpg", IdTypeProduct = 1, IdBrand = 3, ActualStock = 20, MinStock = 5, MaxStock = 50 },

                // Lego
                new Product { IdProduct = 5, ProductName = "Lego Star Wars", Description = "Lego Star Wars collector's set.", PhotoName = "lego_starwars.jpg", PhotoUri = "/images/lego_starwars.jpg", IdTypeProduct = 2, IdBrand = 4, ActualStock = 25, MinStock = 5, MaxStock = 40 },
                new Product { IdProduct = 6, ProductName = "Lego City", Description = "Lego City set for kids.", PhotoName = "lego_city.jpg", PhotoUri = "/images/lego_city.jpg", IdTypeProduct = 2, IdBrand = 4, ActualStock = 30, MinStock = 6, MaxStock = 60 },

                // Playmobil
                new Product { IdProduct = 7, ProductName = "Playmobil Castle", Description = "Medieval Playmobil castle with figures.", PhotoName = "playmobil_castle.jpg", PhotoUri = "/images/playmobil_castle.jpg", IdTypeProduct = 2, IdBrand = 5, ActualStock = 12, MinStock = 2, MaxStock = 25 },

                // Hasbro
                new Product { IdProduct = 8, ProductName = "Monopoly", Description = "Classic Monopoly board game.", PhotoName = "monopoly.jpg", PhotoUri = "/images/monopoly.jpg", IdTypeProduct = 2, IdBrand = 6, ActualStock = 40, MinStock = 8, MaxStock = 80 },

                // Apple
                new Product { IdProduct = 9, ProductName = "iPhone 15", Description = "Latest iPhone model with OLED display.", PhotoName = "iphone15.jpg", PhotoUri = "/images/iphone15.jpg", IdTypeProduct = 3, IdBrand = 7, ActualStock = 50, MinStock = 10, MaxStock = 100 },
                new Product { IdProduct = 10, ProductName = "MacBook Pro", Description = "High performance laptop.", PhotoName = "macbook.jpg", PhotoUri = "/images/macbook.jpg", IdTypeProduct = 3, IdBrand = 7, ActualStock = 20, MinStock = 5, MaxStock = 30 },

                // Samsung
                new Product { IdProduct = 11, ProductName = "Galaxy S24", Description = "Samsung high-end smartphone.", PhotoName = "galaxy_s24.jpg", PhotoUri = "/images/galaxy_s24.jpg", IdTypeProduct = 3, IdBrand = 8, ActualStock = 45, MinStock = 8, MaxStock = 90 },
                new Product { IdProduct = 12, ProductName = "QLED TV 55\"", Description = "High-definition QLED television.", PhotoName = "qled_tv.jpg", PhotoUri = "/images/qled_tv.jpg", IdTypeProduct = 3, IdBrand = 8, ActualStock = 18, MinStock = 3, MaxStock = 25 },

                // Sony
                new Product { IdProduct = 13, ProductName = "PlayStation 5", Description = "Next-generation gaming console.", PhotoName = "ps5.jpg", PhotoUri = "/images/ps5.jpg", IdTypeProduct = 3, IdBrand = 9, ActualStock = 35, MinStock = 7, MaxStock = 50 },
                new Product { IdProduct = 14, ProductName = "Sony WH-1000XM5 Headphones", Description = "Wireless headphones with noise cancellation.", PhotoName = "sony_wh1000xm5.jpg", PhotoUri = "/images/sony_wh1000xm5.jpg", IdTypeProduct = 3, IdBrand = 9, ActualStock = 25, MinStock = 5, MaxStock = 40 },

                // Nike
                new Product { IdProduct = 15, ProductName = "Air Max", Description = "Iconic sports shoes.", PhotoName = "airmax.jpg", PhotoUri = "/images/airmax.jpg", IdTypeProduct = 4, IdBrand = 10, ActualStock = 30, MinStock = 5, MaxStock = 50 },

                // Adidas
                new Product { IdProduct = 16, ProductName = "Stan Smith", Description = "Classic Adidas sneakers.", PhotoName = "stansmith.jpg", PhotoUri = "/images/stansmith.jpg", IdTypeProduct = 4, IdBrand = 11, ActualStock = 25, MinStock = 5, MaxStock = 40 },

                // Puma
                new Product { IdProduct = 17, ProductName = "Puma Suede", Description = "Iconic Puma Suede sneakers.", PhotoName = "puma_suede.jpg", PhotoUri = "/images/puma_suede.jpg", IdTypeProduct = 4, IdBrand = 12, ActualStock = 20, MinStock = 4, MaxStock = 30 },

                // Paypal
                new Product { IdProduct = 18, ProductName = "PayPal Gift Card", Description = "Digital gift card usable everywhere.", PhotoName = "paypal_gift.jpg", PhotoUri = "/images/paypal_gift.jpg", IdTypeProduct = 5, IdBrand = 13, ActualStock = 100, MinStock = 20, MaxStock = 200 }
            );
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
