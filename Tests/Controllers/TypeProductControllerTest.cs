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
        // When : J'appelle la méthode get de mon api pour récupérer le produit
        ActionResult<TypeProductDTO> action = _typeProductdController.Get(_typeProdcutKeybord.IdTypeProduct).GetAwaiter().GetResult();
        TypeProductDTO returnTypeProduct = action.Value;

        // Then : On récupère le produit et le code de retour est 200
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(TypeProductDTO));
        Assert.AreEqual(_typeProdcutKeybord.TypeProductName, returnTypeProduct.Name);
    }

    [TestMethod]
    public void ShouldDeleteBrand()
    {
        // When : Je souhaite supprimé un produit depuis l'API
        IActionResult action = _typeProductdController.Delete(_typeProdcutKeybord.IdTypeProduct).GetAwaiter().GetResult();

        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Products.Find(_typeProdcutKeybord.IdTypeProduct));
    }

    [TestMethod]
    public void ShouldNotDeleteBrandBecauseBrandDoesNotExist()
    {
        // When : Je souhaite supprimé un produit depuis l'API
        IActionResult action = _typeProductdController.Delete(_typeProdcutKeybord.IdTypeProduct).GetAwaiter().GetResult();

        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllBrands()
    {
        // When : On souhaite récupérer tous les produits
        var typeProducts = _typeProductdController.GetAll().GetAwaiter().GetResult();

        // Then : Tous les produits sont récupérés
        Assert.IsNotNull(typeProducts);
        Assert.IsInstanceOfType(typeProducts.Value, typeof(IEnumerable<TypeProduct>));
    }

    [TestMethod]
    public void GetBrandShouldReturnNotFound()
    {
        // When : J'appelle la méthode get de mon api pour récupérer le produit
        ActionResult<TypeProductDTO> action = _typeProductdController.Get(0).GetAwaiter().GetResult(); ;

        // Then : On ne renvoie rien et on renvoie 404
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");
        Assert.IsNull(action.Value, "Le produit n'est pas null");
    }

    [TestMethod]
    public void ShouldCreateBrand()
    {
        // When
        ActionResult<TypeProductDTO> action = _typeProductdController.Create(_typeProductDtoMouse).GetAwaiter().GetResult(); // ✅ Corrigé le type

        // Then
        var createdResult = (CreatedAtActionResult)action.Result;
        var createdDto = (TypeProductDTO)createdResult.Value;

        TypeProduct typeProductInDb = _context.TypeProducts.Find(createdDto.Name);  // pas sûr de ça 

        Assert.IsNotNull(typeProductInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        Assert.AreEqual(_typeProductDtoMouse.Name, typeProductInDb.TypeProductName);
    }

    [TestMethod]
    public void ShouldUpdateBrand()
    {
        // Given
        _typeProdcutKeybord.TypeProductName = "KeybordV2";

        // When
        IActionResult action = _typeProductdController.Update(_typeProdcutKeybord.IdTypeProduct, _typeProdcutKeybord).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        TypeProduct editedTypeProductInDb = _context.TypeProducts.Find(_typeProdcutKeybord.IdTypeProduct);

        Assert.IsNotNull(editedTypeProductInDb);
        Assert.AreEqual(_typeProdcutKeybord.TypeProductName, editedTypeProductInDb.TypeProductName);
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseIdInUrlIsDifferent()
    {
        _typeProdcutScreen.TypeProductName = "OnlyFan";

        // When
        IActionResult action = _typeProductdController.Update(0, _typeProdcutScreen).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseBrandDoesNotExist()
    {
        // When
        IActionResult action = _typeProductdController.Update(_typeProdcutScreen.IdTypeProduct, _typeProdcutScreen).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.TypeProducts.RemoveRange(_context.TypeProducts);
        _context.SaveChanges();
    }
}