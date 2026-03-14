package com.obtbrowser;

import com.fasterxml.jackson.databind.JsonNode;

/**
 * Main entry point for the ObtAntiDetect Browser REST API client.
 *
 * <pre>{@code
 * ObtBrowserClient client = new ObtBrowserClient("http://localhost:3000");
 *
 * JsonNode profile = client.profiles.create("My Profile");
 * String profileId = profile.get("id").asText();
 *
 * client.browsers.launch(profileId);
 * client.browsers.navigate(profileId, "https://example.com");
 *
 * // Fill form using Playwright locators
 * client.browsers.execute(profileId, "getByLabel", List.of("Email"),
 *     List.of(Map.of("method", "fill", "args", List.of("user@example.com"))));
 * client.browsers.execute(profileId, "getByRole", List.of("button", Map.of("name", "Login")),
 *     List.of(Map.of("method", "click")));
 *
 * // Wait and check state
 * client.pages.waitForLoadState(profileId, "networkidle", null);
 * boolean loggedIn = client.pages.isVisible(profileId, ".dashboard").get("visible").asBoolean();
 *
 * // Browser context operations
 * client.context.newPage(profileId);
 * client.context.setGeolocation(profileId, 10.823, 106.629, null);
 *
 * client.browsers.close(profileId);
 * }</pre>
 */
public class ObtBrowserClient {

    public final ProfileApi profiles;
    public final BrowserApi browsers;
    public final PageActionApi pages;
    public final ContextApi context;
    public final TaskApi tasks;
    public final ProxyApi proxies;

    private final HttpHelper http;

    /** Connect to the default local server on port 3000. */
    public ObtBrowserClient() {
        this("http://localhost:3000");
    }

    public ObtBrowserClient(String baseUrl) {
        this.http = new HttpHelper(baseUrl);
        this.profiles = new ProfileApi(http);
        this.browsers = new BrowserApi(http);
        this.pages = new PageActionApi(http);
        this.context = new ContextApi(http);
        this.tasks = new TaskApi(http);
        this.proxies = new ProxyApi(http);
    }

    /** Check API server health. Returns {@code {"status": "ok"}} when running. */
    public JsonNode health() throws Exception {
        return http.get("/api/health");
    }
}
