using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ObtBrowser.Api;

/// <summary>Thrown when the API returns a 4xx or 5xx response.</summary>
public class ApiException : Exception
{
    public int StatusCode { get; }
    public ApiException(int statusCode, string message)
        : base($"HTTP {statusCode}: {message}") => StatusCode = statusCode;
}

/// <summary>Shared HTTP helpers for all API sub-clients.</summary>
public abstract class BaseApi
{
    protected readonly HttpClient Http;

    private static readonly JsonSerializerOptions CamelCase =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected BaseApi(HttpClient http) => Http = http;

    protected async Task<JsonNode?> GetAsync(string path, Dictionary<string, string>? query = null)
    {
        var url = BuildUrl(path, query);
        var resp = await Http.GetAsync(url);
        return await ParseResponseAsync(resp);
    }

    protected async Task<JsonNode?> PostAsync(string path, object? body = null)
    {
        var json = body is not null ? JsonSerializer.Serialize(body, CamelCase) : "{}";
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await Http.PostAsync(path, content);
        return await ParseResponseAsync(resp);
    }

    protected async Task<JsonNode?> PutAsync(string path, object body)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(body, CamelCase), Encoding.UTF8, "application/json");
        var resp = await Http.PutAsync(path, content);
        return await ParseResponseAsync(resp);
    }

    /// <summary>Named to avoid conflict with public DeleteAsync in derived classes.</summary>
    protected async Task HttpDeleteAsync(string path)
    {
        var resp = await Http.DeleteAsync(path);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new ApiException((int)resp.StatusCode, ExtractError(body));
        }
    }

    private static string BuildUrl(string path, Dictionary<string, string>? query)
    {
        if (query is null || query.Count == 0) return path;
        var qs = string.Join("&", query
            .Where(p => p.Value is not null)
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
        return string.IsNullOrEmpty(qs) ? path : $"{path}?{qs}";
    }

    private static async Task<JsonNode?> ParseResponseAsync(HttpResponseMessage resp)
    {
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new ApiException((int)resp.StatusCode, ExtractError(body));
        return string.IsNullOrWhiteSpace(body) ? null : JsonNode.Parse(body);
    }

    private static string ExtractError(string body)
    {
        try
        {
            var node = JsonNode.Parse(body);
            return node?["error"]?.GetValue<string>() ?? body;
        }
        catch { return body; }
    }
}
