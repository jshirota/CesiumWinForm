using CesiumWinForm;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    private static readonly HttpClient _httpClient = new();
    private static readonly string _baseUrl = "https://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer";

    public Form1()
    {
        InitializeComponent();
    }

    private async void cesiumMap1_ViewerReady(object sender, EventArgs e)
    {
        toolStripStatusLabel1.Text = "Ready";
        //await LoadFeatures(0, f => new City { feature = f });
        await LoadFeatures(2, f => new State { feature = f });
    }

    private void cesiumMap1_CameraMoved(object sender, CameraMovedEventArgs e)
    {
        Text = JsonSerializer.Serialize(e.Camera);
    }

    private async Task LoadFeatures<T>(int layerId, Func<Feature, T> factory) where T : IFeature
    {
        var url = $"{_baseUrl}/{layerId}/query?where=1%3E0&outFields=*&returnGeometry=true&f=geojson";
        var featureset = await _httpClient.GetFromJsonAsync<FeatureSet>(url);
        dataGridView1.DataSource = featureset!.features.Select(factory).OrderBy(x => x.Name).ToArray();
    }

    private IFeature? currentFeature;

    private async void dataGridView1_SelectionChanged(object sender, EventArgs e)
    {
        if (dataGridView1.SelectedRows.OfType<DataGridViewRow>().FirstOrDefault()?.DataBoundItem is not IFeature feature
            || feature == currentFeature)
            return;
        currentFeature = feature;
        await cesiumMap1.RemoveAll();
        await cesiumMap1.AddGeoJson(new FeatureSet([feature.feature]), 1, feature.ZoomHeight,
            markerUrl: "https://maps.gstatic.com/mapfiles/api-3/images/spotlight-poi2.png",
            stroke: "#00FFFF", fill: "#00FFFF", fillAlpha: 0.1);
    }
}

public interface IFeature
{
    public Feature feature { get; init; }
    public int? ZoomHeight { get; }
    public string Name { get; }
}

public record City : IFeature
{
    [Browsable(false)]
    public required Feature feature { get; init; }
    [Browsable(false)]
    public int? ZoomHeight => 100000;
    public string Name => feature.properties["areaname"].GetValue<string>();
    public string State => feature.properties["st"].GetValue<string>();
    public int Population => feature.properties["pop2000"].GetValue<int>();
}

public record State : IFeature
{
    [Browsable(false)]
    public required Feature feature { get; init; }
    [Browsable(false)]
    public int? ZoomHeight => null;
    public string Name => feature.properties["state_name"].GetValue<string>();
    public int Population => feature.properties["pop2000"].GetValue<int>();
    public double Density => feature.properties["pop00_sqmi"].GetValue<double>();
}

public record FeatureSet(Feature[] features, string type = "FeatureCollection");

public record Feature(string type, int id, object geometry, Dictionary<string, JsonValue> properties);
