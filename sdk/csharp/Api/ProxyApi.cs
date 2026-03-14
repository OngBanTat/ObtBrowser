using System.Text.Json.Nodes;

namespace ObtBrowser.Api;

/// <summary>Proxy pool management — CRUD, assign, and unassign proxies.</summary>
public class ProxyApi : BaseApi
{
    public ProxyApi(HttpClient http) : base(http) { }

    /// <summary>List all proxies.</summary>
    public Task<JsonNode?> ListAsync() =>
        GetAsync("/api/proxies");

    /// <summary>List proxies not assigned to any profile.</summary>
    public Task<JsonNode?> ListUnassignedAsync() =>
        GetAsync("/api/proxies/unassigned");

    /// <summary>Create a proxy. type must be one of: http, https, socks4, socks5.</summary>
    public Task<JsonNode?> CreateAsync(
        string type,
        string host,
        int port,
        string? name = null,
        string? username = null,
        string? password = null)
    {
        var body = new Dictionary<string, object?>
        {
            ["type"] = type,
            ["host"] = host,
            ["port"] = port,
        };
        if (name is not null) body["name"] = name;
        if (username is not null) body["username"] = username;
        if (password is not null) body["password"] = password;
        return PostAsync("/api/proxies", body);
    }

    /// <summary>Update proxy fields. Omit fields to leave unchanged.</summary>
    public Task<JsonNode?> UpdateAsync(
        string proxyId,
        string? name = null,
        string? type = null,
        string? host = null,
        int? port = null,
        string? username = null,
        string? password = null)
    {
        var body = new Dictionary<string, object?>();
        if (name is not null) body["name"] = name;
        if (type is not null) body["type"] = type;
        if (host is not null) body["host"] = host;
        if (port.HasValue) body["port"] = port.Value;
        if (username is not null) body["username"] = username;
        if (password is not null) body["password"] = password;
        return PutAsync($"/api/proxies/{proxyId}", body);
    }

    /// <summary>Delete a proxy.</summary>
    public Task DeleteAsync(string proxyId) =>
        HttpDeleteAsync($"/api/proxies/{proxyId}");

    /// <summary>Assign a proxy to a profile.</summary>
    public Task<JsonNode?> AssignAsync(string proxyId, string profileId) =>
        PostAsync($"/api/proxies/{proxyId}/assign", new { profileId });

    /// <summary>Unassign the proxy currently attached to a profile.</summary>
    public Task<JsonNode?> UnassignAsync(string profileId) =>
        PostAsync("/api/proxies/unassign", new { profileId });

    /// <summary>Clear stored proxy username and password.</summary>
    public Task<JsonNode?> ClearCredentialsAsync(string proxyId) =>
        PutAsync($"/api/proxies/{proxyId}", new Dictionary<string, object?>
        {
            ["username"] = null,
            ["password"] = null,
        });
}