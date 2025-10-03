using App.Controllers;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using App.DTO;

namespace Tests.EndToEnd;

[TestClass]
[TestCategory("e2e")]
public class ProductE2E
{
    private BaseE2E _baseE2E;
    private List<ProductDTO> _existingProducts;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        _baseE2E = new BaseE2E();
        await _baseE2E.InitializeAsync();

        HttpResponseMessage resp = await _baseE2E.Client.GetAsync("/api/product/all");
        resp.EnsureSuccessStatusCode();
        _existingProducts = await resp.Content.ReadFromJsonAsync<List<ProductDTO>>();
    }

    [TestMethod]
    public async Task ShouldGetAllProducts()
    {
        // Granted : Des produits existent dans la base
        if (_existingProducts == null || _existingProducts.Count == 0)
            Assert.Inconclusive("Aucun produit n'existe dans la base");
        // When : On navigue sur la page produits
        await _baseE2E.Page.GotoAsync("https://localhost:7777/products");
        await _baseE2E.Page.WaitForSelectorAsync("#productTable");
        var rows = await _baseE2E.Page.QuerySelectorAllAsync(".productRow");
        // Then : On voit la liste des produits
        Assert.AreEqual(_existingProducts.Count, rows.Count, "Le nombre de lignes n'est pas égal");
        Assert.IsTrue(rows.Count > 0, "Il n'y a pas de lignes");
        ProductDTO firstProduct = _existingProducts[0];
        var firstName = await rows[0].QuerySelectorAsync(".productName").Result.InnerTextAsync();
        Assert.AreEqual(firstProduct.Name, firstName, "Le nom du premier produit n'est pas égal");
        var firstBrand = await rows[0].QuerySelectorAsync(".productBrand").Result.InnerTextAsync();
        Assert.AreEqual(firstProduct.Brand, firstBrand, "La marque du premier produit n'est pas égal");
        var firstType = await rows[0].QuerySelectorAsync(".productType").Result.InnerTextAsync();
        Assert.AreEqual(firstProduct.Type, firstType, "Le type du premier produit n'est pas égal");
    }

    [TestMethod]
    public async Task ShouldGetProductDetail()
    {
        // Granted : Le premier produit existe
        if (_existingProducts == null || _existingProducts.Count == 0)
            Assert.Inconclusive("Aucun produit n'existe dans la base");
        ProductDTO firstProductDTO = _existingProducts[0];
        ProductDetailDTO? firstProduct = _baseE2E.Client.GetAsync("/api/product/details/" + firstProductDTO.Id).Result.Content.ReadFromJsonAsync<ProductDetailDTO>().Result;
        if (firstProduct == null)
            Assert.Inconclusive("Le premier produit n'a pas pu être récupéré");
        // When : On navigue sur la page du détail du produit
        await _baseE2E.Page.GotoAsync("https://localhost:7777/ProductDetail/" + firstProductDTO.Id);
        await _baseE2E.Page.WaitForSelectorAsync("#productDetail");
        // Then : On voit le détail du produit
        var detailName = await _baseE2E.Page.Locator("#productName").InnerTextAsync();
        Assert.AreEqual(firstProduct.Name, detailName, "Le nom du produit n'est pas égal");
        var detailBrand = await _baseE2E.Page.Locator("#productBrand").InnerTextAsync();
        Assert.AreEqual(firstProduct.Brand, detailBrand, "La marque du produit n'est pas égal");
        var detailType = await _baseE2E.Page.Locator("#productType").InnerTextAsync();
        Assert.AreEqual(firstProduct.Type, detailType, "Le type du produit n'est pas égal");
        var detailDesc = await _baseE2E.Page.Locator("#productDesc").InnerTextAsync();
        Assert.AreEqual(firstProduct.Description, detailDesc, "La description du produit n'est pas égal");
        var detailStock = await _baseE2E.Page.Locator("#productStock").InnerTextAsync();
        Assert.AreEqual(firstProduct.Stock.ToString(), detailStock, "Le stock du produit n'est pas égal");
    }

    [TestMethod]
    public async Task ShouldGetProductEdit()
    {
        // Granted : Le premier produit existe
        if (_existingProducts == null || _existingProducts.Count == 0)
            Assert.Inconclusive("Aucun produit n'existe dans la base");
        ProductDTO firstProductDTO = _existingProducts[0];
        ProductDetailDTO? firstProduct = _baseE2E.Client.GetAsync("/api/product/details/" + firstProductDTO.Id).Result.Content.ReadFromJsonAsync<ProductDetailDTO>().Result;
        if (firstProduct == null)
            Assert.Inconclusive("Le premier produit n'a pas pu être récupéré");
        // When : On navigue sur la page d'édition du produit
        await _baseE2E.Page.GotoAsync("https://localhost:7777/productEdit/" + firstProductDTO.Id);
        await _baseE2E.Page.WaitForSelectorAsync("#EditForm");
        // Then : On voit le formulaire d'édition du produit
        var inputName = await _baseE2E.Page.Locator("#ProductName").InputValueAsync();
        Assert.AreEqual(firstProduct.Name, inputName, "Le nom du produit n'est pas égal");
        var inputBrand = await _baseE2E.Page.Locator("#Brand").InputValueAsync();
        Assert.AreEqual(firstProduct.Brand, inputBrand, "La marque du produit n'est pas égal");
        var inputType = await _baseE2E.Page.Locator("#Type").InputValueAsync();
        Assert.AreEqual(firstProduct.Type, inputType, "Le type du produit n'est pas égal");
        var inputDesc = await _baseE2E.Page.Locator("#Description").InputValueAsync();
        Assert.AreEqual(firstProduct.Description, inputDesc, "La description du produit n'est pas égal");
        var inputPhotoName = await _baseE2E.Page.Locator("#PhotoName").InputValueAsync();
        Assert.AreEqual(firstProduct.PhotoName, inputPhotoName, "Le nom de la photo du produit n'est pas égal");
        var inputPhotoUri = await _baseE2E.Page.Locator("#PhotoUri").InputValueAsync();
        Assert.AreEqual(firstProduct.PhotoUri, inputPhotoUri, "L'uri de la photo du produit n'est pas égal");
    }


    [TestCleanup]
    public async Task DisposeAsync()
    {
        await _baseE2E.DisposeAsync();
    }

}
