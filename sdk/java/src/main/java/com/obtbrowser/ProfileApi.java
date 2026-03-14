package com.obtbrowser;

import com.fasterxml.jackson.databind.JsonNode;

import java.util.HashMap;
import java.util.Map;

/** Profile CRUD — create, list, update, delete browser profiles. */
public class ProfileApi {

    private final HttpHelper http;

    ProfileApi(HttpHelper http) {
        this.http = http;
    }

    /** List all profiles. */
    public JsonNode list() throws Exception {
        return http.get("/api/profiles");
    }

    /** Create a profile with name only. */
    public JsonNode create(String name) throws Exception {
        return create(name, null, null, null);
    }

    /**
     * Create a new browser profile.
     *
     * @param fingerprintOptions map with keys: os, browser, device, locale (all optional)
     * @param proxy              map with keys: type, host, port, username, password
     */
    public JsonNode create(String name, Boolean headless,
                           Map<String, Object> fingerprintOptions,
                           Map<String, Object> proxy) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("name", name);
        if (headless != null)          body.put("headless", headless);
        if (fingerprintOptions != null) body.put("fingerprintOptions", fingerprintOptions);
        if (proxy != null)             body.put("proxy", proxy);
        return http.post("/api/profiles", body);
    }

    /** Update profile name and/or headless flag. Pass null to leave a field unchanged. */
    public JsonNode update(String profileId, String name, Boolean headless) throws Exception {
        Map<String, Object> body = new HashMap<>();
        if (name != null)    body.put("name", name);
        if (headless != null) body.put("headless", headless);
        return http.put("/api/profiles/" + profileId, body);
    }

    /** Permanently delete a profile. */
    public void delete(String profileId) throws Exception {
        http.delete("/api/profiles/" + profileId);
    }
}
