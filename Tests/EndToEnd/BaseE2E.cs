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
using Microsoft.AspNetCore.Hosting;
using App.DTO;
using System.Net.Http.Json;

namespace Tests.EndToEnd;

[TestClass]
[TestCategory("e2e")]
public class BaseE2E
{
    private Process _api;
    private Process _blazor;
    private IPlaywright _playwright;
    private IBrowser _browser;

    public HttpClient Client;
    public IPage Page;

    private async Task FreePortAsync(string port)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c netstat -ano | findstr :{port}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 5)
                    {
                        var pid = parts[4];
                        var killPsi = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c taskkill /PID {pid} /F",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using (var killProcess = Process.Start(killPsi))
                        {
                            string killOutput = await killProcess.StandardOutput.ReadToEndAsync();
                            await killProcess.WaitForExitAsync();
                            Console.WriteLine($"Killed process {pid} on port {port}: {killOutput}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error freeing port {port}: {ex.Message}");
        }
    }

    private async Task StartApiAsync(string projectdir)
    {
        await FreePortAsync("7008");
        _api = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project App/App.csproj --launch-profile https --urls \"https://localhost:7008\"",
            WorkingDirectory = projectdir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        var tcs = new TaskCompletionSource();
        _api.OutputDataReceived += (s, e) =>
        {
            Console.WriteLine($"[API] {e.Data}");
            if (e.Data != null && (e.Data.Contains("Now listening on") || e.Data.Contains("Hosting failed to start")))
                tcs.TrySetResult();
        };
        _api.ErrorDataReceived += (s, e) => Console.WriteLine($"[ERR] {e.Data}");
        _api.Start();
        _api.BeginOutputReadLine();
        _api.BeginErrorReadLine();

        await Task.WhenAny(tcs.Task, Task.Delay(10000));
    }

    private async Task StartBlazorAsync(string projectdir)
    {
        await FreePortAsync("7777");
        _blazor = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --project BlazorApp/BlazorApp.csproj --launch-profile https --urls \"https://localhost:7777\"",
            WorkingDirectory = projectdir,
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
    }

    [TestInitialize]
    public async Task InitializeAsync()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;

        var factory = new WebApplicationFactory<App.Program>();
        Client = factory.CreateClient();

        await StartApiAsync(projectDirectory);
        await StartBlazorAsync(projectDirectory);

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
        await Page.GotoAsync("https://localhost:7777");
        // Then : Le titre de la page est correct
        var title = await Page.TitleAsync();
        Assert.AreEqual("Home", title);
    }

    [TestMethod]
    public async Task ShouldNavigateToProductsPage()
    {
        // When : J'ouvre la page d'accueil
        await Page.GotoAsync("https://localhost:7777");
        // And : Je clique sur le lien avec href = "products"
        await Page.ClickAsync("a[href='products']");
        // Then : L'url de la page est correcte
        Assert.AreEqual("https://localhost:7777/products", Page.Url);
    }

    [TestMethod]
    public async Task ShouldNavigateToBrandsPage()
    {
        // When : J'ouvre la page d'accueil
        await Page.GotoAsync("https://localhost:7777");
        // And : Je clique sur le lien avec href = "brands"
        await Page.ClickAsync("a[href='brands']");
        // Then : L'url de la page est correcte
        Assert.AreEqual("https://localhost:7777/brands", Page.Url);
    }

    [TestMethod]
    public async Task ShouldNavigateToTypesPage()
    {
        // When : J'ouvre la page d'accueil
        await Page.GotoAsync("https://localhost:7777");
        // And : Je clique sur le lien avec href = "typeProducts"
        await Page.ClickAsync("a[href='typeProducts']");
        // Then : L'url de la page est correcte
        Assert.AreEqual("https://localhost:7777/typeProducts", Page.Url);
    }

    [TestCleanup]
    public async Task DisposeAsync()
    {
        if (Page != null) await Page.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
        Console.WriteLine("Disposing processes...");
        if (!_api.HasExited) _api.Kill();
        if (!_blazor.HasExited) _blazor.Kill();
        await FreePortAsync("7008");
        await FreePortAsync("7777");
    }

}
