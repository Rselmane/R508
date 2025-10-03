using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.DTO;

namespace Tests.EndToEnd;

[TestClass]
[TestCategory("e2e")]
public class TypeProductE2E
{
    private BaseE2E _baseE2E;
    private List<TypeProductDTO> _existingTypes;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        _baseE2E = new BaseE2E();
        await _baseE2E.InitializeAsync();

        var resp = await _baseE2E.Client.GetAsync("/api/typeproduct/all");
        resp.EnsureSuccessStatusCode();
        _existingTypes = await resp.Content.ReadFromJsonAsync<List<TypeProductDTO>>();
    }

    [TestMethod]
    public async Task ShouldGetAllTypeProducts()
    {
        if (_existingTypes == null || _existingTypes.Count == 0)
            Assert.Inconclusive("Aucun type de produit n'existe dans la base");

        await _baseE2E.Page.GotoAsync("https://localhost:7777/typeproducts");
        await _baseE2E.Page.WaitForSelectorAsync("#typeProductTable");
        var rows = await _baseE2E.Page.QuerySelectorAllAsync(".typeProductRow");

        Assert.AreEqual(_existingTypes.Count, rows.Count, "Le nombre de types n'est pas égal");
        Assert.IsTrue(rows.Count > 0, "Il n'y a pas de lignes");

        var firstType = _existingTypes[0];
        var firstName = await rows[0].QuerySelectorAsync(".typeProductName").Result.InnerTextAsync();
        Assert.AreEqual(firstType.Name, firstName, "Le nom du premier type n'est pas égal");
    }

    [TestMethod]
    public async Task ShouldGetTypeProductDetail()
    {
        if (_existingTypes == null || _existingTypes.Count == 0)
            Assert.Inconclusive("Aucun type de produit n'existe dans la base");

        var firstTypeDTO = _existingTypes[0];
        var typeDetail = await _baseE2E.Client.GetAsync("/api/typeproduct/details/" + firstTypeDTO.Id)
            .Result.Content.ReadFromJsonAsync<TypeProductDTO>();

        if (typeDetail == null)
            Assert.Inconclusive("Le type de produit n'a pas pu être récupéré");

        await _baseE2E.Page.GotoAsync("https://localhost:7777/typeproductEdit/" + firstTypeDTO.Id);
        await _baseE2E.Page.WaitForSelectorAsync("#typeProductDetail");

        var detailName = await _baseE2E.Page.Locator("#TypeProductName").InputValueAsync();
        Assert.AreEqual(typeDetail.Name, detailName, "Le nom du type de produit n'est pas égal");
    }

    [TestCleanup]
    public async Task DisposeAsync()
    {
        await _baseE2E.DisposeAsync();
    }
}
