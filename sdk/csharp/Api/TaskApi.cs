using System.Text.Json.Nodes;

namespace ObtBrowser.Api;

/// <summary>Task management — create, run, cancel, and delete automation tasks.</summary>
public class TaskApi : BaseApi
{
    public TaskApi(HttpClient http) : base(http) { }

    /// <summary>List all tasks, optionally filtered by profile ID.</summary>
    public Task<JsonNode?> ListAsync(string? profileId = null)
    {
        var query = profileId is not null
            ? new Dictionary<string, string> { ["profileId"] = profileId }
            : null;
        return GetAsync("/api/tasks", query);
    }

    /// <summary>
    /// Create an automation task.
    /// scriptType: "inline" (script string) or "file" (path to script file).
    /// </summary>
    public Task<JsonNode?> CreateAsync(
        string profileId, string name, string scriptType, string scriptContent) =>
        PostAsync("/api/tasks", new { profileId, name, scriptType, scriptContent });

    /// <summary>Enqueue a task for execution.</summary>
    public Task<JsonNode?> RunAsync(string taskId) =>
        PostAsync($"/api/tasks/{taskId}/run");

    /// <summary>Cancel a queued or running task.</summary>
    public Task<JsonNode?> CancelAsync(string taskId) =>
        PostAsync($"/api/tasks/{taskId}/cancel");

    /// <summary>Delete a task record.</summary>
    public Task DeleteAsync(string taskId) =>
        HttpDeleteAsync($"/api/tasks/{taskId}");
}
