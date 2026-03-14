"""Base HTTP client using stdlib urllib — no external dependencies required."""

import json
import urllib.error
import urllib.parse
import urllib.request
from typing import Any, Optional


class HttpError(Exception):
    """Raised when the API returns a 4xx or 5xx response."""

    def __init__(self, status: int, message: str) -> None:
        self.status = status
        super().__init__(f"HTTP {status}: {message}")


class BaseApi:
    """Shared HTTP helpers for all API sub-clients."""

    def __init__(self, base_url: str) -> None:
        self._base_url = base_url.rstrip("/")

    def _request(
        self,
        method: str,
        path: str,
        body: Optional[dict] = None,
        params: Optional[dict] = None,
    ) -> Any:
        url = self._base_url + path
        if params:
            filtered = {k: str(v) for k, v in params.items() if v is not None}
            if filtered:
                url = f"{url}?{urllib.parse.urlencode(filtered)}"

        data = json.dumps(body).encode("utf-8") if body is not None else None
        headers = {"Content-Type": "application/json", "Accept": "application/json"}
        req = urllib.request.Request(url, data=data, headers=headers, method=method)

        try:
            with urllib.request.urlopen(req) as resp:
                raw = resp.read()
                return json.loads(raw) if raw else None
        except urllib.error.HTTPError as exc:
            text = exc.read().decode("utf-8", errors="replace")
            try:
                msg = json.loads(text).get("error", text)
            except Exception:
                msg = text
            raise HttpError(exc.code, msg) from exc

    def _get(self, path: str, params: Optional[dict] = None) -> Any:
        return self._request("GET", path, params=params)

    def _post(self, path: str, body: Optional[dict] = None) -> Any:
        return self._request("POST", path, body=body)

    def _put(self, path: str, body: dict) -> Any:
        return self._request("PUT", path, body=body)

    def _delete(self, path: str) -> Any:
        return self._request("DELETE", path)
