using App.Controllers;
using App.DTO;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.AutoMapper;

namespace Tests.Controllers;

[TestClass]
[TestSubject(typeof(BrandController))]
[TestCategory("integration")]
public class BrandControllerTest : AutoMapperConfigTests
{
    private AppDbContext _context;
    private BrandController _brandController;
    private IDataRepository<Brand> _manager;

    // Entities pour les tests
    private Brand _brandAdidas;
    private Brand _brandNike;
    private Brand _brandCorsair;

    // DTOs pour les tests
    private BrandUpdateDTO _brandUpdateDtoPuma;
    private BrandUpdateDTO _brandUpdateDtoNewBrand;
    private BrandUpdateDTO _brandUpdateDtoOnlyFan;

    [TestInitialize]
    public void Initialize()
    {
        // Configuration du contexte avec InMemory Database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"BrandTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);

        // Manager et controller
        _manager = new BrandManager(_context);
        _brandController = new BrandController(_mapper, _manager, _context);

        // Données communes : entities
        _brandAdidas = new Brand { BrandName = "Adidas" };
        _brandNike = new Brand { BrandName = "Nike" };
        _brandCorsair = new Brand { BrandName = "Corsair" };

        // DTOs
        _brandUpdateDtoPuma = new BrandUpdateDTO { Name = "Puma" };
        _brandUpdateDtoNewBrand = new BrandUpdateDTO { Name = "NewBrand" };
        _brandUpdateDtoOnlyFan = new BrandUpdateDTO { Name = "OnlyFan" };

        // Ajout initial en DB
        _context.Brands.AddRange(_brandAdidas, _brandNike, _brandCorsair);
        _context.SaveChanges();
    }

    [TestMethod]
    public void ShouldGetBrand()
    {
        // When
        IActionResult action = _brandController.Get(_brandAdidas.IdBrand).GetAwaiter().GetResult();
        OkObjectResult okResult = action as OkObjectResult;

        // Then
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
        Assert.IsInstanceOfType(okResult.Value, typeof(BrandDTO));

        var returnBrand = (BrandDTO)okResult.Value!;
        Assert.AreEqual(_brandAdidas.BrandName, returnBrand.Name);
    }


    [TestMethod]
    public void ShouldDeleteBrand()
    {
        // When
        var action = _brandController.Delete(_brandAdidas.IdBrand).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Brands.Find(_brandAdidas.IdBrand));
    }

    [TestMethod]
    public void ShouldNotDeleteBrandBecauseBrandDoesNotExist()
    {
        // When
        var action = _brandController.Delete(999).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllBrands()
    {
        // Given : Des marques enregistrées en DB
        IEnumerable<Brand> brandsInDb = _context.Brands.ToList();
        IEnumerable<BrandDTO> expectedBrands = brandsInDb.Select(b => _mapper.Map<BrandDTO>(b));

        // When : On récupère toutes les marques
        var action = _brandController.GetAll().GetAwaiter().GetResult();

        // Then : Toutes les marques sont récupérées
        Assert.IsInstanceOfType(action, typeof(ActionResult<IEnumerable<BrandDTO>>));
    }

    [TestMethod]
    public void GetBrandShouldReturnNotFound()
    {
        // When
        IActionResult action = _brandController.Get(999).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldCreateBrand()
    {
        // When
        var action = _brandController.Create(_brandUpdateDtoNewBrand).GetAwaiter().GetResult();

        // Then
        CreatedAtActionResult createdResult = (CreatedAtActionResult)action;
        BrandDTO createdDto = (BrandDTO)createdResult.Value;
        Brand brandInDb = _context.Brands.Find(createdDto.Id);

        Assert.IsNotNull(brandInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(CreatedAtActionResult));
        Assert.AreEqual(_brandUpdateDtoNewBrand.Name, brandInDb.BrandName);
    }

    [TestMethod]
    public void ShouldUpdateBrand()
    {
        // When
        var action = _brandController.Update(_brandAdidas.IdBrand, _brandUpdateDtoPuma).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        var editedBrandInDb = _context.Brands.Find(_brandAdidas.IdBrand);

        Assert.IsNotNull(editedBrandInDb);
        Assert.AreEqual(_brandUpdateDtoPuma.Name, editedBrandInDb.BrandName);
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseBrandDoesNotExist()
    {
        // When
        var action = _brandController.Update(999, _brandUpdateDtoOnlyFan).GetAwaiter().GetResult();

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