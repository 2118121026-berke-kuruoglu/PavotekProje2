using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using System.Net.Http;

namespace MapTileDownloader
{
    public partial class Form1 : Form
    {
        private GMarkerGoogle draggedMarker = null;
        private bool isDragging = false;

        PointLatLng? firstPoint = null;
        PointLatLng? secondPoint = null;

        GMapOverlay markersOverlay = new GMapOverlay("markers");
        GMapOverlay polygonsOverlay = new GMapOverlay("polygons");

        private readonly string apiBaseUrl = "http://localhost:5145/maps";
        private readonly string apiKey = "super-secret-api-key-12345";


        public Form1()
        {
            InitializeComponent();

            gmap.Overlays.Add(markersOverlay);
            gmap.Overlays.Add(polygonsOverlay);

            gmap.MapProvider = GMapProviders.OpenStreetMap;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gmap.Position = new PointLatLng(39.92, 32.85);
            gmap.MinZoom = 1;
            gmap.MaxZoom = 18;
            gmap.Zoom = 10;
            gmap.ShowCenter = false;
            gmap.DragButton = MouseButtons.Left;

            nudZoomMin.Minimum = 1;
            nudZoomMin.Maximum = 18;
            nudZoomMin.Value = 5;

            nudZoomMax.Minimum = 1;
            nudZoomMax.Maximum = 18;
            nudZoomMax.Value = 10;

            nudZoomMin.ValueChanged += nudZoomMin_ValueChanged;
            nudZoomMax.ValueChanged += nudZoomMax_ValueChanged;


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (isDragging || draggedMarker != null)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {

                if (firstPoint.HasValue && secondPoint.HasValue)
                {
                    return;
                }

                var point = gmap.FromLocalToLatLng(e.X, e.Y);

                if (firstPoint == null)
                {
                    firstPoint = point;
                    AddMarker(firstPoint.Value, "Baþlangýç");
                    UpdateLabels();
                }
                else if (secondPoint == null)
                {
                    secondPoint = point;
                    AddMarker(secondPoint.Value, "Bitiþ");
                    DrawRectangle(firstPoint.Value, secondPoint.Value);
                    UpdateLabels();
                }

                gmap.Refresh();
            }
            else if (e.Button == MouseButtons.Right)
            {
                GMarkerGoogle markerToRemove = null;
                double minDistance = double.MaxValue;

                int detectionRadius = 35;

                foreach (GMarkerGoogle marker in markersOverlay.Markers)
                {
                    System.Drawing.Point markerPixelLocation = new System.Drawing.Point(
                        (int)gmap.FromLatLngToLocal(marker.Position).X,
                        (int)gmap.FromLatLngToLocal(marker.Position).Y
                    );

                    double distance = Math.Sqrt(Math.Pow(e.Location.X - markerPixelLocation.X, 2) + Math.Pow(e.Location.Y - markerPixelLocation.Y, 2));

                    if (distance < detectionRadius && distance < minDistance)
                    {
                        minDistance = distance;
                        markerToRemove = marker;
                    }
                }

                if (markerToRemove != null)
                {
                    if (firstPoint.HasValue && firstPoint.Value == markerToRemove.Position)
                    {
                        markersOverlay.Markers.Clear();
                        firstPoint = null;
                        secondPoint = null;
                    }
                    else if (secondPoint.HasValue && secondPoint.Value == markerToRemove.Position)
                    {

                        markersOverlay.Markers.Remove(markerToRemove);
                        secondPoint = null;
                    }

                    polygonsOverlay.Polygons.Clear();

                    if (firstPoint.HasValue && secondPoint.HasValue)
                    {
                        DrawRectangle(firstPoint.Value, secondPoint.Value);
                    }

                    UpdateLabels();
                    gmap.Refresh();
                }
            }
        }

        private void AddMarker(PointLatLng point, string label)
        {
            var marker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);
            marker.ToolTipText = label;
            marker.ToolTipMode = MarkerTooltipMode.Always;
            markersOverlay.Markers.Add(marker);
        }

        private void DrawRectangle(PointLatLng p1, PointLatLng p2)
        {
            polygonsOverlay.Polygons.Clear();

            var points = new List<PointLatLng>
        {
            new PointLatLng(Math.Max(p1.Lat, p2.Lat), Math.Min(p1.Lng, p2.Lng)),
            new PointLatLng(Math.Max(p1.Lat, p2.Lat), Math.Max(p1.Lng, p2.Lng)),
            new PointLatLng(Math.Min(p1.Lat, p2.Lat), Math.Max(p1.Lng, p2.Lng)),
            new PointLatLng(Math.Min(p1.Lat, p2.Lat), Math.Min(p1.Lng, p2.Lng))
        };

            var rect = new GMapPolygon(points, "selection_rectangle")
            {
                Fill = new SolidBrush(Color.FromArgb(80, Color.LightBlue)),
                Stroke = new Pen(Color.Blue, 2)
            };

            polygonsOverlay.Polygons.Add(rect);
        }

        private void UpdateLabels()
        {
            if (firstPoint.HasValue)
                MARKER1.Text = $"Baþlangýç: {firstPoint.Value.Lat:F6}, {firstPoint.Value.Lng:F6}";
            else
                MARKER1.Text = "Baþlangýç: -";

            if (secondPoint.HasValue)
                MARKER2.Text = $"Bitiþ: {secondPoint.Value.Lat:F6}, {secondPoint.Value.Lng:F6}";
            else
                MARKER2.Text = "Bitiþ: -";
        }

        private void gmap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                draggedMarker = null;
                isDragging = false;
                gmap.DragButton = MouseButtons.Left;

                foreach (GMarkerGoogle marker in markersOverlay.Markers)
                {
                    System.Drawing.Point markerPixelLocation = new System.Drawing.Point(
                        (int)gmap.FromLatLngToLocal(marker.Position).X,
                        (int)gmap.FromLatLngToLocal(marker.Position).Y
                    );

                    Rectangle markerDetectionRect = new Rectangle(
                        markerPixelLocation.X - 35,
                        markerPixelLocation.Y - 35,
                        70,
                        70
                    );

                    if (markerDetectionRect.Contains(e.Location))
                    {
                        draggedMarker = marker;
                        isDragging = true;

                        gmap.DragButton = MouseButtons.None;
                        break;
                    }
                }
            }
        }

        private void gmap_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedMarker != null && e.Button == MouseButtons.Left)
            {
                var newPos = gmap.FromLocalToLatLng(e.X, e.Y);
                draggedMarker.Position = newPos;

                if (markersOverlay.Markers.Count == 2)
                {
                    firstPoint = markersOverlay.Markers[0].Position;
                    secondPoint = markersOverlay.Markers[1].Position;

                    DrawRectangle(firstPoint.Value, secondPoint.Value);
                    UpdateLabels();
                }

                gmap.Refresh();
            }
        }

        private void gmap_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            draggedMarker = null;

            gmap.DragButton = MouseButtons.Left;
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            if (!firstPoint.HasValue || !secondPoint.HasValue)
            {
                MessageBox.Show("Önce baþlangýç ve bitiþ noktalarýný seçin.");
                return;
            }

            int minZoom = (int)nudZoomMin.Value;
            int maxZoom = (int)nudZoomMax.Value;

            string mapName = txtMapName.Text.Trim();
            if (string.IsNullOrWhiteSpace(mapName))
            {
                MessageBox.Show("Harita ismi girilmelidir.");
                return;
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
            string sessionFolder = $"{mapName}_{timestamp}";
            string rootPath = Path.Combine("tiles", sessionFolder);
            Directory.CreateDirectory(rootPath);

            int totalTiles = 0;

            for (int zoom = minZoom; zoom <= maxZoom; zoom++)
            {
                var (x1, y1) = LatLngToTileXY(firstPoint.Value, zoom);
                var (x2, y2) = LatLngToTileXY(secondPoint.Value, zoom);

                totalTiles += (Math.Abs(x2 - x1) + 1) * (Math.Abs(y2 - y1) + 1);
            }

            int downloadedCount = 0;

            using (HttpClient client = new HttpClient())
            using (ProgressNotifierClient notifier = new ProgressNotifierClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MapTileDownloader/1.0");
                await notifier.ConnectAsync();

                for (int zoom = minZoom; zoom <= maxZoom; zoom++)
                {
                    var (x1, y1) = LatLngToTileXY(firstPoint.Value, zoom);
                    var (x2, y2) = LatLngToTileXY(secondPoint.Value, zoom);

                    int minX = Math.Min(x1, x2);
                    int maxX = Math.Max(x1, x2);
                    int minY = Math.Min(y1, y2);
                    int maxY = Math.Max(y1, y2);

                    for (int x = minX; x <= maxX; x++)
                    {
                        for (int y = minY; y <= maxY; y++)
                        {
                            string tileUrl = $"https://tile.openstreetmap.org/{zoom}/{x}/{y}.png";
                            string tileFolder = Path.Combine(rootPath, zoom.ToString(), x.ToString());
                            Directory.CreateDirectory(tileFolder);
                            string filePath = Path.Combine(tileFolder, $"{y}.png");

                            try
                            {
                                var imageData = await client.GetByteArrayAsync(tileUrl);
                                File.WriteAllBytes(filePath, imageData);

                                downloadedCount++;
                                double progress = (downloadedCount * 100.0) / totalTiles;
                                await notifier.SendProgressAsync(mapName, progress);
                                await Task.Delay(400);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Hata: {tileUrl} => {ex.Message}");
                            }
                        }
                    }
                }

                await notifier.CloseAsync();
            }

            // Enlem, boylam min/max hesaplama
            double latMin = Math.Min(firstPoint.Value.Lat, secondPoint.Value.Lat);
            double latMax = Math.Max(firstPoint.Value.Lat, secondPoint.Value.Lat);
            double lonMin = Math.Min(firstPoint.Value.Lng, secondPoint.Value.Lng);
            double lonMax = Math.Max(firstPoint.Value.Lng, secondPoint.Value.Lng);

            bool isSaved = await SendMapMetadataToApiAsync(mapName, (int)nudZoomMin.Value, (int)nudZoomMax.Value,
                latMin, latMax, lonMin, lonMax, rootPath);

            if (isSaved)
            {
                Console.WriteLine("Harita meta verisi baþarýyla API'ye gönderildi.");
            }
            else
            {
                Console.WriteLine("Harita meta verisi API'ye gönderilirken hata oluþtu.");
            }

            Console.WriteLine("Tüm tile'lar baþarýyla indirildi.");
        }

        private (int x, int y) LatLngToTileXY(PointLatLng point, int zoom)
        {
            int tileX = (int)((point.Lng + 180.0) / 360.0 * (1 << zoom));
            int tileY = (int)((1.0 - Math.Log(Math.Tan(point.Lat * Math.PI / 180.0) + 1.0 / Math.Cos(point.Lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            return (tileX, tileY);
        }

        private void nudZoomMin_ValueChanged(object sender, EventArgs e)
        {
            if (nudZoomMax.Value < nudZoomMin.Value)
            {
                nudZoomMax.Value = nudZoomMin.Value;
            }
            nudZoomMax.Minimum = nudZoomMin.Value;
        }

        private void nudZoomMax_ValueChanged(object sender, EventArgs e)
        {
            if (nudZoomMin.Value > nudZoomMax.Value)
            {
                nudZoomMin.Value = nudZoomMax.Value;
            }
            nudZoomMin.Maximum = nudZoomMax.Value;
        }


        private async Task<bool> SendMapMetadataToApiAsync(string mapName, int zoomMin, int zoomMax,
            double latMin, double latMax, double lonMin, double lonMax, string folderPath)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

                var data = new
                {
                    MapName = mapName,
                    ZoomMin = zoomMin,
                    ZoomMax = zoomMax,
                    LatMin = latMin,
                    LatMax = latMax,
                    LonMin = lonMin,
                    LonMax = lonMax,
                    FolderPath = folderPath,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(apiBaseUrl, content);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"API'ye veri gönderilirken hata: {ex.Message}");
                    return false;
                }
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
