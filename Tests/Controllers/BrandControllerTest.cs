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
[TestSubject(typeof(BrandController))]
[TestCategory("integration")]
public class BrandControllerTest : AutoMapperConfigTests
{
    private AppDbContext _context;
    private BrandController _brandController;
    private IDataRepository<Brand> _manager;

    // Objets communs pour les tests
    private Brand _brandAdidas;
    private Brand _brandNike;
    private Brand _brandCorsairEntity;
    private BrandDTO _brandDtoCorsair;
    private BrandUpdateDTO _brandUpdateDto;

    [TestInitialize]
    public void Initialize()
    {
        // Configuration pour récupérer la connection string
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Configuration du contexte avec PostgreSQL
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(configuration.GetConnectionString("SeriesDbContextRemote"))
            .Options;

        _context = new AppDbContext(options);

        // Manager et controller
        _manager = new BrandManager(_context);
        _brandController = new BrandController(_mapper, _manager, _context);

        // Données communes : entities
        _brandAdidas = new Brand { BrandName = "Adidas" };
        _brandNike = new Brand { BrandName = "Nike" };

        // DTO
        _brandDtoCorsair = new BrandDTO { Name = "Corsair" };
        _brandUpdateDto = new BrandUpdateDTO { Name = "Puma" };

        // Mapper DTO → Entity et ajouter en DB
        _brandCorsairEntity = _mapper.Map<Brand>(_brandDtoCorsair);

        // Ajout initial en DB
        _context.Brands.AddRange(_brandAdidas, _brandNike, _brandCorsairEntity);
        _context.SaveChanges();
    }

    [TestMethod]
    public void ShouldGetBrand()
    {
        // When
        ActionResult<BrandDTO> action = _brandController.Get(_brandAdidas.IdBrand).GetAwaiter().GetResult();
        BrandDTO returnBrand = action.Value;

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(BrandDTO));
        Assert.AreEqual(_brandAdidas.BrandName, returnBrand.Name);
    }

    [TestMethod]
    public void ShouldDeleteBrand()
    {
        // When
        IActionResult action = _brandController.Delete(_brandAdidas.IdBrand).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Brands.Find(_brandAdidas.IdBrand));
    }

    [TestMethod]
    public void ShouldNotDeleteBrandBecauseBrandDoesNotExist()
    {
        // When
        IActionResult action = _brandController.Delete(999).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllBrands()
    {
        // When
        var brands = _brandController.GetAll().GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(brands);
        Assert.IsInstanceOfType(brands.Value, typeof(IEnumerable<BrandDTO>));
    }

    [TestMethod]
    public void GetBrandShouldReturnNotFound()
    {
        // When
        ActionResult<BrandDTO> action = _brandController.Get(999).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        Assert.IsNull(action.Value);
    }

    [TestMethod]
    public void ShouldCreateBrand()
    {
        // Given
        BrandUpdateDTO newBrandDto = new BrandUpdateDTO { Name = "NewBrand" };

        // When
        IActionResult action = _brandController.Create(newBrandDto).GetAwaiter().GetResult();

        // Then
        var createdResult = (CreatedAtActionResult)action;
        var createdDto = (BrandDTO)createdResult.Value;

        Brand brandInDb = _context.Brands.Find(createdDto.Id);

        Assert.IsNotNull(brandInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(CreatedAtActionResult));
        Assert.AreEqual(newBrandDto.Name, brandInDb.BrandName);
    }

    [TestMethod]
    public void ShouldUpdateBrand()
    {
        // Given
        var updateDto = new BrandUpdateDTO { Name = "Puma" };

        // When
        IActionResult action = _brandController.Update(_brandAdidas.IdBrand, updateDto).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        Brand editedBrandInDb = _context.Brands.Find(_brandAdidas.IdBrand);

        Assert.IsNotNull(editedBrandInDb);
        Assert.AreEqual("Puma", editedBrandInDb.BrandName);
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseBrandDoesNotExist()
    {
        // Given
        BrandUpdateDTO updateDto = new BrandUpdateDTO { Name = "OnlyFan" };

        // When
        IActionResult action = _brandController.Update(999, updateDto).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    //[TestCleanup]
    //public void Cleanup()
    //{
    //    // Nettoyage des données de test
    //    _context.Brands.RemoveRange(_context.Brands);
    //    _context.SaveChanges();
    //}
}