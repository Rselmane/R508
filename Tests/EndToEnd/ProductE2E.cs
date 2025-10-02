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
using App.Models;
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

    [TestCleanup]
    public async Task DisposeAsync()
    {
        await _baseE2E.DisposeAsync();
    }

}
