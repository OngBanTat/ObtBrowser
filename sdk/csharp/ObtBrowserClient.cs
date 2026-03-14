using ObtBrowser.Api;
using System.Text.Json.Nodes;

namespace ObtBrowser;

/// <summary>
/// Main entry point for the ObtAntiDetect Browser REST API client.
///
/// <code>
/// var client = new ObtBrowserClient("http://localhost:3000");
///
/// var profile = await client.Profiles.CreateAsync("My Profile");
/// var pid = profile!["id"]!.GetValue&lt;string&gt;();
///
/// await client.Browsers.LaunchAsync(pid);
/// await client.Browsers.NavigateAsync(pid, "https://example.com");
///
/// // Fill form using Playwright locators
/// await client.Browsers.ExecuteAsync(pid, "getByLabel", new[] { "Email" },
///     new[] { new { method = "fill", args = new[] { "user@example.com" } } });
/// await client.Browsers.ExecuteAsync(pid, "getByRole", new object[] { "button", new { name = "Login" } },
///     new[] { new { method = "click" } });
///
/// // Wait and check state
/// await client.Pages.WaitForLoadStateAsync(pid, "networkidle");
/// var loggedIn = (await client.Pages.IsVisibleAsync(pid, ".dashboard"))!["visible"]!.GetValue&lt;bool&gt;();
///
/// // Browser context operations
/// await client.Context.NewPageAsync(pid);
/// await client.Context.SetGeolocationAsync(pid, 10.823, 106.629);
///
/// await client.Browsers.CloseAsync(pid);
/// </code>
/// </summary>
public class ObtBrowserClient : IDisposable
{
    private readonly HttpClient _http;

    public ProfileApi Profiles { get; }
    public BrowserApi Browsers { get; }
    public PageActionApi Pages { get; }
    public ContextApi Context { get; }
    public TaskApi Tasks { get; }
    public ProxyApi Proxies { get; }

    /// <summary>Connect to the default local server on port 3000.</summary>
    public ObtBrowserClient() : this("http://localhost:3000") { }

    public ObtBrowserClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/") };
        Profiles = new ProfileApi(_http);
        Browsers = new BrowserApi(_http);
        Pages    = new PageActionApi(_http);
        Context  = new ContextApi(_http);
        Tasks    = new TaskApi(_http);
        Proxies  = new ProxyApi(_http);
    }

    /// <summary>Check API server health. Returns <c>{"status":"ok"}</c> when running.</summary>
    public async Task<JsonNode?> HealthAsync()
    {
        var resp = await _http.GetAsync("/api/health");
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(body) ? null : JsonNode.Parse(body);
    }

    public void Dispose() => _http.Dispose();
}
