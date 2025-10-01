using Microsoft.AspNetCore.Builder;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Kralizek.AutoFixture.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.EndToEnd;

public class Helper
{
    private Process _blazor;

    async public Task InitializeAsync()
    { 
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;

        var factory = new WebApplicationFactory<App.Program>();
        var client = factory.CreateClient();

        _blazor = Process.Start(new ProcessStartInfo {
            FileName = "dotnet",
            Arguments = "run --project BlazorApp/BlazorApp.csproj --launch-profile http --urls \"http://localhost:5109\"",
            WorkingDirectory = projectDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        _blazor.OutputDataReceived += (s, e) => Console.WriteLine($"[BLZ] {e.Data}");
        _blazor.ErrorDataReceived += (s, e) => Console.WriteLine($"[BLZ-ERR] {e.Data}");
        _blazor.Start();
        _blazor.BeginOutputReadLine();
        _blazor.BeginErrorReadLine();

        var tcs = new TaskCompletionSource();
        _blazor.OutputDataReceived += (s, e) =>
        {
            if (e.Data != null && e.Data.Contains("Now listening on"))
                tcs.TrySetResult();
        };
        await Task.WhenAny(tcs.Task, Task.Delay(10000));
    }

    public Task DisposeAsync()
    {
        if (!_blazor.HasExited) _blazor.Kill();
        return Task.CompletedTask;
    }
}
