package com.obtbrowser;

import com.fasterxml.jackson.databind.JsonNode;

import java.util.HashMap;
import java.util.Map;

/** Proxy pool management — CRUD, assign, and unassign proxies. */
public class ProxyApi {

    private final HttpHelper http;

    ProxyApi(HttpHelper http) {
        this.http = http;
    }

    /** List all proxies. */
    public JsonNode list() throws Exception {
        return http.get("/api/proxies");
    }

    /** List proxies that are not assigned to any profile. */
    public JsonNode listUnassigned() throws Exception {
        return http.get("/api/proxies/unassigned");
    }

    /**
     * Create a proxy.
     *
     * @param type one of: http, https, socks4, socks5
     */
    public JsonNode create(String type, String host, int port,
            String name, String username, String password) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("type", type);
        body.put("host", host);
        body.put("port", port);
        if (name != null)
            body.put("name", name);
        if (username != null)
            body.put("username", username);
        if (password != null)
            body.put("password", password);
        return http.post("/api/proxies", body);
    }

    /** Update proxy fields. Pass null for fields you do not want to change. */
    public JsonNode update(String proxyId, String name, String type, String host,
            Integer port, String username, String password) throws Exception {
        Map<String, Object> body = new HashMap<>();
        if (name != null)
            body.put("name", name);
        if (type != null)
            body.put("type", type);
        if (host != null)
            body.put("host", host);
        if (port != null)
            body.put("port", port);
        if (username != null)
            body.put("username", username);
        if (password != null)
            body.put("password", password);
        return http.put("/api/proxies/" + proxyId, body);
    }

    /** Delete a proxy. */
    public void delete(String proxyId) throws Exception {
        http.delete("/api/proxies/" + proxyId);
    }

    /** Assign a proxy to a profile. */
    public JsonNode assign(String proxyId, String profileId) throws Exception {
        return http.post("/api/proxies/" + proxyId + "/assign", Map.of("profileId", profileId));
    }

    /** Unassign any proxy currently attached to a profile. */
    public JsonNode unassign(String profileId) throws Exception {
        return http.post("/api/proxies/unassign", Map.of("profileId", profileId));
    }

    /** Clear stored proxy username and password. */
    public JsonNode clearCredentials(String proxyId) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("username", null);
        body.put("password", null);
        return http.put("/api/proxies/" + proxyId, body);
    }
}