namespace MapTileApi.Models;

public class MapRecord
{
    public int Id { get; set; }
    public string MapName { get; set; } = string.Empty;
    public int ZoomMin { get; set; }
    public int ZoomMax { get; set; }
    public double LatMin { get; set; }
    public double LatMax { get; set; }
    public double LonMin { get; set; }
    public double LonMax { get; set; }
    public string FolderPath { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}
public class RenameMapDto
{
    public string NewName { get; set; } = string.Empty;
}
