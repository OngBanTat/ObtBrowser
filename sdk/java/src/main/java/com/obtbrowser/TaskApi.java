package com.obtbrowser;

import com.fasterxml.jackson.databind.JsonNode;

import java.util.HashMap;
import java.util.Map;

/** Task management — create, run, cancel, and delete automation tasks. */
public class TaskApi {

    private final HttpHelper http;

    TaskApi(HttpHelper http) {
        this.http = http;
    }

    /** List all tasks. */
    public JsonNode list() throws Exception {
        return list(null);
    }

    /** List tasks filtered by profile ID. */
    public JsonNode list(String profileId) throws Exception {
        Map<String, String> params = profileId != null ? Map.of("profileId", profileId) : null;
        return http.get("/api/tasks", params);
    }

    /**
     * Create an automation task.
     *
     * @param scriptType    "inline" (script string) or "file" (path to script file)
     * @param scriptContent inline script or file path
     */
    public JsonNode create(String profileId, String name,
                           String scriptType, String scriptContent) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("profileId", profileId);
        body.put("name", name);
        body.put("scriptType", scriptType);
        body.put("scriptContent", scriptContent);
        return http.post("/api/tasks", body);
    }

    /** Enqueue a task for execution. */
    public JsonNode run(String taskId) throws Exception {
        return http.post("/api/tasks/" + taskId + "/run", null);
    }

    /** Cancel a queued or running task. */
    public JsonNode cancel(String taskId) throws Exception {
        return http.post("/api/tasks/" + taskId + "/cancel", null);
    }

    /** Delete a task record. */
    public void delete(String taskId) throws Exception {
        http.delete("/api/tasks/" + taskId);
    }
}
