using Microsoft.Data.Sqlite;
using MapTileApi.Models;
using System.IO;

namespace MapTileApi.Services;

public class DatabaseService
{
    private readonly string _dbPath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..")), "Sqlite", "maptiles.db");

    public List<MapRecord> GetAllMaps()
    {
        var result = new List<MapRecord>();
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM DATA";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine($"Id={reader.GetInt32(0)}, MapName={reader.GetString(1)}, ZoomMin={reader.GetInt32(2)}, ZoomMax={reader.GetInt32(3)}, LatMin={reader.GetDouble(4)}, LatMax={reader.GetDouble(5)}, LonMin={reader.GetDouble(6)}, LonMax={reader.GetDouble(7)}, FolderPath={reader.GetString(8)}, CreatedAt={reader.GetString(9)}");

            result.Add(new MapRecord
            {
                Id = reader.GetInt32(0),
                MapName = reader.GetString(1),
                ZoomMin = reader.GetInt32(2),
                ZoomMax = reader.GetInt32(3),
                LatMin = reader.GetDouble(4),
                LatMax = reader.GetDouble(5),
                LonMin = reader.GetDouble(6),
                LonMax = reader.GetDouble(7),
                FolderPath = reader.GetString(8),
                CreatedAt = reader.GetString(9)
            });
        }

        return result;
    }


    public MapRecord InsertMap(MapRecord map)
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO DATA 
        (map_name, zoom_min, zoom_max, lat_min, lat_max, lon_min, lon_max, folder_path, created_at)
        VALUES ($name, $zmin, $zmax, $latmin, $latmax, $lonmin, $lonmax, $path, $created);
    ";

        command.Parameters.AddWithValue("$name", map.MapName);
        command.Parameters.AddWithValue("$zmin", map.ZoomMin);
        command.Parameters.AddWithValue("$zmax", map.ZoomMax);
        command.Parameters.AddWithValue("$latmin", map.LatMin);
        command.Parameters.AddWithValue("$latmax", map.LatMax);
        command.Parameters.AddWithValue("$lonmin", map.LonMin);
        command.Parameters.AddWithValue("$lonmax", map.LonMax);
        command.Parameters.AddWithValue("$path", map.FolderPath);
        command.Parameters.AddWithValue("$created", map.CreatedAt);

        command.ExecuteNonQuery();

        var idCommand = connection.CreateCommand();
        idCommand.CommandText = "SELECT last_insert_rowid()";
        var newId = (long)idCommand.ExecuteScalar();

        map.Id = (int)newId;
        return map;
    }

    public bool DeleteMap(int id)
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT map_name, folder_path FROM DATA WHERE id = $id";
        selectCmd.Parameters.AddWithValue("$id", id);

        using var reader = selectCmd.ExecuteReader();
        if (!reader.Read())
            return false;

        string mapName = reader.GetString(0);
        string folderPath = reader.GetString(1);

        // DB’de kayıtlı klasörü sil
        if (Directory.Exists(folderPath))
        {
            try
            {
                Directory.Delete(folderPath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB kayıtlı klasör silme hatası: {ex.Message}");
            }
        }

        string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\.."));
        string tilesRootPath = Path.Combine(projectRoot, "MapTileDownloader", "MapTileDownloader", "bin", "Debug", "net8.0-windows", "tiles");

        if (Directory.Exists(tilesRootPath))
        {
            var matchDir = Directory.GetDirectories(tilesRootPath)
                .FirstOrDefault(dir => Path.GetFileName(dir).Contains(mapName, StringComparison.OrdinalIgnoreCase));

            if (matchDir != null && Directory.Exists(matchDir))
            {
                try
                {
                    Directory.Delete(matchDir, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Tiles klasörü silme hatası: {ex.Message}");
                }
            }
        }

        // DB kaydını sil
        var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM DATA WHERE id = $id";
        deleteCmd.Parameters.AddWithValue("$id", id);

        int rowsAffected = deleteCmd.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    public bool RenameMap(int id, string newMapName)
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT map_name FROM DATA WHERE id = $id";
        selectCmd.Parameters.AddWithValue("$id", id);

        using var reader = selectCmd.ExecuteReader();
        if (!reader.Read())
            return false;

        string oldMapName = reader.GetString(0);

        string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\.."));
        string tilesRootPath = Path.Combine(projectRoot, "MapTileDownloader", "MapTileDownloader", "bin", "Debug", "net8.0-windows", "tiles");

        string oldFolderPath = Directory.GetDirectories(tilesRootPath)
            .FirstOrDefault(dir => Path.GetFileName(dir).Contains(oldMapName));

        if (oldFolderPath == null)
        {
            Console.WriteLine($"Eşleşen klasör bulunamadı: {oldMapName}");
            return false;
        }

        string newFolderPath = Path.Combine(tilesRootPath, newMapName);

        try
        {
            Directory.Move(oldFolderPath, newFolderPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Klasör yeniden adlandırma hatası: {ex.Message}");
            return false;
        }

        var updateCmd = connection.CreateCommand();
        updateCmd.CommandText = @"
        UPDATE DATA
        SET
            map_name = $mapName,
            folder_path = $folderPath
        WHERE id = $id
    ";

        updateCmd.Parameters.AddWithValue("$mapName", newMapName);
        updateCmd.Parameters.AddWithValue("$folderPath", Path.Combine("tiles", newMapName));
        updateCmd.Parameters.AddWithValue("$id", id);

        int rows = updateCmd.ExecuteNonQuery();

        return rows > 0;
    }
    public bool UserExists(string username)
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM USERS WHERE username = $username";
        cmd.Parameters.AddWithValue("$username", username);

        var count = Convert.ToInt32(cmd.ExecuteScalar());
        return count > 0;
    }

    public void AddUser(string username, string passwordHash)
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO USERS (username, password) VALUES ($username, $password)";
        cmd.Parameters.AddWithValue("$username", username);
        cmd.Parameters.AddWithValue("$password", passwordHash);

        cmd.ExecuteNonQuery();
    }

    public User? GetUserByUsername(string username)
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM USERS WHERE username = $username";
        cmd.Parameters.AddWithValue("$username", username);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            PasswordHash = reader.GetString(2)
        };
    }
}
