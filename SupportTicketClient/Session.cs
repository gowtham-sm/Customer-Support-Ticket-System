using System.Net.Http;
using System.Net.Http.Headers;

namespace SupportTicketClient;

public static class Session
{
    // Single instance of HttpClient for connection pooling
    public static readonly HttpClient ApiClient = new HttpClient { BaseAddress = new Uri("http://localhost:5208/") };

    public static string? JwtToken { get; private set; }
    public static string? CurrentRole { get; private set; }

    public static void Login(string token, string role)
    {
        JwtToken = token;
        CurrentRole = role;
        // Inject the token into the auth header for all future requests
        ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static void Logout()
    {
        JwtToken = null;
        CurrentRole = null;
        ApiClient.DefaultRequestHeaders.Authorization = null;
    }
}