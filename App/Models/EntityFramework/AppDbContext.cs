using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace App.Models.EntityFramework;

public partial class AppDbContext : DbContext
{
    public DbSet<Product> Produits { get; set; }
    public DbSet<Brand> Marques { get; set; }
    public DbSet<TypeProduct> TypeProduits { get; internal set; }

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
        });
        
        modelBuilder.Entity<Brand>(e =>
        {
            e.HasKey(p => p.IdBrand);

            e.HasMany(p => p.Products)
                .WithOne(m => m.NavigationBrand)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
