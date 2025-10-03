using App.Controllers;
using App.DTO;
using App.Mapper;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.AutoMapper;

namespace Tests.Controllers;

[TestClass]
[TestSubject(typeof(ProductController))]
[TestCategory("integration")]
public class ProductControllerTest : AutoMapperConfigTests
{
    private AppDbContext _context;
    private ProductController _productController;
    private IProductRepository _manager;

    // Objets communs pour les tests
    private Product _productChaise;
    private Product _productArmoir;
    private Product _productBureau;
    private ProductAddDTO _productAddChaise;
    private ProductAddDTO _productUpdateLit;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"BrandTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);        

        _manager = new ProductManager(_context);
        _productController = new ProductController(_mapper, _manager, _context);

        // Données communes : entities
        _productChaise = new Product()
        {
            ProductName = "Chaise",
            Description = "Une superbe chaise",
            PhotoName = "Une superbe chaise bleu",
            PhotoUri = "https://ikea.fr/chaise.jpg",
            MinStock = 5,
            MaxStock = 30
        };

        _productArmoir = new Product()
        {
            ProductName = "Armoir",
            Description = "Une superbe armoire",
            PhotoName = "Une superbe armoire jaune",
            PhotoUri = "https://ikea.fr/armoire-jaune.jpg",
            MinStock = 2,
            MaxStock = 10
        };

        _productBureau = new Product()
        {
            ProductName = "Bureau",
            Description = "Un super bureau",
            PhotoName = "Un super bureau bleu",
            PhotoUri = "https://ikea.fr/bureau.jpg",
            MinStock = 3,
            MaxStock = 20
        };

        _context.Products.AddRange(_productChaise, _productArmoir, _productBureau);
        _context.SaveChanges();

        // DTOs
        _productAddChaise = new ProductAddDTO()
        {
            Name = "Chaise",
            Description = "Une superbe chaise",
            PhotoName = "Une superbe chaise bleu",
            PhotoUri = "https://ikea.fr/chaise.jpg",
            Stock = 15,
            MinStock = 5,
            MaxStock = 40,
            Brand = "Ikea",
            Type = "Chaise"
        };

        _productUpdateLit = new ProductAddDTO()
        {
            Name = "Lit",
            Description = "Un super lit",
            PhotoName = "Un super bureau bleu",
            PhotoUri = "https://ikea.fr/bureau.jpg",
            Stock = 6,
            MinStock = 2,
            MaxStock = 15,
            Brand = "Conforama",
            Type = "Lit"
        };
    }


    [TestMethod]
    public void ShouldGetProduct()
    {
        // When
        IActionResult action = _productController.Get(_productChaise.IdProduct).GetAwaiter().GetResult();
        OkObjectResult okResult = action as OkObjectResult;

        // Then
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
        Assert.IsInstanceOfType(okResult.Value, typeof(ProductDetailDTO));

        var returnProduct = (ProductDetailDTO)okResult.Value!;
        Assert.AreEqual(_productChaise.ProductName, returnProduct.Name);
    }

    [TestMethod]
    public void ShouldDeleteProduct()
    {
        // When
        IActionResult action = _productController.Delete(_productChaise.IdProduct).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Products.Find(_productChaise.IdProduct));
    }

    [TestMethod]
    public void ShouldNotDeleteProductBecauseProductDoesNotExist()
    {
        // When
        IActionResult action = _productController.Delete(999).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllProducts()
    {
        IEnumerable<Product> productsInDb = _context.Products.ToList();
        IEnumerable<ProductDTO> expectedProducts = productsInDb.Select(p => _mapper.Map<ProductDTO>(p));

        // When
        var products = _productController.GetAll().GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(products);
        Assert.IsInstanceOfType(products, typeof(ActionResult<IEnumerable<ProductDTO>>));
    }

    [TestMethod]
    public void GetProductShouldReturnNotFound()
    {
        // When
        IActionResult action = _productController.Get(999).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldCreateProduct()
    {
        // When
        IActionResult action = _productController.Create(_productAddChaise).GetAwaiter().GetResult();

        // Then
        var createdResult = (CreatedAtActionResult)action;
        var createdDto = (ProductDetailDTO)createdResult.Value;

        Product productInDb = _context.Products.Find(createdDto.Id);


        Assert.IsInstanceOfType(action, typeof(CreatedAtActionResult));
        Assert.AreEqual(_productAddChaise.Name, productInDb.ProductName);
    }

    [TestMethod]
    public void ShouldUpdateProduct()
    {
        // When
        IActionResult action = _productController.Update(_productBureau.IdProduct, _productUpdateLit).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        Product editedProductInDb = _context.Products.Find(_productBureau.IdProduct);

        Assert.IsNotNull(editedProductInDb);
        Assert.AreEqual("Lit", editedProductInDb.ProductName);
        Assert.AreEqual("Un super lit", editedProductInDb.Description);
    }

    [TestMethod]
    public void ShouldNotUpdateProductBecauseProductDoesNotExist()
    {
        // When
        IActionResult action = _productController.Update(999, _productUpdateLit).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}