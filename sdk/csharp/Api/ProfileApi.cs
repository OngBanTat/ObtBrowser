using System.Text.Json.Nodes;

namespace ObtBrowser.Api;

/// <summary>Profile CRUD — create, list, update, delete browser profiles.</summary>
public class ProfileApi : BaseApi
{
    public ProfileApi(HttpClient http) : base(http) { }

    /// <summary>List all profiles.</summary>
    public Task<JsonNode?> ListAsync() =>
        GetAsync("/api/profiles");

    /// <summary>
    /// Create a new browser profile.
    /// fingerprintOptions keys: os, browser, device, locale (all optional).
    /// proxy keys: type, host, port, username, password.
    /// </summary>
    public Task<JsonNode?> CreateAsync(
        string name,
        bool? headless = null,
        object? fingerprintOptions = null,
        object? proxy = null)
    {
        var body = new Dictionary<string, object?> { ["name"] = name };
        if (headless.HasValue)          body["headless"] = headless.Value;
        if (fingerprintOptions is not null) body["fingerprintOptions"] = fingerprintOptions;
        if (proxy is not null)          body["proxy"] = proxy;
        return PostAsync("/api/profiles", body);
    }

    /// <summary>Update profile name and/or headless flag. Omit to leave unchanged.</summary>
    public Task<JsonNode?> UpdateAsync(string profileId, string? name = null, bool? headless = null)
    {
        var body = new Dictionary<string, object?>();
        if (name is not null)   body["name"] = name;
        if (headless.HasValue)  body["headless"] = headless.Value;
        return PutAsync($"/api/profiles/{profileId}", body);
    }

    /// <summary>Permanently delete a profile.</summary>
    public Task DeleteAsync(string profileId) =>
        HttpDeleteAsync($"/api/profiles/{profileId}");
}
