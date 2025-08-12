using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorDashboard.Shared
{
    public class UserSession
    {
        private readonly IJSRuntime _jsRuntime;

        public bool IsLoggedIn { get; private set; }
        public string? Username { get; private set; }

        public event Action OnChange;

        public UserSession(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            Username = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "username");
            IsLoggedIn = !string.IsNullOrEmpty(Username);
            NotifyStateChanged();
        }

        public async Task SetUserAsync(string username)
        {
            Username = username;
            IsLoggedIn = true;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "username", username);
            NotifyStateChanged();
        }

        public async Task LogoutAsync()
        {
            Username = null;
            IsLoggedIn = false;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "username");
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
