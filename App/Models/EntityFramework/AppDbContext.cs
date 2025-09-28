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
            e.HasKey(p => p.IdProduit);
            
            e.HasOne(p => p.MarqueNavigation)
                .WithMany(m => m.Produits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_produits_marque");
            
            e.HasOne(p => p.TypeProduitNavigation)
                .WithMany(m => m.Produits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_produits_type_produit");
        });
        modelBuilder.Entity<TypeProduct>(e =>
        {
            e.HasKey(p => p.IdTypeProduit);

            e.HasMany(p => p.Produits)
                .WithOne(m => m.TypeProduitNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        
        modelBuilder.Entity<Brand>(e =>
        {
            e.HasKey(p => p.IdMarque);

            e.HasMany(p => p.Produits)
                .WithOne(m => m.MarqueNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
