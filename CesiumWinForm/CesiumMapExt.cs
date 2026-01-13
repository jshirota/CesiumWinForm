using CesiumWinForm;

namespace CesiumWinForm
{
    public static class CesiumMapExt
    {
        public static async Task FlyTo(this CesiumMap map, double longitude, double latitude, double height)
        {
            string script = $@"
                viewer.camera.flyTo({{
                    destination: Cesium.Cartesian3.fromDegrees({longitude}, {latitude}, {height}),
                    duration: 2.0,
                    complete: () => notify('flyTo'),
                }});
            ";

            await map.ExecuteScriptAsync("flyTo", script);
        }
    }
}
