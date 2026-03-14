"""Profile CRUD operations — create, list, update, delete browser profiles."""

from typing import Any, Optional

from ._http import BaseApi


class ProfileApi(BaseApi):
    def list(self) -> list:
        """Return all profiles."""
        return self._get("/api/profiles")

    def create(
        self,
        name: str,
        headless: Optional[bool] = None,
        fingerprint_options: Optional[dict] = None,
        proxy: Optional[dict] = None,
    ) -> dict:
        """Create a new browser profile.

        fingerprint_options keys: os, browser, device, locale
        proxy keys: type (http/https/socks4/socks5), host, port, username, password
        """
        body: dict = {"name": name}
        if headless is not None:
            body["headless"] = headless
        if fingerprint_options:
            body["fingerprintOptions"] = fingerprint_options
        if proxy:
            body["proxy"] = proxy
        return self._post("/api/profiles", body)

    def update(
        self,
        profile_id: str,
        name: Optional[str] = None,
        headless: Optional[bool] = None,
    ) -> dict:
        """Update profile name and/or headless flag."""
        body: dict = {}
        if name is not None:
            body["name"] = name
        if headless is not None:
            body["headless"] = headless
        return self._put(f"/api/profiles/{profile_id}", body)

    def delete(self, profile_id: str) -> None:
        """Permanently delete a profile."""
        self._delete(f"/api/profiles/{profile_id}")
