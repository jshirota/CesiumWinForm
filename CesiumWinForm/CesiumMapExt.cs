using System.Text.Json;

namespace CesiumWinForm;

public static class CesiumMapExt
{
    public static Task FlyTo(this CesiumMap map, double longitude, double latitude, double height,
        double heading = 0, double pitch = -90, double roll = 0, double duration = 1)
    {
        return map.ExecuteScriptAsync($"flyTo({longitude}, {latitude}, {height}, {heading}, {pitch}, {roll}, {duration})");
    }

    public static Task<int> AddCzml(this CesiumMap map, object czml, double? flyToDuration = null)
    {
        var json = czml is string str ? str : JsonSerializer.Serialize(czml);
        return map.ExecuteScriptAsync<int>($"addCzml({json}, {flyToDuration})");
    }

    public static Task RemoveCzml(this CesiumMap map, int? key = null)
    {
        return map.ExecuteScriptAsync($"removeCzml({(key)})");
    }

    public static Task Add3DTileset(this CesiumMap map, int assetId, double? flyToDuration = null)
    {
        return map.ExecuteScriptAsync($"add3DTileset({assetId}, {flyToDuration})");
    }
}
