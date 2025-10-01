using App.Controllers;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Tests.EndToEnd;

[TestClass]
[TestCategory("e2e")]
public class BaseE2E
{
    private Helper _helper;
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        _helper = new Helper();
        await _helper.InitializeAsync();
        await Task.Delay(10000);
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        _page = await _browser.NewPageAsync();
        await _page.GotoAsync("http://localhost:5109");
    }

    [TestMethod]
    public async Task ShouldLoadHomePage()
    {
        // When : J'ouvre la page d'accueil
        var title = await _page.TitleAsync();
        // Then : Le titre de la page est correct
        Assert.AreEqual("Home", title);
    }

    [TestCleanup]
    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
        await _helper.DisposeAsync();
    }

}
