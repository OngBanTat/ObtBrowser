using System.Text.Json.Nodes;

namespace ObtBrowser.Api;

/// <summary>Browser lifecycle, navigation, page info, screenshot, and generic Playwright execution.</summary>
public class BrowserApi : BaseApi
{
    public BrowserApi(HttpClient http) : base(http) { }

    // ── Lifecycle ──────────────────────────────────────────────────────────────

    /// <summary>Launch browser for a profile (uses profile's default headless setting).</summary>
    public Task<JsonNode?> LaunchAsync(string profileId, bool? headless = null)
    {
        object? body = headless.HasValue ? new { headless = headless.Value } : null;
        return PostAsync($"/api/browsers/{profileId}/launch", body);
    }

    /// <summary>Close the browser for a profile.</summary>
    public Task<JsonNode?> CloseAsync(string profileId) =>
        PostAsync($"/api/browsers/{profileId}/close");

    /// <summary>Returns <c>{{ "running": bool }}</c> for a profile.</summary>
    public Task<JsonNode?> StatusAsync(string profileId) =>
        GetAsync($"/api/browsers/{profileId}/status");

    // ── Navigation ─────────────────────────────────────────────────────────────

    public Task<JsonNode?> NavigateAsync(string profileId, string url, int? timeoutMs = null)
    {
        var body = new Dictionary<string, object> { ["url"] = url };
        if (timeoutMs.HasValue) body["timeout"] = timeoutMs.Value;
        return PostAsync($"/api/browsers/{profileId}/actions/navigate", body);
    }

    public Task<JsonNode?> ReloadAsync(string profileId, int? timeoutMs = null)
    {
        object? body = timeoutMs.HasValue ? new { timeout = timeoutMs.Value } : null;
        return PostAsync($"/api/browsers/{profileId}/actions/reload", body);
    }

    public Task<JsonNode?> GoBackAsync(string profileId) =>
        PostAsync($"/api/browsers/{profileId}/actions/go-back");

    public Task<JsonNode?> GoForwardAsync(string profileId) =>
        PostAsync($"/api/browsers/{profileId}/actions/go-forward");

    // ── Page info ──────────────────────────────────────────────────────────────

    /// <summary>Return current URL and page title.</summary>
    public Task<JsonNode?> PageInfoAsync(string profileId) =>
        GetAsync($"/api/browsers/{profileId}/actions/page-info");

    /// <summary>Return full page HTML.</summary>
    public Task<JsonNode?> ContentAsync(string profileId) =>
        GetAsync($"/api/browsers/{profileId}/actions/content");

    /// <summary>Capture screenshot; response contains base64-encoded PNG.</summary>
    public Task<JsonNode?> ScreenshotAsync(string profileId, bool? fullPage = null)
    {
        object? body = fullPage.HasValue ? new { fullPage = fullPage.Value } : null;
        return PostAsync($"/api/browsers/{profileId}/actions/screenshot", body);
    }

    // ── Generic Playwright execution ──────────────────────────────────────────

    /// <summary>
    /// Execute any Playwright Page method dynamically.
    /// <code>
    /// // Click element by visible text
    /// await client.Browsers.ExecuteAsync(pid, "getByText", new[] { "Submit" },
    ///     new[] { new { method = "click" } });
    ///
    /// // Fill input by label
    /// await client.Browsers.ExecuteAsync(pid, "getByLabel", new[] { "Email" },
    ///     new[] { new { method = "fill", args = new[] { "user@example.com" } } });
    ///
    /// // Count table rows
    /// await client.Browsers.ExecuteAsync(pid, "locator", new[] { "table tbody tr" },
    ///     new[] { new { method = "count" } });
    /// </code>
    /// </summary>
    /// <param name="profileId">Profile ID.</param>
    /// <param name="method">Playwright Page method name (e.g. "getByText", "locator").</param>
    /// <param name="args">Arguments to pass to the method (null for none).</param>
    /// <param name="chain">Method calls to chain on the result (null for none).</param>
    public Task<JsonNode?> ExecuteAsync(string profileId, string method, object[]? args = null, object[]? chain = null)
    {
        var body = new Dictionary<string, object?> { ["method"] = method };
        if (args is not null) body["args"] = args;
        if (chain is not null) body["chain"] = chain;
        return PostAsync($"/api/browsers/{profileId}/execute", body);
    }
}
