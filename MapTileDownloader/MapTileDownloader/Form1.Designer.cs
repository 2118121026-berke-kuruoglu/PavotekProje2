namespace MapTileDownloader
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
            gmap = new GMap.NET.WindowsForms.GMapControl();
            nudZoomMin = new NumericUpDown();
            btnDownload = new Button();
            txtMapName = new TextBox();
            nudZoomMax = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            MARKER2 = new Label();
            MARKER1 = new Label();
            ((System.ComponentModel.ISupportInitialize)nudZoomMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudZoomMax).BeginInit();
            SuspendLayout();
            // 
            // gmap
            // 
            gmap.Bearing = 0F;
            gmap.CanDragMap = true;
            gmap.EmptyTileColor = Color.Navy;
            gmap.GrayScaleMode = false;
            gmap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            gmap.LevelsKeepInMemory = 5;
            gmap.Location = new Point(274, 12);
            gmap.MarkersEnabled = true;
            gmap.MaxZoom = 2;
            gmap.MinZoom = 2;
            gmap.MouseWheelZoomEnabled = true;
            gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            gmap.Name = "gmap";
            gmap.NegativeMode = false;
            gmap.PolygonsEnabled = true;
            gmap.RetryLoadTile = 0;
            gmap.RoutesEnabled = true;
            gmap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            gmap.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
            gmap.ShowTileGridLines = false;
            gmap.Size = new Size(662, 602);
            gmap.TabIndex = 0;
            gmap.Zoom = 0D;
            gmap.MouseClick += gmap_MouseClick;
            gmap.MouseDown += gmap_MouseDown;
            gmap.MouseMove += gmap_MouseMove;
            gmap.MouseUp += gmap_MouseUp;
            // 
            // nudZoomMin
            // 
            nudZoomMin.Location = new Point(24, 27);
            nudZoomMin.Maximum = new decimal(new int[] { 17, 0, 0, 0 });
            nudZoomMin.Name = "nudZoomMin";
            nudZoomMin.Size = new Size(74, 23);
            nudZoomMin.TabIndex = 1;
            nudZoomMin.ValueChanged += nudZoomMin_ValueChanged;
            // 
            // btnDownload
            // 
            btnDownload.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnDownload.Location = new Point(12, 322);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(241, 37);
            btnDownload.TabIndex = 2;
            btnDownload.Text = "İNDİR";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // txtMapName
            // 
            txtMapName.Location = new Point(12, 156);
            txtMapName.Name = "txtMapName";
            txtMapName.Size = new Size(241, 23);
            txtMapName.TabIndex = 4;
            // 
            // nudZoomMax
            // 
            nudZoomMax.Location = new Point(124, 27);
            nudZoomMax.Maximum = new decimal(new int[] { 17, 0, 0, 0 });
            nudZoomMax.Name = "nudZoomMax";
            nudZoomMax.Size = new Size(74, 23);
            nudZoomMax.TabIndex = 5;
            nudZoomMax.ValueChanged += nudZoomMax_ValueChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label1.Location = new Point(24, 7);
            label1.Name = "label1";
            label1.Size = new Size(72, 17);
            label1.TabIndex = 6;
            label1.Text = "Min Zoom";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label2.Location = new Point(124, 7);
            label2.Name = "label2";
            label2.Size = new Size(74, 17);
            label2.TabIndex = 7;
            label2.Text = "Max Zoom";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label3.Location = new Point(104, 27);
            label3.Name = "label3";
            label3.Size = new Size(14, 17);
            label3.TabIndex = 8;
            label3.Text = "/";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label4.Location = new Point(11, 132);
            label4.Name = "label4";
            label4.Size = new Size(105, 21);
            label4.TabIndex = 9;
            label4.Text = "HARİTA İSMİ";
            // 
            // MARKER2
            // 
            MARKER2.AutoSize = true;
            MARKER2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            MARKER2.Location = new Point(13, 229);
            MARKER2.Name = "MARKER2";
            MARKER2.Size = new Size(88, 21);
            MARKER2.TabIndex = 10;
            MARKER2.Text = "2.MARKER";
            // 
            // MARKER1
            // 
            MARKER1.AutoSize = true;
            MARKER1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            MARKER1.Location = new Point(11, 199);
            MARKER1.Name = "MARKER1";
            MARKER1.Size = new Size(88, 21);
            MARKER1.TabIndex = 11;
            MARKER1.Text = "1.MARKER";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(948, 626);
            Controls.Add(MARKER1);
            Controls.Add(MARKER2);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(nudZoomMax);
            Controls.Add(txtMapName);
            Controls.Add(btnDownload);
            Controls.Add(nudZoomMin);
            Controls.Add(gmap);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load_1;
            ((System.ComponentModel.ISupportInitialize)nudZoomMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudZoomMax).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl gmap;
        private NumericUpDown nudZoomMin;
        private Button btnDownload;
        private TextBox txtMapName;
        private NumericUpDown nudZoomMax;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label MARKER2;
        private Label MARKER1;
    }
}
