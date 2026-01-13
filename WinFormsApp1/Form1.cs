using CesiumWinForm;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await cesiumMap1.FlyTo(-120.0, 35.0, 5000.0);
            await cesiumMap1.FlyTo(-80.0, 45.0, 5000.0);
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
    }
}
