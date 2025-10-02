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

namespace Tests.EndToEnd;

[TestClass]
[TestCategory("e2e")]
public class BaseE2E
{
    private Process _blazor;
    private IPlaywright _playwright;
    private IBrowser _browser;

    public HttpClient Client;
    public IPage Page;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;

        var factory = new WebApplicationFactory<App.Program>();
        Client = factory.CreateClient();

        _blazor = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project BlazorApp/BlazorApp.csproj --launch-profile http --urls \"http://localhost:5110\"",
            WorkingDirectory = projectDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        var tcs = new TaskCompletionSource();
        _blazor.OutputDataReceived += (s, e) =>
        {
            Console.WriteLine($"[BLZ] {e.Data}");
            if (e.Data != null && (e.Data.Contains("Now listening on") || e.Data.Contains("Hosting failed to start")))
                tcs.TrySetResult();
        };
        _blazor.ErrorDataReceived += (s, e) => Console.WriteLine($"[ERR] {e.Data}");
        _blazor.Start();
        _blazor.BeginOutputReadLine();
        _blazor.BeginErrorReadLine();

        await Task.WhenAny(tcs.Task, Task.Delay(10000));
        if (!_blazor.HasExited && !tcs.Task.IsCompleted)
        {
            _blazor.Kill();
            Assert.Fail("Le serveur Blazor n'a pas démarré à temps.");
        }

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        Page = await _browser.NewPageAsync();
    }

    [TestMethod]
    public async Task ShouldLoadHomePage()
    {
        // When : J'ouvre la page d'accueil
        await Page.GotoAsync("http://localhost:5110");
        // Then : Le titre de la page est correct
        var title = await Page.TitleAsync();
        Assert.AreEqual("Home", title);
    }

    [TestMethod]
    public async Task ShouldNavigateToProductsPage()
    {
        // When : J'ouvre la page d'accueil
        await Page.GotoAsync("http://localhost:5110");
        // And : Je clique sur le lien avec href = "products"
        await Page.ClickAsync("a[href='products']");
        // Then : L'url de la page est correcte
        Assert.AreEqual("http://localhost:5110/products", Page.Url);
    }

    [TestCleanup]
    public async Task DisposeAsync()
    {
        if (Page != null) await Page.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
        if (!_blazor.HasExited) _blazor.Kill();
    }

}
