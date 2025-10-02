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
using Tests.AutoMapper;

namespace Tests.Controllers;

[TestClass]
[TestSubject(typeof(TypeProductController))]
[TestCategory("integration")]
public class TypeProductControllerTest : AutoMapperConfigTests
{
    private AppDbContext _context;
    private TypeProductController _typeProductdController;
    private IDataRepository<TypeProduct> _manager;

    // Objets communs pour les tests
    private TypeProduct _typeProdcutKeybord;
    private TypeProduct _typeProdcutScreen;
    private TypeProduct _typeProdcutMouseEntity;
    private TypeProductDTO _typeProductDtoMouse;

    [TestInitialize]
    public void Initialize()
    {
        // Contexte et mapper
        _context = new AppDbContext();

        // Manager et controller
        _manager = new TypeProductManager(_context);
        _typeProductdController = new TypeProductController(_mapper, _manager, _context);

        // Données communes : entities
        _typeProdcutKeybord = new TypeProduct { TypeProductName = "Adidas" };
        _typeProdcutScreen = new TypeProduct { TypeProductName = "Nike" };

        // DTO
        _typeProductDtoMouse = new TypeProductDTO { Name = "Corsair" };

        // Mapper DTO → Entity et ajouter en DB
        _typeProdcutMouseEntity = _mapper.Map<TypeProduct>(_typeProductDtoMouse);

        // Ajout initial en DB
        _context.TypeProducts.AddRange(_typeProdcutKeybord, _typeProdcutScreen, _typeProdcutMouseEntity);
        _context.SaveChanges();
    }

    [TestMethod]
    public void ShouldGetBrand()
    {
        // When
        IActionResult action = _typeProductdController.Get(_typeProdcutKeybord.IdTypeProduct).GetAwaiter().GetResult();
        TypeProductDTO returnTypeProduct = (TypeProductDTO)action;

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(TypeProductDTO));
        Assert.AreEqual(_typeProdcutKeybord.TypeProductName, returnTypeProduct.Name);
    }

    [TestMethod]
    public void ShouldDeleteBrand()
    {
        // When
        IActionResult action = _typeProductdController.Delete(_typeProdcutKeybord.IdTypeProduct).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.TypeProducts.Find(_typeProdcutKeybord.IdTypeProduct));
    }

    [TestMethod]
    public void ShouldNotDeleteBrandBecauseBrandDoesNotExist()
    {
        // When
        IActionResult action = _typeProductdController.Delete(999).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllBrands()
    {
        // When
        var typeProducts = _typeProductdController.GetAll().GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(typeProducts);
        Assert.IsInstanceOfType(typeProducts.Value, typeof(IEnumerable<TypeProductDTO>));
    }

    [TestMethod]
    public void GetBrandShouldReturnNotFound()
    {
        // When
        IActionResult action = _typeProductdController.Get(999).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        Assert.IsNull(action);
    }

    [TestMethod]
    public void ShouldCreateBrand()
    {
        // Given
        TypeProductUpdateDTO newTypeProductDto = new TypeProductUpdateDTO { Name = "NewTypeProduct" };

        // When
        IActionResult action = _typeProductdController.Create(newTypeProductDto).GetAwaiter().GetResult();

        // Then
        CreatedAtActionResult createdResult = (CreatedAtActionResult)action;
        TypeProductDTO createdDto = (TypeProductDTO)createdResult.Value;

        TypeProduct typeProductInDb = _context.TypeProducts.Find(createdDto.Id);

        Assert.IsNotNull(typeProductInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(CreatedAtActionResult));
        Assert.AreEqual(newTypeProductDto.Name, typeProductInDb.TypeProductName);
    }

    [TestMethod]
    public void ShouldUpdateBrand()
    {
        // Given
        var updateDto = new TypeProductUpdateDTO { Name = "KeybordV2" };

        // When
        IActionResult action = _typeProductdController.Update(_typeProdcutKeybord.IdTypeProduct, updateDto).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        TypeProduct editedTypeProductInDb = _context.TypeProducts.Find(_typeProdcutKeybord.IdTypeProduct);

        Assert.IsNotNull(editedTypeProductInDb);
        Assert.AreEqual("KeybordV2", editedTypeProductInDb.TypeProductName);
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseIdInUrlIsDifferent()
    {
        // Given
        var updateDto = new TypeProductUpdateDTO { Name = "OnlyFan" };

        // When
        IActionResult action = _typeProductdController.Update(0, updateDto).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseBrandDoesNotExist()
    {
        // Given
        var updateDto = new TypeProductUpdateDTO { Name = "ScreenV2" };

        // When
        IActionResult action = _typeProductdController.Update(999, updateDto).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    //[TestCleanup]
    //public void Cleanup()
    //{
    //    _context.TypeProducts.RemoveRange(_context.TypeProducts);
    //    _context.SaveChanges();
    //}
}