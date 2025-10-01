using App.Controllers;
using App.DTO;
using App.Mapper;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Controllers;

[TestClass]
[TestSubject(typeof(BrandController))]
[TestCategory("integration")]
public class BrandControllerTest
{
    private readonly AppDbContext _context;
    private readonly BrandController _brandController;
    private readonly MapperConfiguration _config;

    public BrandControllerTest()
    {
        _context = new AppDbContext();

        _config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<BrandMapper>();
        }, new LoggerFactory());

        _config.AssertConfigurationIsValid();
        IMapper mapper = _config.CreateMapper();

        IDataRepository<Brand> manager = new BrandManager(_context);
        _brandController = new BrandController(mapper, manager, _context);
    }

    [TestMethod]
    public void ShouldGetBrand()
    {
        // Given : un produit en base de donnée
        Brand brandInDb = new Brand()
        {
            BrandName = "Adidas"
        };
        _context.Brands.Add(brandInDb);
        _context.SaveChanges();

        // When : J'appelle la méthode get de mon api pour récupérer le produit
        ActionResult<BrandDTO> action = _brandController.Get(brandInDb.IdBrand).GetAwaiter().GetResult();

        // Then : On récupère le produit et le code de retour est 200
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(ProductDetailDTO));
        BrandDTO returnProduct = action.Value; // 
        Assert.AreEqual(brandInDb.BrandName, returnProduct.Name);
    }

    [TestMethod]
    public void ShouldDeleteProduct()
    {
        // Given : Un produit enregistré
        Brand brandInDb = new Brand()
        {
            BrandName = "Adidas"
        };

        _context.Brands.Add(brandInDb);
        _context.SaveChanges();

        // When : Je souhaite supprimé un produit depuis l'API
        IActionResult action = _brandController.Delete(brandInDb.IdBrand).GetAwaiter().GetResult();

        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Products.Find(brandInDb.IdBrand));
    }

    [TestMethod]
    public void ShouldNotDeleteProductBecauseProductDoesNotExist()
    {
        // Given : Un produit enregistré
        Brand produitInDb = new Brand()
        {
            BrandName = "Adidas"
        };

        // When : Je souhaite supprimé un produit depuis l'API
        IActionResult action = _brandController.Delete(produitInDb.IdBrand).GetAwaiter().GetResult();

        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllProducts()
    {
        // Given : Des produits enregistrées
        IEnumerable<Brand> brandInDb = [
            new()
            {
                BrandName = "Adidas"
            },
            new()
            {
                BrandName = "Nike"
            }
        ];

        _context.Brands.AddRange(brandInDb);
        _context.SaveChanges();

        // When : On souhaite récupérer tous les produits
        var products = _brandController.GetAll().GetAwaiter().GetResult();

        // Then : Tous les produits sont récupérés
        Assert.IsNotNull(products);
        Assert.IsInstanceOfType(products.Value, typeof(IEnumerable<Brand>));
    }

    [TestMethod]
    public void GetProductShouldReturnNotFound()
    {
        // When : J'appelle la méthode get de mon api pour récupérer le produit
        ActionResult<BrandDTO> action = _brandController.Get(0).GetAwaiter().GetResult(); ;

        // Then : On ne renvoie rien et on renvoie 404
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");
        Assert.IsNull(action.Value, "Le produit n'est pas null");
    }

    [TestMethod]
    public void ShouldCreateProduct()
    {
        // Given
        BrandDTO brandToInsert = new BrandDTO()
        {
            Name = "Adidas"
        };

        // When
        ActionResult<BrandDTO> action = _brandController.Create(brandToInsert).GetAwaiter().GetResult(); // ✅ Corrigé le type

        // Then
        var createdResult = (CreatedAtActionResult)action.Result;
        var createdDto = (BrandDTO)createdResult.Value;

        Brand brandInDb = _context.Brands.Find(createdDto.Name);  // pas sûr de ça 

        Assert.IsNotNull(brandInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        Assert.AreEqual(brandToInsert.Name, brandInDb.BrandName);
    }

    [TestMethod]
    public void ShouldUpdateProduct()
    {
        // Given 
        Brand brandToEdit = new Brand()
        {
            BrandName = "Adidas"
        };

        _context.Brands.Add(brandToEdit);
        _context.SaveChanges();

        brandToEdit.BrandName = "Puma";

        // When
        IActionResult action = _brandController.Update(brandToEdit.IdBrand, brandToEdit).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        Product editedBrandInDb = _context.Products.Find(brandToEdit.IdBrand);

        Assert.IsNotNull(editedBrandInDb);
        Assert.AreEqual(brandToEdit.BrandName, editedBrandInDb.ProductName);
    }

    [TestMethod]
    public void ShouldNotUpdateProductBecauseIdInUrlIsDifferent()
    {
        // Given 
        Brand brandToEdit = new Brand()
        {
            BrandName = "Adidas"
        };

        _context.Brands.Add(brandToEdit);
        _context.SaveChanges();

        brandToEdit.BrandName = "OnlyFan";

        // When
        IActionResult action = _brandController.Update(0, brandToEdit).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }

    [TestMethod]
    public void ShouldNotUpdateProductBecauseProductDoesNotExist()
    {
        // Given 
        Brand produitToEdit = new Brand()
        {
            BrandName = "Adidas"
        };

        // When
        IActionResult action = _brandController.Update(produitToEdit.IdBrand, produitToEdit).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Brands.RemoveRange(_context.Brands);
        _context.SaveChanges();
    }
}
