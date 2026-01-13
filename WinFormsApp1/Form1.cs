using CesiumWinForm;
using System.Text.Json;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void cesiumMap1_ViewerReady(object sender, EventArgs e)
        {
            if (cesiumMap1.Token == "")
            {
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = "Please provide the Cesium ion Access Token.";
            }
            else
            {
                toolStripStatusLabel1.Text = "Ready";
            }
        }

        private void cesiumMap1_CameraMoved(object sender, CameraMovedEventArgs e)
        {
            this.Text = JsonSerializer.Serialize(e.Camera);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await cesiumMap1.FlyTo(-120.0, 35.0, 5000.0);
            await cesiumMap1.FlyTo(-80.0, 45.0, 5000.0);
            await cesiumMap1.Add3DTileset(354307, 1);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await cesiumMap1.AddCzml("""
                [
                  {
                    id: "document",
                    name: "CZML Geometries: Spheres and Ellipsoids",
                    version: "1.0",
                  },
                  {
                    id: "blueEllipsoid",
                    name: "blue ellipsoid",
                    position: {
                      cartographicDegrees: [-114.0, 40.0, 300000.0],
                    },
                    ellipsoid: {
                      radii: {
                        cartesian: [200000.0, 200000.0, 300000.0],
                      },
                      fill: true,
                      material: {
                        solidColor: {
                          color: {
                            rgba: [0, 0, 255, 255],
                          },
                        },
                      },
                    },
                  },
                  {
                    id: "redSphere",
                    name: "Red sphere with black outline",
                    position: {
                      cartographicDegrees: [-107.0, 40.0, 300000.0],
                    },
                    ellipsoid: {
                      radii: {
                        cartesian: [300000.0, 300000.0, 300000.0],
                      },
                      fill: true,
                      material: {
                        solidColor: {
                          color: {
                            rgba: [255, 0, 0, 100],
                          },
                        },
                      },
                      outline: true,
                      outlineColor: {
                        rgbaf: [0, 0, 0, 1],
                      },
                    },
                  },
                  {
                    id: "yellowEllipsoid",
                    name: "ellipsoid with yellow outline",
                    position: {
                      cartographicDegrees: [-100.0, 40.0, 300000.0],
                    },
                    ellipsoid: {
                      radii: {
                        cartesian: [200000.0, 200000.0, 300000.0],
                      },
                      fill: false,
                      outline: true,
                      outlineColor: {
                        rgba: [255, 255, 0, 255],
                      },
                      slicePartitions: 24,
                      stackPartitions: 36,
                    },
                  },
                ]
                """);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await cesiumMap1.RemoveCzml();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
        }
    }
}
