using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;

namespace CesiumWinForm;

internal sealed class WebServer : IDisposable
{
    public int Port { get; }

    private readonly IHost _host;

    public WebServer()
    {
        Port = GetFreePort();

        _host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseKestrel()
                    .UseUrls($"http://127.0.0.1:{Port}")
                    .Configure(app =>
                    {
                        app.UseDefaultFiles();
                        app.UseStaticFiles(new StaticFileOptions
                        {
                            FileProvider = new EmbeddedFileProvider(typeof(WebServer).Assembly, "CesiumWinForm.wwwroot")
                        });
                    });
            })
            .Build();

        _host.Start();
    }

    public void Dispose() => _host.Dispose();

    private static int GetFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
