using System.Text.Json.Nodes;

namespace ObtBrowser.Api;

/// <summary>
/// Browser context operations — multi-tab management, geolocation, permissions, storage.
/// <code>
/// await client.Context.NewPageAsync(profileId);
/// await client.Context.SetGeolocationAsync(profileId, 10.823, 106.629);
/// await client.Context.GrantPermissionsAsync(profileId, new[] { "geolocation" });
/// var state = await client.Context.StorageStateAsync(profileId);
/// </code>
/// </summary>
public class ContextApi : BaseApi
{
    public ContextApi(HttpClient http) : base(http) { }

    private string Base(string profileId) => $"/api/browsers/{profileId}/context";

    /// <summary>Export cookies + localStorage for session reuse.</summary>
    public Task<JsonNode?> StorageStateAsync(string profileId) =>
        GetAsync($"{Base(profileId)}/storage-state");

    /// <summary>Open a new tab in the browser context.</summary>
    public Task<JsonNode?> NewPageAsync(string profileId) =>
        PostAsync($"{Base(profileId)}/new-page");

    /// <summary>Returns <c>{{ "count": int, "urls": [...] }}</c> listing all open tabs.</summary>
    public Task<JsonNode?> PagesAsync(string profileId) =>
        GetAsync($"{Base(profileId)}/pages");

    /// <summary>Set HTTP headers for all pages in this context.</summary>
    public Task<JsonNode?> SetExtraHttpHeadersAsync(string profileId, Dictionary<string, string> headers) =>
        PostAsync($"{Base(profileId)}/extra-http-headers", new { headers });

    /// <summary>
    /// Grant browser permissions.
    /// permissions: "geolocation"|"camera"|"microphone"|"notifications"|"clipboard-read"|"clipboard-write"
    /// </summary>
    public Task<JsonNode?> GrantPermissionsAsync(string profileId, IEnumerable<string> permissions, string? origin = null)
    {
        var body = new Dictionary<string, object> { ["permissions"] = permissions };
        if (origin is not null) body["origin"] = origin;
        return PostAsync($"{Base(profileId)}/grant-permissions", body);
    }

    /// <summary>Revoke all granted permissions.</summary>
    public Task<JsonNode?> ClearPermissionsAsync(string profileId) =>
        PostAsync($"{Base(profileId)}/clear-permissions");

    /// <summary>Spoof GPS location.</summary>
    public Task<JsonNode?> SetGeolocationAsync(string profileId, double latitude, double longitude, double? accuracy = null)
    {
        var body = new Dictionary<string, object> { ["latitude"] = latitude, ["longitude"] = longitude };
        if (accuracy.HasValue) body["accuracy"] = accuracy.Value;
        return PostAsync($"{Base(profileId)}/geolocation", body);
    }
}
