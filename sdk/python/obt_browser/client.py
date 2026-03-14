"""Main entry point — ObtBrowserClient composes all API sub-clients."""

from ._http import BaseApi
from .browser_api import BrowserApi
from .context_api import ContextApi
from .profile_api import ProfileApi
from .proxy_api import ProxyApi
from .task_api import TaskApi


class ObtBrowserClient:
    """Client for the ObtAntiDetect Browser REST API.

    Usage::

        client = ObtBrowserClient("http://localhost:3000")

        profile = client.profiles.create("My Profile")
        pid = profile["id"]

        client.browsers.launch(pid)
        client.browsers.navigate(pid, "https://example.com")
        print(client.browsers.page_info(pid))

        # Fill form using Playwright locators
        client.browsers.execute(pid, "getByLabel", ["Email"],
            [{"method": "fill", "args": ["user@example.com"]}])
        client.browsers.execute(pid, "getByRole", ["button", {"name": "Login"}],
            [{"method": "click"}])

        # Wait for navigation, check result
        client.browsers.wait_for_load_state(pid, "networkidle")
        print(client.browsers.is_visible(pid, ".dashboard"))

        # Browser context operations
        client.context.new_page(pid)
        client.context.set_geolocation(pid, 10.823, 106.629)

        client.browsers.close(pid)
    """

    def __init__(self, base_url: str = "http://localhost:3000") -> None:
        self.profiles = ProfileApi(base_url)
        self.browsers = BrowserApi(base_url)
        self.context = ContextApi(base_url)
        self.tasks = TaskApi(base_url)
        self.proxies = ProxyApi(base_url)
        self._base = BaseApi(base_url)

    def health(self) -> dict:
        """Check API server health."""
        return self._base._get("/api/health")
