using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using App.DTO;

namespace Tests.EndToEnd;

[TestClass]
[TestCategory("e2e")]
public class BrandE2E
{
    private BaseE2E _baseE2E;
    private List<BrandDTO> _existingBrands;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        _baseE2E = new BaseE2E();
        await _baseE2E.InitializeAsync();

        var resp = await _baseE2E.Client.GetAsync("/api/brand/all");
        resp.EnsureSuccessStatusCode();
        _existingBrands = await resp.Content.ReadFromJsonAsync<List<BrandDTO>>();
    }

    [TestMethod]
    public async Task ShouldGetAllBrands()
    {
        if (_existingBrands == null || _existingBrands.Count == 0)
            Assert.Inconclusive("Aucune marque n'existe dans la base");

        await _baseE2E.Page.GotoAsync("https://localhost:7777/brands");
        await _baseE2E.Page.WaitForSelectorAsync("#brandTable");
        var rows = await _baseE2E.Page.QuerySelectorAllAsync(".brandRow");

        Assert.AreEqual(_existingBrands.Count, rows.Count, "Le nombre de marques n'est pas égal");
        Assert.IsTrue(rows.Count > 0, "Il n'y a pas de lignes");

        var firstBrand = _existingBrands[0];
        var firstName = await rows[0].QuerySelectorAsync(".brandName").Result.InnerTextAsync();
        Assert.AreEqual(firstBrand.Name, firstName, "Le nom de la première marque n'est pas égal");
    }

    [TestMethod]
    public async Task ShouldGetBrandDetail()
    {
        if (_existingBrands == null || _existingBrands.Count == 0)
            Assert.Inconclusive("Aucune marque n'existe dans la base");

        var firstBrandDTO = _existingBrands[0];
        var brandDetail = await _baseE2E.Client.GetAsync("/api/brand/details/" + firstBrandDTO.Id)
            .Result.Content.ReadFromJsonAsync<BrandDTO>();

        if (brandDetail == null)
            Assert.Inconclusive("La marque n'a pas pu être récupérée");

        await _baseE2E.Page.GotoAsync("https://localhost:7777/brandEdit/" + firstBrandDTO.Id);
        await _baseE2E.Page.WaitForSelectorAsync("#brandDetail");

        var detailName = await _baseE2E.Page.Locator("#BrandName").InputValueAsync();
        Assert.AreEqual(brandDetail.Name, detailName, "Le nom de la marque n'est pas égal");
    }

    [TestCleanup]
    public async Task DisposeAsync()
    {
        await _baseE2E.DisposeAsync();
    }
}
