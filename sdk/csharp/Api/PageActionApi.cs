using System.Text.Json.Nodes;

namespace ObtBrowser.Api;

/// <summary>Element interactions, state checks, waits, data extraction, keyboard, mouse, scripts, and cookies.</summary>
public class PageActionApi : BaseApi
{
    public PageActionApi(HttpClient http) : base(http) { }

    private string Base(string profileId) => $"/api/browsers/{profileId}/actions";

    // ── Element interactions ───────────────────────────────────────────────────

    /// <summary>Click an element. button: "left" | "right" | "middle".</summary>
    public Task<JsonNode?> ClickAsync(string profileId, string selector, string? button = null)
    {
        var body = new Dictionary<string, object> { ["selector"] = selector };
        if (button is not null) body["button"] = button;
        return PostAsync($"{Base(profileId)}/click", body);
    }

    public Task<JsonNode?> DoubleClickAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/double-click", new { selector });

    public Task<JsonNode?> HoverAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/hover", new { selector });

    public Task<JsonNode?> FocusAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/focus", new { selector });

    /// <summary>Clear and fill a form field.</summary>
    public Task<JsonNode?> FillAsync(string profileId, string selector, string value) =>
        PostAsync($"{Base(profileId)}/fill", new { selector, value });

    /// <summary>Type text char-by-char with optional keystroke delay in ms.</summary>
    public Task<JsonNode?> TypeAsync(string profileId, string selector, string text, int? delayMs = null)
    {
        var body = new Dictionary<string, object> { ["selector"] = selector, ["text"] = text };
        if (delayMs.HasValue) body["delay"] = delayMs.Value;
        return PostAsync($"{Base(profileId)}/type", body);
    }

    /// <summary>Press a key (e.g. "Enter", "Tab"). Optionally focus selector first.</summary>
    public Task<JsonNode?> PressKeyAsync(string profileId, string key, string? selector = null)
    {
        var body = new Dictionary<string, object> { ["key"] = key };
        if (selector is not null) body["selector"] = selector;
        return PostAsync($"{Base(profileId)}/press-key", body);
    }

    /// <summary>Select option(s) in a &lt;select&gt; element. value can be string or string[].</summary>
    public Task<JsonNode?> SelectOptionAsync(string profileId, string selector, object value) =>
        PostAsync($"{Base(profileId)}/select-option", new { selector, value });

    public Task<JsonNode?> CheckAsync(string profileId, string selector, bool checked_ = true) =>
        PostAsync($"{Base(profileId)}/check", new { selector, @checked = checked_ });

    /// <summary>Scroll by (x, y) delta, or scroll element into view if selector given.</summary>
    public Task<JsonNode?> ScrollAsync(string profileId, int x = 0, int y = 0, string? selector = null)
    {
        var body = new Dictionary<string, object> { ["x"] = x, ["y"] = y };
        if (selector is not null) body["selector"] = selector;
        return PostAsync($"{Base(profileId)}/scroll", body);
    }

    /// <summary>Mobile touch tap on element.</summary>
    public Task<JsonNode?> TapAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/tap", new { selector });

    /// <summary>Drag element from source selector to target selector.</summary>
    public Task<JsonNode?> DragAndDropAsync(string profileId, string source, string target) =>
        PostAsync($"{Base(profileId)}/drag-and-drop", new { source, target });

    /// <summary>Fire a DOM event (e.g. "click", "input") on element.</summary>
    public Task<JsonNode?> DispatchEventAsync(string profileId, string selector, string eventType) =>
        PostAsync($"{Base(profileId)}/dispatch-event", new { selector, type = eventType });

    /// <summary>Resize browser viewport.</summary>
    public Task<JsonNode?> SetViewportSizeAsync(string profileId, int width, int height) =>
        PostAsync($"{Base(profileId)}/set-viewport-size", new { width, height });

    /// <summary>Replace page content with an HTML string.</summary>
    public Task<JsonNode?> SetContentAsync(string profileId, string html) =>
        PostAsync($"{Base(profileId)}/set-content", new { html });

    /// <summary>Wait for next page navigation to complete.</summary>
    public Task<JsonNode?> WaitForNavigationAsync(string profileId, int? timeoutMs = null)
    {
        object? body = timeoutMs.HasValue ? new { timeout = timeoutMs.Value } : null;
        return PostAsync($"{Base(profileId)}/wait-for-navigation", body);
    }

    // ── Waits ──────────────────────────────────────────────────────────────────

    public Task<JsonNode?> WaitForSelectorAsync(string profileId, string selector, int? timeoutMs = null)
    {
        var body = new Dictionary<string, object> { ["selector"] = selector };
        if (timeoutMs.HasValue) body["timeout"] = timeoutMs.Value;
        return PostAsync($"{Base(profileId)}/wait-for-selector", body);
    }

    public Task<JsonNode?> WaitForUrlAsync(string profileId, string pattern, int? timeoutMs = null)
    {
        var body = new Dictionary<string, object> { ["pattern"] = pattern };
        if (timeoutMs.HasValue) body["timeout"] = timeoutMs.Value;
        return PostAsync($"{Base(profileId)}/wait-for-url", body);
    }

    /// <summary>Pause execution for a fixed number of milliseconds.</summary>
    public Task<JsonNode?> WaitForTimeoutAsync(string profileId, int ms) =>
        PostAsync($"{Base(profileId)}/wait-for-timeout", new { ms });

    /// <summary>Wait for page load state: "load" | "domcontentloaded" | "networkidle".</summary>
    public Task<JsonNode?> WaitForLoadStateAsync(string profileId, string? state = null, int? timeoutMs = null)
    {
        var body = new Dictionary<string, object>();
        if (state is not null) body["state"] = state;
        if (timeoutMs.HasValue) body["timeout"] = timeoutMs.Value;
        return PostAsync($"{Base(profileId)}/wait-for-load-state", body.Count > 0 ? body : null);
    }

    // ── State checks ───────────────────────────────────────────────────────────

    /// <summary>Returns <c>{{ "visible": bool }}</c> — no waiting.</summary>
    public Task<JsonNode?> IsVisibleAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/is-visible", new { selector });

    /// <summary>Returns <c>{{ "hidden": bool }}</c> — hidden or absent from DOM.</summary>
    public Task<JsonNode?> IsHiddenAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/is-hidden", new { selector });

    /// <summary>Returns <c>{{ "checked": bool }}</c> — checkbox/radio state.</summary>
    public Task<JsonNode?> IsCheckedAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/is-checked", new { selector });

    /// <summary>Returns <c>{{ "enabled": bool }}</c>.</summary>
    public Task<JsonNode?> IsEnabledAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/is-enabled", new { selector });

    /// <summary>Returns <c>{{ "disabled": bool }}</c>.</summary>
    public Task<JsonNode?> IsDisabledAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/is-disabled", new { selector });

    /// <summary>Returns <c>{{ "editable": bool }}</c> — not readonly/disabled.</summary>
    public Task<JsonNode?> IsEditableAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/is-editable", new { selector });

    /// <summary>Returns <c>{{ "text": string }}</c> — raw textContent including hidden text.</summary>
    public Task<JsonNode?> TextContentAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/text-content", new { selector });

    // ── Data extraction ────────────────────────────────────────────────────────

    public Task<JsonNode?> GetTextAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/get-text", new { selector });

    public Task<JsonNode?> GetAttributeAsync(string profileId, string selector, string attribute) =>
        PostAsync($"{Base(profileId)}/get-attribute", new { selector, attribute });

    public Task<JsonNode?> GetValueAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/get-value", new { selector });

    public Task<JsonNode?> GetInnerHtmlAsync(string profileId, string selector) =>
        PostAsync($"{Base(profileId)}/get-inner-html", new { selector });

    // ── Keyboard (low-level) ──────────────────────────────────────────────────

    /// <summary>Hold a key down (for modifier combos like Shift+Click).</summary>
    public Task<JsonNode?> KeyboardDownAsync(string profileId, string key) =>
        PostAsync($"{Base(profileId)}/keyboard/down", new { key });

    /// <summary>Release a held key.</summary>
    public Task<JsonNode?> KeyboardUpAsync(string profileId, string key) =>
        PostAsync($"{Base(profileId)}/keyboard/up", new { key });

    /// <summary>Type text char-by-char (page-wide, no selector).</summary>
    public Task<JsonNode?> KeyboardTypeAsync(string profileId, string text, int? delayMs = null)
    {
        var body = new Dictionary<string, object> { ["text"] = text };
        if (delayMs.HasValue) body["delay"] = delayMs.Value;
        return PostAsync($"{Base(profileId)}/keyboard/type", body);
    }

    /// <summary>Insert text instantly (supports unicode, no keystroke events).</summary>
    public Task<JsonNode?> KeyboardInsertTextAsync(string profileId, string text) =>
        PostAsync($"{Base(profileId)}/keyboard/insert-text", new { text });

    // ── Mouse (low-level) ─────────────────────────────────────────────────────

    /// <summary>Click at absolute coordinates. button: "left"|"right"|"middle".</summary>
    public Task<JsonNode?> MouseClickAsync(string profileId, int x, int y, string? button = null, int? clickCount = null)
    {
        var body = new Dictionary<string, object> { ["x"] = x, ["y"] = y };
        if (button is not null) body["button"] = button;
        if (clickCount.HasValue) body["clickCount"] = clickCount.Value;
        return PostAsync($"{Base(profileId)}/mouse/click", body);
    }

    /// <summary>Move mouse to absolute coordinates.</summary>
    public Task<JsonNode?> MouseMoveAsync(string profileId, int x, int y, int? steps = null)
    {
        var body = new Dictionary<string, object> { ["x"] = x, ["y"] = y };
        if (steps.HasValue) body["steps"] = steps.Value;
        return PostAsync($"{Base(profileId)}/mouse/move", body);
    }

    /// <summary>Double-click at coordinates.</summary>
    public Task<JsonNode?> MouseDblClickAsync(string profileId, int x, int y, string? button = null)
    {
        var body = new Dictionary<string, object> { ["x"] = x, ["y"] = y };
        if (button is not null) body["button"] = button;
        return PostAsync($"{Base(profileId)}/mouse/dblclick", body);
    }

    /// <summary>Press and hold mouse button.</summary>
    public Task<JsonNode?> MouseDownAsync(string profileId, string? button = null)
    {
        object? body = button is not null ? new { button } : null;
        return PostAsync($"{Base(profileId)}/mouse/down", body);
    }

    /// <summary>Release held mouse button.</summary>
    public Task<JsonNode?> MouseUpAsync(string profileId, string? button = null)
    {
        object? body = button is not null ? new { button } : null;
        return PostAsync($"{Base(profileId)}/mouse/up", body);
    }

    /// <summary>Scroll via mouse wheel delta.</summary>
    public Task<JsonNode?> MouseWheelAsync(string profileId, int deltaX, int deltaY) =>
        PostAsync($"{Base(profileId)}/mouse/wheel", new { deltaX, deltaY });

    // ── Page configuration ────────────────────────────────────────────────────

    /// <summary>Set custom HTTP headers for all subsequent requests on this page.</summary>
    public Task<JsonNode?> SetExtraHttpHeadersAsync(string profileId, Dictionary<string, string> headers) =>
        PostAsync($"{Base(profileId)}/set-extra-http-headers", new { headers });

    /// <summary>Inject JS to run before every page load on this context.</summary>
    public Task<JsonNode?> AddInitScriptAsync(string profileId, string script) =>
        PostAsync($"{Base(profileId)}/add-init-script", new { script });

    // ── Scripts ────────────────────────────────────────────────────────────────

    /// <summary>Evaluate a JS expression and return its result.</summary>
    public Task<JsonNode?> EvaluateAsync(string profileId, string expression) =>
        PostAsync($"{Base(profileId)}/evaluate", new { expression });

    /// <summary>Run a multi-line async script. Returns output from log() calls.</summary>
    public Task<JsonNode?> RunScriptAsync(string profileId, string script) =>
        PostAsync($"{Base(profileId)}/run-script", new { script });

    // ── Cookies ────────────────────────────────────────────────────────────────

    /// <summary>Get cookies, optionally filtered by URL list.</summary>
    public Task<JsonNode?> GetCookiesAsync(string profileId, IEnumerable<string>? urls = null)
    {
        var urlList = urls?.ToList();
        var query = urlList is { Count: > 0 }
            ? new Dictionary<string, string> { ["urls"] = string.Join(",", urlList) }
            : null;
        return GetAsync($"{Base(profileId)}/cookies", query);
    }

    /// <summary>Add cookies to the browser context.</summary>
    public Task<JsonNode?> SetCookiesAsync(string profileId, IEnumerable<object> cookies) =>
        PostAsync($"{Base(profileId)}/cookies", new { cookies });

    /// <summary>Clear all cookies from the browser context.</summary>
    public Task ClearCookiesAsync(string profileId) =>
        HttpDeleteAsync($"{Base(profileId)}/cookies");
}
