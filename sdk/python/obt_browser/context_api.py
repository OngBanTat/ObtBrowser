"""Browser context operations — multi-tab management, geolocation, permissions, storage."""

from typing import List, Optional

from ._http import BaseApi


class ContextApi(BaseApi):
    """Context-level operations affecting all pages in a browser session.

    Access via ``client.context``.

    Examples::

        client.context.new_page(pid)
        client.context.set_geolocation(pid, 10.823, 106.629)
        state = client.context.storage_state(pid)
        client.context.grant_permissions(pid, ["geolocation"])
    """

    def _base(self, profile_id: str) -> str:
        return f"/api/browsers/{profile_id}/context"

    def storage_state(self, profile_id: str) -> dict:
        """Export cookies + localStorage for session reuse."""
        return self._get(f"{self._base(profile_id)}/storage-state")

    def new_page(self, profile_id: str) -> dict:
        """Open a new tab in the browser context."""
        return self._post(f"{self._base(profile_id)}/new-page")

    def pages(self, profile_id: str) -> dict:
        """Return {'count': int, 'urls': [...]} listing all open tabs."""
        return self._get(f"{self._base(profile_id)}/pages")

    def set_extra_http_headers(self, profile_id: str, headers: dict) -> dict:
        """Set HTTP headers for all pages in this context."""
        return self._post(f"{self._base(profile_id)}/extra-http-headers", {"headers": headers})

    def grant_permissions(self, profile_id: str, permissions: List[str], origin: Optional[str] = None) -> dict:
        """Grant browser permissions.

        permissions: 'geolocation'|'camera'|'microphone'|'notifications'|
                     'clipboard-read'|'clipboard-write'
        """
        body: dict = {"permissions": permissions}
        if origin:
            body["origin"] = origin
        return self._post(f"{self._base(profile_id)}/grant-permissions", body)

    def clear_permissions(self, profile_id: str) -> dict:
        """Revoke all granted permissions."""
        return self._post(f"{self._base(profile_id)}/clear-permissions")

    def set_geolocation(self, profile_id: str, latitude: float, longitude: float, accuracy: Optional[float] = None) -> dict:
        """Spoof GPS location."""
        body: dict = {"latitude": latitude, "longitude": longitude}
        if accuracy is not None:
            body["accuracy"] = accuracy
        return self._post(f"{self._base(profile_id)}/geolocation", body)
