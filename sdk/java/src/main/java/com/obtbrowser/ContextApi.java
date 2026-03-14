package com.obtbrowser;

import com.fasterxml.jackson.databind.JsonNode;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Browser context operations — multi-tab management, geolocation, permissions, storage.
 *
 * <pre>{@code
 * client.context.newPage(profileId);
 * client.context.setGeolocation(profileId, 10.823, 106.629, null);
 * client.context.grantPermissions(profileId, List.of("geolocation"), null);
 * JsonNode state = client.context.storageState(profileId);
 * }</pre>
 */
public class ContextApi {

    private final HttpHelper http;

    ContextApi(HttpHelper http) {
        this.http = http;
    }

    private String base(String profileId) {
        return "/api/browsers/" + profileId + "/context";
    }

    /** Export cookies + localStorage for session reuse. */
    public JsonNode storageState(String profileId) throws Exception {
        return http.get(base(profileId) + "/storage-state");
    }

    /** Open a new tab in the browser context. */
    public JsonNode newPage(String profileId) throws Exception {
        return http.post(base(profileId) + "/new-page", null);
    }

    /** Return {@code {"count": int, "urls": [...]}} listing all open tabs. */
    public JsonNode pages(String profileId) throws Exception {
        return http.get(base(profileId) + "/pages");
    }

    /** Set HTTP headers for all pages in this context. */
    public JsonNode setExtraHttpHeaders(String profileId, Map<String, String> headers) throws Exception {
        return http.post(base(profileId) + "/extra-http-headers", Map.of("headers", headers));
    }

    /**
     * Grant browser permissions.
     *
     * @param permissions "geolocation"|"camera"|"microphone"|"notifications"|"clipboard-read"|"clipboard-write"
     * @param origin      optional origin to scope permission (e.g. "https://example.com")
     */
    public JsonNode grantPermissions(String profileId, List<String> permissions, String origin) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("permissions", permissions);
        if (origin != null) body.put("origin", origin);
        return http.post(base(profileId) + "/grant-permissions", body);
    }

    /** Revoke all granted permissions. */
    public JsonNode clearPermissions(String profileId) throws Exception {
        return http.post(base(profileId) + "/clear-permissions", null);
    }

    /**
     * Spoof GPS location.
     *
     * @param accuracy optional accuracy in meters
     */
    public JsonNode setGeolocation(String profileId, double latitude, double longitude, Double accuracy) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("latitude", latitude);
        body.put("longitude", longitude);
        if (accuracy != null) body.put("accuracy", accuracy);
        return http.post(base(profileId) + "/geolocation", body);
    }
}
