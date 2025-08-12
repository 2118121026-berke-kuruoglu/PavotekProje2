namespace BlazorDashboard.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Username = username, Password = password });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new { Username = username, Password = password });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RenameMapAsync(int id, string newName)
    {
        var dto = new { NewName = newName };
        var response = await _httpClient.PutAsJsonAsync($"/maps/{id}/rename", dto);
        return response.IsSuccessStatusCode;
    }

}
