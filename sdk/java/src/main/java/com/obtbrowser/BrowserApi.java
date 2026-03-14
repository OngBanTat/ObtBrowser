package com.obtbrowser;

import com.fasterxml.jackson.databind.JsonNode;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/** Browser lifecycle, navigation, page info, screenshot, and generic Playwright execution. */
public class BrowserApi {

    private final HttpHelper http;

    BrowserApi(HttpHelper http) {
        this.http = http;
    }

    // ── Lifecycle ──────────────────────────────────────────────────────────────

    /** Launch browser for a profile (uses profile's default headless setting). */
    public JsonNode launch(String profileId) throws Exception {
        return launch(profileId, null);
    }

    /** Launch browser, overriding the headless setting if provided. */
    public JsonNode launch(String profileId, Boolean headless) throws Exception {
        Map<String, Object> body = new HashMap<>();
        if (headless != null) body.put("headless", headless);
        return http.post("/api/browsers/" + profileId + "/launch", body.isEmpty() ? null : body);
    }

    /** Close the browser for a profile. */
    public JsonNode close(String profileId) throws Exception {
        return http.post("/api/browsers/" + profileId + "/close", null);
    }

    /** Return {@code {"running": true/false}} for a profile. */
    public JsonNode status(String profileId) throws Exception {
        return http.get("/api/browsers/" + profileId + "/status");
    }

    // ── Navigation ─────────────────────────────────────────────────────────────

    public JsonNode navigate(String profileId, String url) throws Exception {
        return navigate(profileId, url, null);
    }

    public JsonNode navigate(String profileId, String url, Integer timeoutMs) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("url", url);
        if (timeoutMs != null) body.put("timeout", timeoutMs);
        return http.post("/api/browsers/" + profileId + "/actions/navigate", body);
    }

    public JsonNode reload(String profileId) throws Exception {
        return http.post("/api/browsers/" + profileId + "/actions/reload", null);
    }

    public JsonNode goBack(String profileId) throws Exception {
        return http.post("/api/browsers/" + profileId + "/actions/go-back", null);
    }

    public JsonNode goForward(String profileId) throws Exception {
        return http.post("/api/browsers/" + profileId + "/actions/go-forward", null);
    }

    // ── Page info ──────────────────────────────────────────────────────────────

    /** Return current URL and page title. */
    public JsonNode pageInfo(String profileId) throws Exception {
        return http.get("/api/browsers/" + profileId + "/actions/page-info");
    }

    /** Return full page HTML. */
    public JsonNode content(String profileId) throws Exception {
        return http.get("/api/browsers/" + profileId + "/actions/content");
    }

    /** Capture screenshot; response contains base64-encoded PNG. */
    public JsonNode screenshot(String profileId, Boolean fullPage) throws Exception {
        Map<String, Object> body = new HashMap<>();
        if (fullPage != null) body.put("fullPage", fullPage);
        return http.post("/api/browsers/" + profileId + "/actions/screenshot",
                body.isEmpty() ? null : body);
    }

    // ── Generic Playwright execution ──────────────────────────────────────────

    /**
     * Execute any Playwright Page method dynamically.
     *
     * <pre>{@code
     * // Click element by visible text
     * client.browsers.execute(profileId, "getByText", List.of("Submit"),
     *     List.of(Map.of("method", "click")));
     *
     * // Fill input by label
     * client.browsers.execute(profileId, "getByLabel", List.of("Email"),
     *     List.of(Map.of("method", "fill", "args", List.of("user@example.com"))));
     *
     * // Count table rows
     * client.browsers.execute(profileId, "locator", List.of("table tbody tr"),
     *     List.of(Map.of("method", "count")));
     * }</pre>
     *
     * @param profileId Profile ID
     * @param method    Playwright Page method name (e.g. "getByText", "locator")
     * @param args      Arguments to pass to the method (null for none)
     * @param chain     List of method calls to chain on the result (null for none)
     */
    public JsonNode execute(String profileId, String method, List<?> args, List<Map<String, Object>> chain) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("method", method);
        if (args != null) body.put("args", args);
        if (chain != null) body.put("chain", chain);
        return http.post("/api/browsers/" + profileId + "/execute", body);
    }

    /** Execute a Playwright Page method with no arguments. */
    public JsonNode execute(String profileId, String method) throws Exception {
        return execute(profileId, method, null, null);
    }
}
