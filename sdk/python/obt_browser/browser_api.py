"""Browser lifecycle, navigation, page info, element actions, scripts, cookies, and generic execution."""

from typing import Any, List, Optional, Union

from ._http import BaseApi


class BrowserApi(BaseApi):
    # ── Lifecycle ──────────────────────────────────────────────────────────────

    def launch(self, profile_id: str, headless: Optional[bool] = None) -> dict:
        """Launch browser for a profile."""
        body = {}
        if headless is not None:
            body["headless"] = headless
        return self._post(f"/api/browsers/{profile_id}/launch", body or None)

    def close(self, profile_id: str) -> dict:
        """Close browser for a profile."""
        return self._post(f"/api/browsers/{profile_id}/close")

    def status(self, profile_id: str) -> dict:
        """Return {'running': bool} for a profile."""
        return self._get(f"/api/browsers/{profile_id}/status")

    # ── Navigation ─────────────────────────────────────────────────────────────

    def navigate(self, profile_id: str, url: str, timeout: Optional[int] = None) -> dict:
        """Navigate the active page to a URL."""
        body: dict = {"url": url}
        if timeout is not None:
            body["timeout"] = timeout
        return self._post(f"/api/browsers/{profile_id}/actions/navigate", body)

    def reload(self, profile_id: str, timeout: Optional[int] = None) -> dict:
        """Reload the current page."""
        body = {"timeout": timeout} if timeout is not None else None
        return self._post(f"/api/browsers/{profile_id}/actions/reload", body)

    def go_back(self, profile_id: str) -> dict:
        return self._post(f"/api/browsers/{profile_id}/actions/go-back")

    def go_forward(self, profile_id: str) -> dict:
        return self._post(f"/api/browsers/{profile_id}/actions/go-forward")

    # ── Page info ──────────────────────────────────────────────────────────────

    def page_info(self, profile_id: str) -> dict:
        """Return current URL and page title."""
        return self._get(f"/api/browsers/{profile_id}/actions/page-info")

    def content(self, profile_id: str) -> dict:
        """Return full page HTML."""
        return self._get(f"/api/browsers/{profile_id}/actions/content")

    def screenshot(self, profile_id: str, full_page: Optional[bool] = None) -> dict:
        """Capture screenshot; returns base64 PNG in response."""
        body = {"fullPage": full_page} if full_page is not None else None
        return self._post(f"/api/browsers/{profile_id}/actions/screenshot", body)

    # ── Element interactions ───────────────────────────────────────────────────

    def click(self, profile_id: str, selector: str, button: Optional[str] = None) -> dict:
        """Click an element. button: 'left'|'right'|'middle'."""
        body: dict = {"selector": selector}
        if button:
            body["button"] = button
        return self._post(f"/api/browsers/{profile_id}/actions/click", body)

    def double_click(self, profile_id: str, selector: str) -> dict:
        return self._post(f"/api/browsers/{profile_id}/actions/double-click", {"selector": selector})

    def hover(self, profile_id: str, selector: str) -> dict:
        return self._post(f"/api/browsers/{profile_id}/actions/hover", {"selector": selector})

    def focus(self, profile_id: str, selector: str) -> dict:
        return self._post(f"/api/browsers/{profile_id}/actions/focus", {"selector": selector})

    def fill(self, profile_id: str, selector: str, value: str) -> dict:
        """Clear and fill a form field."""
        return self._post(f"/api/browsers/{profile_id}/actions/fill", {"selector": selector, "value": value})

    def type_text(self, profile_id: str, selector: str, text: str, delay: Optional[int] = None) -> dict:
        """Type text char-by-char with optional keystroke delay (ms)."""
        body: dict = {"selector": selector, "text": text}
        if delay is not None:
            body["delay"] = delay
        return self._post(f"/api/browsers/{profile_id}/actions/type", body)

    def press_key(self, profile_id: str, key: str, selector: Optional[str] = None) -> dict:
        """Press a keyboard key (e.g. 'Enter', 'Tab'). Optionally focus selector first."""
        body: dict = {"key": key}
        if selector:
            body["selector"] = selector
        return self._post(f"/api/browsers/{profile_id}/actions/press-key", body)

    def select_option(self, profile_id: str, selector: str, value: Union[str, list]) -> dict:
        """Select option(s) in a <select> element."""
        return self._post(f"/api/browsers/{profile_id}/actions/select-option", {"selector": selector, "value": value})

    def check(self, profile_id: str, selector: str, checked: bool = True) -> dict:
        """Check or uncheck a checkbox."""
        return self._post(f"/api/browsers/{profile_id}/actions/check", {"selector": selector, "checked": checked})

    def scroll(self, profile_id: str, x: int = 0, y: int = 0, selector: Optional[str] = None) -> dict:
        """Scroll page by (x, y) delta, or scroll element into view if selector given."""
        body: dict = {"x": x, "y": y}
        if selector:
            body["selector"] = selector
        return self._post(f"/api/browsers/{profile_id}/actions/scroll", body)

    def tap(self, profile_id: str, selector: str) -> dict:
        """Mobile touch tap on element."""
        return self._post(f"/api/browsers/{profile_id}/actions/tap", {"selector": selector})

    def drag_and_drop(self, profile_id: str, source: str, target: str) -> dict:
        """Drag element from source selector to target selector."""
        return self._post(f"/api/browsers/{profile_id}/actions/drag-and-drop", {"source": source, "target": target})

    def dispatch_event(self, profile_id: str, selector: str, event_type: str) -> dict:
        """Fire a DOM event (e.g. 'click', 'input') on element."""
        return self._post(f"/api/browsers/{profile_id}/actions/dispatch-event", {"selector": selector, "type": event_type})

    def set_viewport_size(self, profile_id: str, width: int, height: int) -> dict:
        """Resize browser viewport."""
        return self._post(f"/api/browsers/{profile_id}/actions/set-viewport-size", {"width": width, "height": height})

    def set_content(self, profile_id: str, html: str) -> dict:
        """Replace page content with an HTML string."""
        return self._post(f"/api/browsers/{profile_id}/actions/set-content", {"html": html})

    def wait_for_navigation(self, profile_id: str, timeout: Optional[int] = None) -> dict:
        """Wait for next page navigation to complete."""
        body = {"timeout": timeout} if timeout is not None else None
        return self._post(f"/api/browsers/{profile_id}/actions/wait-for-navigation", body)

    # ── Waits ──────────────────────────────────────────────────────────────────

    def wait_for_selector(self, profile_id: str, selector: str, timeout: Optional[int] = None) -> dict:
        body: dict = {"selector": selector}
        if timeout is not None:
            body["timeout"] = timeout
        return self._post(f"/api/browsers/{profile_id}/actions/wait-for-selector", body)

    def wait_for_url(self, profile_id: str, pattern: str, timeout: Optional[int] = None) -> dict:
        body: dict = {"pattern": pattern}
        if timeout is not None:
            body["timeout"] = timeout
        return self._post(f"/api/browsers/{profile_id}/actions/wait-for-url", body)

    def wait_for_timeout(self, profile_id: str, ms: int) -> dict:
        """Pause execution for a fixed number of milliseconds."""
        return self._post(f"/api/browsers/{profile_id}/actions/wait-for-timeout", {"ms": ms})

    def wait_for_load_state(self, profile_id: str, state: Optional[str] = None, timeout: Optional[int] = None) -> dict:
        """Wait for 'load' | 'domcontentloaded' | 'networkidle'."""
        body: dict = {}
        if state:
            body["state"] = state
        if timeout is not None:
            body["timeout"] = timeout
        return self._post(f"/api/browsers/{profile_id}/actions/wait-for-load-state", body or None)

    # ── State checks ───────────────────────────────────────────────────────────

    def is_visible(self, profile_id: str, selector: str) -> dict:
        """Return {'visible': bool} — no waiting."""
        return self._post(f"/api/browsers/{profile_id}/actions/is-visible", {"selector": selector})

    def is_hidden(self, profile_id: str, selector: str) -> dict:
        """Return {'hidden': bool} — hidden or absent from DOM."""
        return self._post(f"/api/browsers/{profile_id}/actions/is-hidden", {"selector": selector})

    def is_checked(self, profile_id: str, selector: str) -> dict:
        """Return {'checked': bool} — checkbox/radio state."""
        return self._post(f"/api/browsers/{profile_id}/actions/is-checked", {"selector": selector})

    def is_enabled(self, profile_id: str, selector: str) -> dict:
        """Return {'enabled': bool}."""
        return self._post(f"/api/browsers/{profile_id}/actions/is-enabled", {"selector": selector})

    def is_disabled(self, profile_id: str, selector: str) -> dict:
        """Return {'disabled': bool}."""
        return self._post(f"/api/browsers/{profile_id}/actions/is-disabled", {"selector": selector})

    def is_editable(self, profile_id: str, selector: str) -> dict:
        """Return {'editable': bool} — not readonly/disabled."""
        return self._post(f"/api/browsers/{profile_id}/actions/is-editable", {"selector": selector})

    def text_content(self, profile_id: str, selector: str) -> dict:
        """Return {'text': str} — raw textContent including hidden text."""
        return self._post(f"/api/browsers/{profile_id}/actions/text-content", {"selector": selector})

    # ── Data extraction ────────────────────────────────────────────────────────

    def get_text(self, profile_id: str, selector: str) -> dict:
        return self._post(f"/api/browsers/{profile_id}/actions/get-text", {"selector": selector})

    def get_attribute(self, profile_id: str, selector: str, attribute: str) -> dict:
        return self._post(f"/api/browsers/{profile_id}/actions/get-attribute", {"selector": selector, "attribute": attribute})

    def get_value(self, profile_id: str, selector: str) -> dict:
        return self._post(f"/api/browsers/{profile_id}/actions/get-value", {"selector": selector})

    def get_inner_html(self, profile_id: str, selector: str) -> dict:
        return self._post(f"/api/browsers/{profile_id}/actions/get-inner-html", {"selector": selector})

    # ── Keyboard (low-level) ──────────────────────────────────────────────────

    def keyboard_down(self, profile_id: str, key: str) -> dict:
        """Hold a key down (for modifier combos like Shift+Click)."""
        return self._post(f"/api/browsers/{profile_id}/actions/keyboard/down", {"key": key})

    def keyboard_up(self, profile_id: str, key: str) -> dict:
        """Release a held key."""
        return self._post(f"/api/browsers/{profile_id}/actions/keyboard/up", {"key": key})

    def keyboard_type(self, profile_id: str, text: str, delay: Optional[int] = None) -> dict:
        """Type text char-by-char (page-wide, no selector needed)."""
        body: dict = {"text": text}
        if delay is not None:
            body["delay"] = delay
        return self._post(f"/api/browsers/{profile_id}/actions/keyboard/type", body)

    def keyboard_insert_text(self, profile_id: str, text: str) -> dict:
        """Insert text instantly (supports unicode, no keystroke events)."""
        return self._post(f"/api/browsers/{profile_id}/actions/keyboard/insert-text", {"text": text})

    # ── Mouse (low-level) ─────────────────────────────────────────────────────

    def mouse_click(self, profile_id: str, x: int, y: int, button: Optional[str] = None, click_count: Optional[int] = None) -> dict:
        """Click at absolute coordinates."""
        body: dict = {"x": x, "y": y}
        if button:
            body["button"] = button
        if click_count is not None:
            body["clickCount"] = click_count
        return self._post(f"/api/browsers/{profile_id}/actions/mouse/click", body)

    def mouse_move(self, profile_id: str, x: int, y: int, steps: Optional[int] = None) -> dict:
        """Move mouse to absolute coordinates."""
        body: dict = {"x": x, "y": y}
        if steps is not None:
            body["steps"] = steps
        return self._post(f"/api/browsers/{profile_id}/actions/mouse/move", body)

    def mouse_dblclick(self, profile_id: str, x: int, y: int, button: Optional[str] = None) -> dict:
        """Double-click at coordinates."""
        body: dict = {"x": x, "y": y}
        if button:
            body["button"] = button
        return self._post(f"/api/browsers/{profile_id}/actions/mouse/dblclick", body)

    def mouse_down(self, profile_id: str, button: Optional[str] = None) -> dict:
        """Press and hold mouse button."""
        body = {"button": button} if button else None
        return self._post(f"/api/browsers/{profile_id}/actions/mouse/down", body)

    def mouse_up(self, profile_id: str, button: Optional[str] = None) -> dict:
        """Release held mouse button."""
        body = {"button": button} if button else None
        return self._post(f"/api/browsers/{profile_id}/actions/mouse/up", body)

    def mouse_wheel(self, profile_id: str, delta_x: int, delta_y: int) -> dict:
        """Scroll via mouse wheel delta."""
        return self._post(f"/api/browsers/{profile_id}/actions/mouse/wheel", {"deltaX": delta_x, "deltaY": delta_y})

    # ── Page configuration ────────────────────────────────────────────────────

    def set_extra_http_headers(self, profile_id: str, headers: dict) -> dict:
        """Set custom HTTP headers for all subsequent requests on this page."""
        return self._post(f"/api/browsers/{profile_id}/actions/set-extra-http-headers", {"headers": headers})

    def add_init_script(self, profile_id: str, script: str) -> dict:
        """Inject JS to run before every page load on this context."""
        return self._post(f"/api/browsers/{profile_id}/actions/add-init-script", {"script": script})

    # ── Scripts ────────────────────────────────────────────────────────────────

    def evaluate(self, profile_id: str, expression: str) -> dict:
        """Evaluate a JS expression and return its result."""
        return self._post(f"/api/browsers/{profile_id}/actions/evaluate", {"expression": expression})

    def run_script(self, profile_id: str, script: str) -> dict:
        """Run a multi-line async script; returns {'output': str} from log() calls."""
        return self._post(f"/api/browsers/{profile_id}/actions/run-script", {"script": script})

    # ── Cookies ────────────────────────────────────────────────────────────────

    def get_cookies(self, profile_id: str, urls: Optional[List[str]] = None) -> dict:
        """Return cookies; optionally filter by URL list."""
        params = {"urls": ",".join(urls)} if urls else None
        return self._get(f"/api/browsers/{profile_id}/actions/cookies", params)

    def set_cookies(self, profile_id: str, cookies: list) -> dict:
        """Add cookies to the browser context."""
        return self._post(f"/api/browsers/{profile_id}/actions/cookies", {"cookies": cookies})

    def clear_cookies(self, profile_id: str) -> dict:
        """Clear all cookies from the browser context."""
        return self._delete(f"/api/browsers/{profile_id}/actions/cookies")

    # ── Generic Playwright execution ──────────────────────────────────────────

    def execute(self, profile_id: str, method: str, args: Optional[list] = None, chain: Optional[list] = None) -> dict:
        """Execute any Playwright Page method dynamically.

        Args:
            profile_id: Profile ID.
            method: Playwright Page method name (e.g. 'getByText', 'locator').
            args: Arguments to pass to the method.
            chain: List of {'method': str, 'args'?: list} dicts to call on the result.

        Examples::

            # Click element by visible text
            client.browsers.execute(pid, "getByText", ["Submit"], [{"method": "click"}])

            # Fill input by label
            client.browsers.execute(pid, "getByLabel", ["Email"],
                [{"method": "fill", "args": ["user@example.com"]}])

            # Count table rows
            client.browsers.execute(pid, "locator", ["table tbody tr"],
                [{"method": "count"}])

            # Get all hrefs on page
            client.browsers.execute(pid, "evaluate",
                ["() => Array.from(document.querySelectorAll('a')).map(a => a.href)"])
        """
        body: dict = {"method": method}
        if args is not None:
            body["args"] = args
        if chain is not None:
            body["chain"] = chain
        return self._post(f"/api/browsers/{profile_id}/execute", body)
