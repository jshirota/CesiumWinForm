using System.Text.Json;

namespace CesiumWinForm;

public static class CesiumMapExt
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static Task FlyTo(this CesiumMap map, double longitude, double latitude, double height,
        double heading = 0, double pitch = -90, double roll = 0, double duration = 1)
    {
        return map.ExecuteScriptAsync(
            $"flyTo({F(longitude)}, {F(latitude)}, {F(height)}, {F(heading)}, {F(pitch)}, {F(roll)}, {F(duration)})");
    }

    public static Task<int> AddCzml(this CesiumMap map, object czml, double? flyToDuration = 0)
    {
        var json = czml is string str ? str : JsonSerializer.Serialize(czml, _jsonOptions);
        return map.ExecuteScriptAsync<int>(
            $"addCzml({json}, {F(flyToDuration)})");
    }

    public static Task<int> AddGeoJson(this CesiumMap map, object geojson, double? flyToDuration = 0, int? cameraHeight = null,
        string? markerUrl = null, int? markerWidth = null, int? markerHeight = null, bool anchorBottom = true,
        string stroke = "#FFFFFF", int strokeWidth = 1, double strokeAlpha = 1,
        string fill = "#FFFFFF", double fillAlpha = 0.5)
    {
        var json = geojson is string str ? str : JsonSerializer.Serialize(geojson, _jsonOptions);
        return map.ExecuteScriptAsync<int>(
            $"addGeoJson({json}, {F(flyToDuration)}, {F(cameraHeight)}, {F(markerUrl)}, {F(markerWidth)}, {F(markerHeight)}, {F(anchorBottom)}, {F(stroke)}, {F(strokeWidth)}, {F(strokeAlpha)}, {F(fill)}, {F(fillAlpha)})");
    }

    public static Task Remove(this CesiumMap map, int key)
    {
        return map.ExecuteScriptAsync(
            $"remove({F(key)})");
    }

    public static Task RemoveAll(this CesiumMap map)
    {
        return map.ExecuteScriptAsync(
            $"removeAll()");
    }

    public static Task Add3DTileset(this CesiumMap map, int assetId, double? flyToDuration = 0)
    {
        return map.ExecuteScriptAsync(
            $"add3DTileset({F(assetId)}, {F(flyToDuration)})");
    }

    private static string F<T>(T? value)
    {
        if (value is null)
            return "null";
        if (value is bool b)
            return b ? "true" : "false";
        return value is string s ? $"'{s}'" : $"{value}";
    }
}
