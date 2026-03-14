"""Task management — create, run, cancel, and delete automation tasks."""

from typing import Optional

from ._http import BaseApi


class TaskApi(BaseApi):
    def list(self, profile_id: Optional[str] = None) -> list:
        """List all tasks, optionally filtered by profile ID."""
        params = {"profileId": profile_id} if profile_id else None
        return self._get("/api/tasks", params)

    def create(
        self,
        profile_id: str,
        name: str,
        script_type: str,
        script_content: str,
    ) -> dict:
        """Create an automation task.

        script_type: 'inline' (script string) or 'file' (path to script file)
        """
        return self._post("/api/tasks", {
            "profileId": profile_id,
            "name": name,
            "scriptType": script_type,
            "scriptContent": script_content,
        })

    def run(self, task_id: str) -> dict:
        """Enqueue a task for execution."""
        return self._post(f"/api/tasks/{task_id}/run")

    def cancel(self, task_id: str) -> dict:
        """Cancel a queued or running task."""
        return self._post(f"/api/tasks/{task_id}/cancel")

    def delete(self, task_id: str) -> None:
        """Delete a task record."""
        self._delete(f"/api/tasks/{task_id}")
