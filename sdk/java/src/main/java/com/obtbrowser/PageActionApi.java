package com.obtbrowser;

import com.fasterxml.jackson.databind.JsonNode;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/** Element interactions, state checks, waits, data extraction, keyboard, mouse, scripts, and cookies. */
public class PageActionApi {

    private final HttpHelper http;

    PageActionApi(HttpHelper http) {
        this.http = http;
    }

    private String base(String profileId) {
        return "/api/browsers/" + profileId + "/actions";
    }

    // ── Element interactions ───────────────────────────────────────────────────

    public JsonNode click(String profileId, String selector) throws Exception {
        return click(profileId, selector, null);
    }

    /** button: "left" | "right" | "middle" */
    public JsonNode click(String profileId, String selector, String button) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("selector", selector);
        if (button != null) body.put("button", button);
        return http.post(base(profileId) + "/click", body);
    }

    public JsonNode doubleClick(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/double-click", Map.of("selector", selector));
    }

    public JsonNode hover(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/hover", Map.of("selector", selector));
    }

    public JsonNode focus(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/focus", Map.of("selector", selector));
    }

    /** Clear and fill a form field. */
    public JsonNode fill(String profileId, String selector, String value) throws Exception {
        return http.post(base(profileId) + "/fill", Map.of("selector", selector, "value", value));
    }

    public JsonNode type(String profileId, String selector, String text) throws Exception {
        return type(profileId, selector, text, null);
    }

    /** Type text char-by-char with optional keystroke delay in ms. */
    public JsonNode type(String profileId, String selector, String text, Integer delayMs) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("selector", selector);
        body.put("text", text);
        if (delayMs != null) body.put("delay", delayMs);
        return http.post(base(profileId) + "/type", body);
    }

    public JsonNode pressKey(String profileId, String key) throws Exception {
        return pressKey(profileId, key, null);
    }

    /** Press a key (e.g. "Enter", "Tab"). Optionally focus selector first. */
    public JsonNode pressKey(String profileId, String key, String selector) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("key", key);
        if (selector != null) body.put("selector", selector);
        return http.post(base(profileId) + "/press-key", body);
    }

    /** value can be String or List<String> for multi-select. */
    public JsonNode selectOption(String profileId, String selector, Object value) throws Exception {
        return http.post(base(profileId) + "/select-option",
                Map.of("selector", selector, "value", value));
    }

    public JsonNode check(String profileId, String selector, boolean checked) throws Exception {
        return http.post(base(profileId) + "/check",
                Map.of("selector", selector, "checked", checked));
    }

    public JsonNode scroll(String profileId, int x, int y) throws Exception {
        return scroll(profileId, x, y, null);
    }

    /** Scroll by (x, y), or scroll element into view if selector is provided. */
    public JsonNode scroll(String profileId, int x, int y, String selector) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("x", x);
        body.put("y", y);
        if (selector != null) body.put("selector", selector);
        return http.post(base(profileId) + "/scroll", body);
    }

    /** Mobile touch tap on element. */
    public JsonNode tap(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/tap", Map.of("selector", selector));
    }

    /** Drag element from source selector to target selector. */
    public JsonNode dragAndDrop(String profileId, String source, String target) throws Exception {
        return http.post(base(profileId) + "/drag-and-drop",
                Map.of("source", source, "target", target));
    }

    /** Fire a DOM event (e.g. "click", "input") on element. */
    public JsonNode dispatchEvent(String profileId, String selector, String eventType) throws Exception {
        return http.post(base(profileId) + "/dispatch-event",
                Map.of("selector", selector, "type", eventType));
    }

    /** Resize browser viewport. */
    public JsonNode setViewportSize(String profileId, int width, int height) throws Exception {
        return http.post(base(profileId) + "/set-viewport-size",
                Map.of("width", width, "height", height));
    }

    /** Replace page content with an HTML string. */
    public JsonNode setContent(String profileId, String html) throws Exception {
        return http.post(base(profileId) + "/set-content", Map.of("html", html));
    }

    /** Wait for next page navigation to complete. */
    public JsonNode waitForNavigation(String profileId, Integer timeoutMs) throws Exception {
        Map<String, Object> body = new HashMap<>();
        if (timeoutMs != null) body.put("timeout", timeoutMs);
        return http.post(base(profileId) + "/wait-for-navigation", body.isEmpty() ? null : body);
    }

    // ── Waits ──────────────────────────────────────────────────────────────────

    public JsonNode waitForSelector(String profileId, String selector) throws Exception {
        return waitForSelector(profileId, selector, null);
    }

    public JsonNode waitForSelector(String profileId, String selector, Integer timeoutMs) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("selector", selector);
        if (timeoutMs != null) body.put("timeout", timeoutMs);
        return http.post(base(profileId) + "/wait-for-selector", body);
    }

    public JsonNode waitForUrl(String profileId, String pattern) throws Exception {
        return waitForUrl(profileId, pattern, null);
    }

    public JsonNode waitForUrl(String profileId, String pattern, Integer timeoutMs) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("pattern", pattern);
        if (timeoutMs != null) body.put("timeout", timeoutMs);
        return http.post(base(profileId) + "/wait-for-url", body);
    }

    /** Pause execution for a fixed number of milliseconds. */
    public JsonNode waitForTimeout(String profileId, int ms) throws Exception {
        return http.post(base(profileId) + "/wait-for-timeout", Map.of("ms", ms));
    }

    /** Wait for page load state: "load" | "domcontentloaded" | "networkidle". */
    public JsonNode waitForLoadState(String profileId, String state, Integer timeoutMs) throws Exception {
        Map<String, Object> body = new HashMap<>();
        if (state != null) body.put("state", state);
        if (timeoutMs != null) body.put("timeout", timeoutMs);
        return http.post(base(profileId) + "/wait-for-load-state", body.isEmpty() ? null : body);
    }

    // ── State checks ───────────────────────────────────────────────────────────

    /** Return {@code {"visible": bool}} — no waiting. */
    public JsonNode isVisible(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/is-visible", Map.of("selector", selector));
    }

    /** Return {@code {"hidden": bool}} — hidden or absent from DOM. */
    public JsonNode isHidden(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/is-hidden", Map.of("selector", selector));
    }

    /** Return {@code {"checked": bool}} — checkbox/radio state. */
    public JsonNode isChecked(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/is-checked", Map.of("selector", selector));
    }

    /** Return {@code {"enabled": bool}}. */
    public JsonNode isEnabled(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/is-enabled", Map.of("selector", selector));
    }

    /** Return {@code {"disabled": bool}}. */
    public JsonNode isDisabled(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/is-disabled", Map.of("selector", selector));
    }

    /** Return {@code {"editable": bool}} — not readonly/disabled. */
    public JsonNode isEditable(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/is-editable", Map.of("selector", selector));
    }

    /** Return {@code {"text": str}} — raw textContent including hidden text. */
    public JsonNode textContent(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/text-content", Map.of("selector", selector));
    }

    // ── Data extraction ────────────────────────────────────────────────────────

    public JsonNode getText(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/get-text", Map.of("selector", selector));
    }

    public JsonNode getAttribute(String profileId, String selector, String attribute) throws Exception {
        return http.post(base(profileId) + "/get-attribute",
                Map.of("selector", selector, "attribute", attribute));
    }

    public JsonNode getValue(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/get-value", Map.of("selector", selector));
    }

    public JsonNode getInnerHtml(String profileId, String selector) throws Exception {
        return http.post(base(profileId) + "/get-inner-html", Map.of("selector", selector));
    }

    // ── Keyboard (low-level) ──────────────────────────────────────────────────

    /** Hold a key down (for modifier combos like Shift+Click). */
    public JsonNode keyboardDown(String profileId, String key) throws Exception {
        return http.post(base(profileId) + "/keyboard/down", Map.of("key", key));
    }

    /** Release a held key. */
    public JsonNode keyboardUp(String profileId, String key) throws Exception {
        return http.post(base(profileId) + "/keyboard/up", Map.of("key", key));
    }

    /** Type text char-by-char (page-wide, no selector). */
    public JsonNode keyboardType(String profileId, String text, Integer delayMs) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("text", text);
        if (delayMs != null) body.put("delay", delayMs);
        return http.post(base(profileId) + "/keyboard/type", body);
    }

    /** Insert text instantly (supports unicode, no keystroke events). */
    public JsonNode keyboardInsertText(String profileId, String text) throws Exception {
        return http.post(base(profileId) + "/keyboard/insert-text", Map.of("text", text));
    }

    // ── Mouse (low-level) ─────────────────────────────────────────────────────

    /** Click at absolute coordinates. button: "left"|"right"|"middle". */
    public JsonNode mouseClick(String profileId, int x, int y, String button, Integer clickCount) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("x", x);
        body.put("y", y);
        if (button != null) body.put("button", button);
        if (clickCount != null) body.put("clickCount", clickCount);
        return http.post(base(profileId) + "/mouse/click", body);
    }

    /** Move mouse to absolute coordinates. */
    public JsonNode mouseMove(String profileId, int x, int y, Integer steps) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("x", x);
        body.put("y", y);
        if (steps != null) body.put("steps", steps);
        return http.post(base(profileId) + "/mouse/move", body);
    }

    /** Double-click at coordinates. */
    public JsonNode mouseDblClick(String profileId, int x, int y, String button) throws Exception {
        Map<String, Object> body = new HashMap<>();
        body.put("x", x);
        body.put("y", y);
        if (button != null) body.put("button", button);
        return http.post(base(profileId) + "/mouse/dblclick", body);
    }

    /** Press and hold mouse button. */
    public JsonNode mouseDown(String profileId, String button) throws Exception {
        Map<String, Object> body = new HashMap<>();
        if (button != null) body.put("button", button);
        return http.post(base(profileId) + "/mouse/down", body.isEmpty() ? null : body);
    }

    /** Release held mouse button. */
    public JsonNode mouseUp(String profileId, String button) throws Exception {
        Map<String, Object> body = new HashMap<>();
        if (button != null) body.put("button", button);
        return http.post(base(profileId) + "/mouse/up", body.isEmpty() ? null : body);
    }

    /** Scroll via mouse wheel delta. */
    public JsonNode mouseWheel(String profileId, int deltaX, int deltaY) throws Exception {
        return http.post(base(profileId) + "/mouse/wheel",
                Map.of("deltaX", deltaX, "deltaY", deltaY));
    }

    // ── Page configuration ────────────────────────────────────────────────────

    /** Set custom HTTP headers for all subsequent requests on this page. */
    public JsonNode setExtraHttpHeaders(String profileId, Map<String, String> headers) throws Exception {
        return http.post(base(profileId) + "/set-extra-http-headers", Map.of("headers", headers));
    }

    /** Inject JS to run before every page load on this context. */
    public JsonNode addInitScript(String profileId, String script) throws Exception {
        return http.post(base(profileId) + "/add-init-script", Map.of("script", script));
    }

    // ── Scripts ────────────────────────────────────────────────────────────────

    /** Evaluate a JS expression and return its result. */
    public JsonNode evaluate(String profileId, String expression) throws Exception {
        return http.post(base(profileId) + "/evaluate", Map.of("expression", expression));
    }

    /** Run a multi-line async script. Returns {"output": "..."} from log() calls. */
    public JsonNode runScript(String profileId, String script) throws Exception {
        return http.post(base(profileId) + "/run-script", Map.of("script", script));
    }

    // ── Cookies ────────────────────────────────────────────────────────────────

    public JsonNode getCookies(String profileId) throws Exception {
        return getCookies(profileId, null);
    }

    public JsonNode getCookies(String profileId, List<String> urls) throws Exception {
        Map<String, String> params = null;
        if (urls != null && !urls.isEmpty()) {
            params = Map.of("urls", String.join(",", urls));
        }
        return http.get(base(profileId) + "/cookies", params);
    }

    public JsonNode setCookies(String profileId, List<Map<String, Object>> cookies) throws Exception {
        return http.post(base(profileId) + "/cookies", Map.of("cookies", cookies));
    }

    public void clearCookies(String profileId) throws Exception {
        http.delete(base(profileId) + "/cookies");
    }
}
