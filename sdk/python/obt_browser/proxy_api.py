"""Proxy pool operations — CRUD, assign, and unassign."""

from typing import Any, Optional

from ._http import BaseApi


class ProxyApi(BaseApi):
    def list(self) -> list:
        """Return all proxies."""
        return self._get("/api/proxies")

    def list_unassigned(self) -> list:
        """Return proxies that are not assigned to any profile."""
        return self._get("/api/proxies/unassigned")

    def create(
        self,
        proxy_type: str,
        host: str,
        port: int,
        name: Optional[str] = None,
        username: Optional[str] = None,
        password: Optional[str] = None,
    ) -> dict:
        """Create a proxy. proxy_type must be http/https/socks4/socks5."""
        body: dict[str, Any] = {
            "type": proxy_type,
            "host": host,
            "port": port,
        }
        if name is not None:
            body["name"] = name
        if username is not None:
            body["username"] = username
        if password is not None:
            body["password"] = password
        return self._post("/api/proxies", body)

    def update(
        self,
        proxy_id: str,
        name: Optional[str] = None,
        proxy_type: Optional[str] = None,
        host: Optional[str] = None,
        port: Optional[int] = None,
        username: Optional[str] = None,
        password: Optional[str] = None,
    ) -> dict:
        """Update one or more proxy fields."""
        body: dict[str, Any] = {}
        if name is not None:
            body["name"] = name
        if proxy_type is not None:
            body["type"] = proxy_type
        if host is not None:
            body["host"] = host
        if port is not None:
            body["port"] = port
        if username is not None:
            body["username"] = username
        if password is not None:
            body["password"] = password
        return self._put(f"/api/proxies/{proxy_id}", body)

    def delete(self, proxy_id: str) -> None:
        """Delete a proxy."""
        self._delete(f"/api/proxies/{proxy_id}")

    def assign(self, proxy_id: str, profile_id: str) -> dict:
        """Assign a proxy to a profile."""
        return self._post(f"/api/proxies/{proxy_id}/assign", {"profileId": profile_id})

    def unassign(self, profile_id: str) -> dict:
        """Unassign the current proxy from a profile."""
        return self._post("/api/proxies/unassign", {"profileId": profile_id})

    def clear_credentials(self, proxy_id: str) -> dict:
        """Clear stored proxy username and password."""
        return self._put(
            f"/api/proxies/{proxy_id}",
            {
                "username": None,
                "password": None,
            },
        )