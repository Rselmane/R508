using App.Controllers;
using App.DTO;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Tests.Controllers;

[TestClass]
[TestSubject(typeof(ProductController))]
[TestCategory("integration")]
public class ProductControllerTest
{
    private readonly AppDbContext _context;
    private readonly ProductController _productController;

    public ProductControllerTest()
    {
        _context = new AppDbContext();

        var manager = new ProductManager(_context);
        _productController = new ProductController(manager);
    }

    [TestMethod]
    public void ShouldGetProduct()
    {
        // Given : un produit en base de donnée
        Product produitInDb = new Product()
        {
            NomProduit = "Chaise",
            Description = "Une superbe chaise",
            NomPhoto = "Une superbe chaise bleu",
            UriPhoto = "https://ikea.fr/chaise.jpg"
        };
        _context.Produits.Add(produitInDb);
        _context.SaveChanges();

        // When : J'appelle la méthode get de mon api pour récupérer le produit
        ActionResult<ProductDetailDTO> action = _productController.Get(produitInDb.IdProduit).GetAwaiter().GetResult();

        // Then : On récupère le produit et le code de retour est 200
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(ProductDetailDTO));
        ProductDetailDTO returnProduct = action.Value; // 
        Assert.AreEqual(produitInDb.NomProduit, returnProduct.Nom);
    }

    [TestMethod]
    public void ShouldDeleteProduct()
    {
        // Given : Un produit enregistré
        Product produitInDb = new Product()
        {
            NomProduit = "Chaise",
            Description = "Une superbe chaise",
            NomPhoto = "Une superbe chaise bleu",
            UriPhoto = "https://ikea.fr/chaise.jpg"
        };

        _context.Produits.Add(produitInDb);
        _context.SaveChanges();

        // When : Je souhaite supprimé un produit depuis l'API
        IActionResult action = _productController.Delete(produitInDb.IdProduit).GetAwaiter().GetResult();

        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Produits.Find(produitInDb.IdProduit));
    }

    [TestMethod]
    public void ShouldNotDeleteProductBecauseProductDoesNotExist()
    {
        // Given : Un produit enregistré
        Product produitInDb = new Product()
        {
            NomProduit = "Chaise",
            Description = "Une superbe chaise",
            NomPhoto = "Une superbe chaise bleu",
            UriPhoto = "https://ikea.fr/chaise.jpg"
        };

        // When : Je souhaite supprimé un produit depuis l'API
        IActionResult action = _productController.Delete(produitInDb.IdProduit).GetAwaiter().GetResult();

        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllProducts()
    {
        // Given : Des produits enregistrées
        IEnumerable<Product> productInDb = [
            new()
            {
                NomProduit = "Chaise",
                Description = "Une superbe chaise",
                NomPhoto = "Une superbe chaise bleu",
                UriPhoto = "https://ikea.fr/chaise.jpg"
            },
            new()
            {
                NomProduit = "Armoir",
                Description = "Une superbe armoire",
                NomPhoto = "Une superbe armoire jaune",
                UriPhoto = "https://ikea.fr/armoire-jaune.jpg"
            }
        ];

        _context.Produits.AddRange(productInDb);
        _context.SaveChanges();

        // When : On souhaite récupérer tous les produits
        var products = _productController.GetAll().GetAwaiter().GetResult();

        // Then : Tous les produits sont récupérés
        Assert.IsNotNull(products);
        Assert.IsInstanceOfType(products.Value, typeof(IEnumerable<Product>));
    }

    [TestMethod]
    public void GetProductShouldReturnNotFound()
    {
        // When : J'appelle la méthode get de mon api pour récupérer le produit
        ActionResult<ProductDetailDTO> action = _productController.Get(0).GetAwaiter().GetResult(); ;

        // Then : On ne renvoie rien et on renvoie 404
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");
        Assert.IsNull(action.Value, "Le produit n'est pas null");
    }

    [TestMethod]
    public void ShouldCreateProduct()
    {
        // Given
        ProductAddDTO productToInsert = new()
        {
            Nom = "Chaise",
            Description = "Une superbe chaise",
            NomPhoto = "Une superbe chaise bleu",
            UriPhoto = "https://ikea.fr/chaise.jpg"
        };

        // When
        ActionResult<ProductDetailDTO> action = _productController.Create(productToInsert).GetAwaiter().GetResult(); // ✅ Corrigé le type

        // Then
        var createdResult = (CreatedAtActionResult)action.Result;
        var createdDto = (ProductDetailDTO)createdResult.Value;

        Product productInDb = _context.Produits.Find(createdDto.Nom);  // pas sûr de ça 

        Assert.IsNotNull(productInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        Assert.AreEqual(productToInsert.Nom, productInDb.NomProduit);
    }

    [TestMethod]
    public void ShouldUpdateProduct()
    {
        // Given 
        Product produitToEdit = new()
        {
            NomProduit = "Bureau",
            Description = "Un super bureau",
            NomPhoto = "Un super bureau bleu",
            UriPhoto = "https://ikea.fr/bureau.jpg"
        };

        _context.Produits.Add(produitToEdit);
        _context.SaveChanges();

        produitToEdit.NomProduit = "Lit";
        produitToEdit.Description = "Un super lit";

        // When
        IActionResult action = _productController.Update(produitToEdit.IdProduit, produitToEdit).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        Product editedProductInDb = _context.Produits.Find(produitToEdit.IdProduit);

        Assert.IsNotNull(editedProductInDb);
        Assert.AreEqual(produitToEdit.NomProduit, editedProductInDb.NomProduit);
        Assert.AreEqual(produitToEdit.Description, editedProductInDb.Description);
    }

    [TestMethod]
    public void ShouldNotUpdateProductBecauseIdInUrlIsDifferent()
    {
        // Given 
        Product produitToEdit = new()
        {
            NomProduit = "Bureau",
            Description = "Un super bureau",
            NomPhoto = "Un super bureau bleu",
            UriPhoto = "https://ikea.fr/bureau.jpg"
        };

        _context.Produits.Add(produitToEdit);
        _context.SaveChanges();

        produitToEdit.NomProduit = "Lit";
        produitToEdit.Description = "Un super lit";

        // When
        IActionResult action = _productController.Update(0, produitToEdit).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }

    [TestMethod]
    public void ShouldNotUpdateProductBecauseProductDoesNotExist()
    {
        // Given 
        Product produitToEdit = new()
        {
            IdProduit = 20,
            NomProduit = "Bureau",
            Description = "Un super bureau",
            NomPhoto = "Un super bureau bleu",
            UriPhoto = "https://ikea.fr/bureau.jpg"
        };

        // When
        IActionResult action = _productController.Update(produitToEdit.IdProduit, produitToEdit).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Produits.RemoveRange(_context.Produits);
        _context.SaveChanges();
    }
}