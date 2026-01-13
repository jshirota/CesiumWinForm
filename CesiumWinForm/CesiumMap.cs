using CesiumWinForm;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;
using System.Text.Json;

namespace CesiumWinForm
{
    [DesignerCategory("Code")]
    [ToolboxItem(true)]
    [DefaultProperty(nameof(Dock))]
    [DefaultEvent(nameof(IsViewerReady))]
    public sealed class CesiumMap : UserControl
    {
        private WebServer? _server;
        private WebView2? _webView;
        private Panel? _designPlaceholder;

        public event EventHandler? ViewerReady;

        [Category("Cesium")]
        [Description("Cesium access token")]
        [DefaultValue("")]
        public string Token { get; set; } = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsViewerReady { get; private set; }

        public CesiumMap()
        {
            SuspendLayout();

            if (IsDesignMode(this))
                CreateDesignTimeView();
            else
                CreateRuntimeView();

            ResumeLayout();
        }

        public static bool IsDesignMode(Control c)
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime || c.Site?.DesignMode == true;
        }

        private void CreateDesignTimeView()
        {
            _designPlaceholder = new Panel { Dock = DockStyle.Fill };
            Controls.Add(_designPlaceholder);
        }

        private void CreateRuntimeView()
        {
            if (_designPlaceholder != null)
            {
                Controls.Remove(_designPlaceholder);
                _designPlaceholder.Dispose();
            }
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
            _webView.Source = new Uri($"http://127.0.0.1:{_server!.Port}/index.html?token={Uri.EscapeDataString(Token)}");
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var message = e.TryGetWebMessageAsString();
            var result = JsonSerializer.Deserialize<ScriptResult<object>>(message)!;

            if (result.name == "ready")
            {
                if (!IsViewerReady)
                {
                    IsViewerReady = true;
                    ViewerReady?.Invoke(this, EventArgs.Empty);
                }
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

        public async Task<T> ExecuteScriptAsync<T>(string name, string script)
        {
            if (_webView?.CoreWebView2 is null)
                return default!;

            var taskCompletionSource = new TaskCompletionSource<T>();

            void Handler(object sender, CoreWebView2WebMessageReceivedEventArgs args)
            {
                var result = JsonSerializer.Deserialize<ScriptResult<T>>(args.TryGetWebMessageAsString());
                if (result?.name == name)
                {
                    taskCompletionSource.TrySetResult(result.data);
                    _webView.CoreWebView2.WebMessageReceived -= Handler!;
                }
            }

            _webView.CoreWebView2.WebMessageReceived += Handler!;

            await _webView.ExecuteScriptAsync(script);
            return await taskCompletionSource.Task;
        }

        public async Task ExecuteScriptAsync(string name, string script)
        {
            await ExecuteScriptAsync<object>(name, script);
        }

        record ScriptResult<T>(string name, T data);
    }
}
