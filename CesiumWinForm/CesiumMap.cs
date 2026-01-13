using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;
using System.Text.Json;

namespace CesiumWinForm;

[DesignerCategory("Code")]
[ToolboxItem(true)]
[DefaultProperty(nameof(Dock))]
[DefaultEvent(nameof(IsViewerReady))]
public sealed class CesiumMap : UserControl
{
    private WebServer? _server;
    private WebView2? _webView;
    private readonly Panel? _designPlaceholder;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [Category("Cesium")]
    [Description("Occurs when the viewer is ready")]
    public event EventHandler? ViewerReady;

    [Category("Cesium")]
    [Description("Occurs when the camera is moved")]
    public event EventHandler<CameraMovedEventArgs>? CameraMoved;

    [Category("Cesium")]
    [Description("Cesium ion access token")]
    [DefaultValue("")]
    public string Token { get; set; } = string.Empty;

    [Category("Cesium")]
    [Description("Imagery asset id")]
    [DefaultValue(3)]
    public int ImageryAssetId { get; set; } = 3;

    [Category("Cesium")]
    [Description("Animation control visibility")]
    [DefaultValue(false)]
    public bool Animation { get; set; } = false;

    [Category("Cesium")]
    [Description("Base layer picker control visibility")]
    [DefaultValue(false)]
    public bool BaseLayerPicker { get; set; } = false;

    [Category("Cesium")]
    [Description("Geocoder control visibility")]
    [DefaultValue(true)]
    public bool Geocoder { get; set; } = true;

    [Category("Cesium")]
    [Description("Timeline control visibility")]
    [DefaultValue(false)]
    public bool Timeline { get; set; } = false;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsViewerReady { get; private set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Camera Camera { get; private set; } = new(0, 0, 0, 0, 0, 0);

    public CesiumMap()
    {
        SuspendLayout();

        if (IsDesignMode(this))
        {
            _designPlaceholder = new Panel { Dock = DockStyle.Fill };
            Controls.Add(_designPlaceholder);
        }
        else
        {
            if (_designPlaceholder != null)
            {
                Controls.Remove(_designPlaceholder);
                _designPlaceholder.Dispose();
            }
        }

        ResumeLayout();
    }

    private static bool IsDesignMode(Control control)
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime || control.Site?.DesignMode == true;
    }

    protected override async void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (IsDesignMode(this) || _webView is not null)
            return;

        _server = new WebServer();
        _webView = new WebView2 { Dock = DockStyle.Fill };

        Controls.Add(_webView);
        await _webView.EnsureCoreWebView2Async();

        _webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        _webView.Source = new Uri($"http://127.0.0.1:{_server!.Port}/index.html?token={Uri.EscapeDataString(Token)}"
            + $"&imageryAssetId={ImageryAssetId}"
            + $"&animation={(Animation ? "true" : "false")}"
            + $"&baseLayerPicker={(BaseLayerPicker ? "true" : "false")}"
            + $"&geocoder={(Geocoder ? "true" : "false")}"
            + $"&timeline={(Timeline ? "true" : "false")}");
    }

    private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var json = e.WebMessageAsJson;
        var result = JsonSerializer.Deserialize<ScriptResult<JsonElement>>(json)!;

        if (result.Id == "ready")
        {
            if (!IsViewerReady)
            {
                IsViewerReady = true;
                ViewerReady?.Invoke(this, EventArgs.Empty);
            }
        }
        else if (result.Id == "moved")
        {
            var camera = result.Data.Deserialize<Camera>(_jsonOptions)!;
            this.Camera = camera;
            CameraMoved?.Invoke(this, new CameraMovedEventArgs(camera));
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _server?.Dispose();
            _webView?.Dispose();
        }

        base.Dispose(disposing);
    }

    public async Task<T> ExecuteScriptAsync<T>(string script)
    {
        if (_webView?.CoreWebView2 is null)
            return default!;

        var id = $"_{Guid.NewGuid():N}";
        var taskCompletionSource = new TaskCompletionSource<T>();

        void Handler(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            var json = args.WebMessageAsJson;
            var result = JsonSerializer.Deserialize<ScriptResult<JsonElement>>(json);
            if (result?.Id == id)
            {
                taskCompletionSource.TrySetResult(result.Data.Deserialize<T>(_jsonOptions)!);
                _webView.CoreWebView2.WebMessageReceived -= Handler!;
            }
        }

        _webView.CoreWebView2.WebMessageReceived += Handler!;

        await _webView.ExecuteScriptAsync($"{script}.then(data => notify('{id}', data));");
        return await taskCompletionSource.Task;
    }

    public async Task ExecuteScriptAsync(string script)
    {
        await ExecuteScriptAsync<object>(script);
    }
}

internal record ScriptResult<T>(string Id, T Data);

public record Camera(double Longitude, double Latitude, double Height, double Heading, double Pitch, double Roll);

public sealed class CameraMovedEventArgs(Camera camera) : EventArgs { public Camera Camera { get; } = camera; }
