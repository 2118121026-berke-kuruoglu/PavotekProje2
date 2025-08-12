using MapTileApi.Models;
using MapTileApi.Services;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5071")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseRouting();

app.UseCors();


var apiKey = builder.Configuration["ApiKey"];
var db = new DatabaseService();

app.Use(async (context, next) =>
{
    // API Key kontrolü CORS'tan sonra olmalý
    var apiKeyHeader = context.Request.Headers["X-API-KEY"].FirstOrDefault();
    var apiKeyQuery = context.Request.Query["X-API-KEY"].FirstOrDefault();
    var extractedApiKey = apiKeyHeader ?? apiKeyQuery;

    if (string.IsNullOrEmpty(extractedApiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("API Key gerekli.");
        return;
    }

    if (!apiKey.Equals(extractedApiKey))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("API Key geçersiz.");
        return;
    }

    await next();
});

app.MapGet("/maps", () => Results.Ok(db.GetAllMaps()));

app.MapPost("/maps", (MapRecord map) =>
{
    var inserted = db.InsertMap(map);
    return Results.Created($"/maps/{inserted.Id}", inserted);
});

app.MapPost("/api/auth/register", (UserRegisterDto dto) =>
{
    if (db.UserExists(dto.Username))
        return Results.BadRequest(new { message = "Kullanýcý zaten mevcut." });

    var hashed = HashPassword(dto.Password);
    db.AddUser(dto.Username, hashed);

    return Results.Ok(new { message = "Kayýt baþarýlý." });
});

app.MapPost("/api/auth/login", (UserLoginDto dto) =>
{
    var user = db.GetUserByUsername(dto.Username);
    if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
        return Results.Unauthorized();

    return Results.Ok(new { message = "Giriþ baþarýlý." });
});

app.MapGet("/api/maps/{id:int}/tile/{z:int}/{x:int}/{y:int}", async (int id, int z, int x, int y) =>
{

    string yol = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..")), "Sqlite", "maptiles.db");

    using var connection = new SqliteConnection($"Data Source={yol}");
    await connection.OpenAsync();

    var command = connection.CreateCommand();
    command.CommandText = @"SELECT folder_path FROM DATA WHERE id = $id";
    command.Parameters.AddWithValue("$id", id);

    var folderPath = (string?)await command.ExecuteScalarAsync();
    if (string.IsNullOrWhiteSpace(folderPath))
        return Results.NotFound("Map not found");

    string folderPathClean = folderPath.Replace('/', '\\');

    // Proje ana klasörünün 5 üst klasörüne çýkýlýyor
    string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\.."));

    // tiles klasörünün tam yolu oluþturuluyor
    string tilesRootPath = Path.Combine(projectRoot, "MapTileDownloader", "MapTileDownloader", "bin", "Debug", "net8.0-windows");

    string tilePath = Path.Combine(tilesRootPath, folderPathClean, z.ToString(), x.ToString(), $"{y}.png");

    Console.WriteLine($"Tile path: {tilePath}");

    if (!File.Exists(tilePath))
        return Results.NotFound("Tile not found");

    return Results.File(tilePath, "image/png");
});



app.MapGet("/api/maps/{id:int}", async (int id) =>
{
    string yol = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..")), "Sqlite", "maptiles.db");

    using var connection = new SqliteConnection($"Data Source={yol}");
    await connection.OpenAsync();


    var command = connection.CreateCommand();
    command.CommandText = @"
        SELECT id, map_name, zoom_min, zoom_max, lat_min, lat_max, lon_min, lon_max, folder_path, created_at 
        FROM DATA WHERE id = $id";
    command.Parameters.AddWithValue("$id", id);

    using var reader = await command.ExecuteReaderAsync();

    if (!await reader.ReadAsync())
        return Results.NotFound();

    var mapData = new MapRecord
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
    };

    return Results.Ok(mapData);
});



app.MapDelete("/maps/{id:int}", (int id) =>
{
    bool deleted = db.DeleteMap(id);
    if (!deleted)
        return Results.NotFound(new { message = "Harita bulunamadý veya zaten silinmiþ." });

    return Results.Ok(new { message = "Harita baþarýyla silindi." });
});

app.MapPut("/maps/{id}/rename", (int id, RenameMapDto dto) =>
{
    bool success = db.RenameMap(id, dto.NewName);
    if (!success)
        return Results.NotFound(new { message = "Harita bulunamadý veya isim deðiþtirilemedi." });

    return Results.Ok(new { message = "Harita ismi baþarýyla deðiþtirildi." });
});


string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToHexString(hash);
}

bool VerifyPassword(string inputPassword, string storedHash)
{
    var inputHash = HashPassword(inputPassword);
    return inputHash == storedHash;
}

app.Run();
