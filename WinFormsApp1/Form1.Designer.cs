namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cesiumMap1 = new CesiumWinForm.CesiumMap();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            dataGridView1 = new DataGridView();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // cesiumMap1
            // 
            cesiumMap1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cesiumMap1.Location = new Point(12, 12);
            cesiumMap1.Name = "cesiumMap1";
            cesiumMap1.Size = new Size(668, 513);
            cesiumMap1.TabIndex = 2;
            cesiumMap1.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJjY2RkNzVlZC1hNzBhLTRhOGQtYjdlMi1jNDU0ODEwYWYxZjQiLCJpZCI6MzA2MTgyLCJpYXQiOjE3NDgyMTY4NTd9.yhmXB-2U8x4bkxMFwdoy18SbPIN2_KirXJBnR1ha5VY";
            cesiumMap1.ViewerReady += cesiumMap1_ViewerReady;
            cesiumMap1.CameraMoved += cesiumMap1_CameraMoved;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 528);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1081, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(59, 17);
            toolStripStatusLabel1.Text = "Loading...";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(686, 12);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(383, 513);
            dataGridView1.TabIndex = 8;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1081, 550);
            Controls.Add(dataGridView1);
            Controls.Add(statusStrip1);
            Controls.Add(cesiumMap1);
            Name = "Form1";
            Text = "Form1";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private CesiumWinForm.CesiumMap cesiumMap1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private DataGridView dataGridView1;
    }
}
