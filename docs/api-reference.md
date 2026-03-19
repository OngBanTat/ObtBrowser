# REST API Reference

Base URL: `http://localhost:{port}` (default port `3000`, configure in Settings → API Server)

Interactive docs (Swagger UI): `GET /docs`

All responses use JSON. Error responses: `{ "error": "message" }`.

**⚡ Generic Playwright Execution:** The `/api/browsers/:profileId/execute` endpoint allows you to call any Playwright Page method not covered by specific endpoints (e.g., `getByText`, `getByRole`, `locator`). See [Generic Playwright Execution](#generic-playwright-execution) section for details.

---

## Health

| Method | Path          | Description                                |
| ------ | ------------- | ------------------------------------------ |
| GET    | `/api/health` | Server health check → `{ "status": "ok" }` |

---

## Profiles `/api/profiles`

| Method | Path                | Body                                               | Description          |
| ------ | ------------------- | -------------------------------------------------- | -------------------- |
| GET    | `/api/profiles`     | —                                                  | List all profiles    |
| POST   | `/api/profiles`     | `{ name, headless?, fingerprintOptions?, proxy? }` | Create profile (201) |
| PUT    | `/api/profiles/:id` | `{ name?, headless? }`                             | Update profile       |
| DELETE | `/api/profiles/:id` | —                                                  | Delete profile (204) |

**fingerprintOptions:** `{ os?, browser?, device?, locale? }`
**proxy (inline):** `{ type, host, port, username?, password? }`

---

## Browsers `/api/browsers`

| Method | Path                              | Body            | Description                |
| ------ | --------------------------------- | --------------- | -------------------------- |
| POST   | `/api/browsers/:profileId/launch` | `{ headless? }` | Launch browser for profile |
| POST   | `/api/browsers/:profileId/close`  | —               | Close browser              |
| GET    | `/api/browsers/:profileId/status` | —               | `{ "running": boolean }`   |

All browser action routes below require the browser to be launched first.

---

## Browser Actions `/api/browsers/:profileId/actions`

### Navigation

| Method | Path             | Body                | Description                     |
| ------ | ---------------- | ------------------- | ------------------------------- |
| POST   | `.../navigate`   | `{ url, timeout? }` | Navigate to URL                 |
| POST   | `.../reload`     | `{ timeout? }`      | Reload page                     |
| POST   | `.../go-back`    | —                   | Browser back                    |
| POST   | `.../go-forward` | —                   | Browser forward                 |
| GET    | `.../page-info`  | —                   | `{ url, title }`                |
| GET    | `.../content`    | —                   | `{ html }` — full page HTML     |
| POST   | `.../screenshot` | `{ fullPage? }`     | `{ data: base64PNG, mimeType }` |

### Element Interactions

| Method | Path                | Body                                             | Description                                 |
| ------ | ------------------- | ------------------------------------------------ | ------------------------------------------- |
| POST   | `.../click`         | `{ selector, button? }`                          | Click element (`button`: left/right/middle) |
| POST   | `.../double-click`  | `{ selector }`                                   | Double-click element                        |
| POST   | `.../hover`         | `{ selector }`                                   | Hover over element                          |
| POST   | `.../focus`         | `{ selector }`                                   | Focus element                               |
| POST   | `.../fill`          | `{ selector, value }`                            | Clear and fill input field                  |
| POST   | `.../type`          | `{ selector, text, delay? }`                     | Type char-by-char (human-like)              |
| POST   | `.../press-key`     | `{ key, selector? }`                             | Press keyboard key (Enter, Tab, etc.)       |
| POST   | `.../select-option` | `{ selector, value }` or `{ selector, options }` | Select dropdown option(s)                   |
| POST   | `.../check`         | `{ selector, checked? }`                         | Check/uncheck checkbox                      |
| POST   | `.../scroll`        | `{ x?, y?, selector? }`                          | Scroll page or element into view            |

`select-option` accepts Playwright-compatible input:

```json
{
  "selector": "select#country",
  "value": "us"
}
```

```json
{
  "selector": "select#country",
  "value": { "label": "United States" }
}
```

```json
{
  "selector": "select#countries",
  "options": [{ "value": "us" }, { "index": 2 }]
}
```

### Wait / Assert

| Method | Path                      | Body                     | Description                                                 |
| ------ | ------------------------- | ------------------------ | ----------------------------------------------------------- |
| POST   | `.../wait-for-selector`   | `{ selector, timeout? }` | Wait for element in DOM (408 on timeout)                    |
| POST   | `.../wait-for-url`        | `{ pattern, timeout? }`  | Wait for URL glob match (408 on timeout)                    |
| POST   | `.../wait-for-timeout`    | `{ ms }`                 | Pause for fixed duration                                    |
| POST   | `.../wait-for-load-state` | `{ state?, timeout? }`   | Wait for load/domcontentloaded/networkidle (408 on timeout) |

### State Checks

| Method | Path               | Body           | Description                                       |
| ------ | ------------------ | -------------- | ------------------------------------------------- |
| POST   | `.../is-visible`   | `{ selector }` | `{ visible: boolean }` — no waiting               |
| POST   | `.../is-hidden`    | `{ selector }` | `{ hidden: boolean }` — hidden or absent from DOM |
| POST   | `.../is-checked`   | `{ selector }` | `{ checked: boolean }` — checkbox/radio state     |
| POST   | `.../is-enabled`   | `{ selector }` | `{ enabled: boolean }` — element enabled state    |
| POST   | `.../is-disabled`  | `{ selector }` | `{ disabled: boolean }` — element disabled state  |
| POST   | `.../is-editable`  | `{ selector }` | `{ editable: boolean }` — not readonly/disabled   |
| POST   | `.../text-content` | `{ selector }` | `{ text }` — raw textContent (incl. hidden)       |

### Data Extraction

| Method | Path                 | Body                      | Description                      |
| ------ | -------------------- | ------------------------- | -------------------------------- |
| POST   | `.../get-text`       | `{ selector }`            | `{ text }` — innerText           |
| POST   | `.../get-attribute`  | `{ selector, attribute }` | `{ value }` — attribute value    |
| POST   | `.../get-value`      | `{ selector }`            | `{ value }` — input/select value |
| POST   | `.../get-inner-html` | `{ selector }`            | `{ html }` — innerHTML           |

### Element Actions (extended)

| Method | Path                      | Body                 | Description                                             |
| ------ | ------------------------- | -------------------- | ------------------------------------------------------- |
| POST   | `.../tap`                 | `{ selector }`       | Mobile touch tap on element                             |
| POST   | `.../drag-and-drop`       | `{ source, target }` | Drag element from source to target selector             |
| POST   | `.../dispatch-event`      | `{ selector, type }` | Fire a DOM event (e.g. `'click'`, `'input'`) on element |
| POST   | `.../set-viewport-size`   | `{ width, height }`  | Resize browser viewport                                 |
| POST   | `.../set-content`         | `{ html }`           | Replace page content with HTML string                   |
| POST   | `.../wait-for-navigation` | `{ timeout? }`       | Wait for next page navigation to complete               |

### Keyboard (low-level)

All keyboard routes: `/api/browsers/:profileId/actions/keyboard/*`

| Method | Path                       | Body               | Description                              |
| ------ | -------------------------- | ------------------ | ---------------------------------------- |
| POST   | `.../keyboard/down`        | `{ key }`          | Hold key down (for modifier combos)      |
| POST   | `.../keyboard/up`          | `{ key }`          | Release held key                         |
| POST   | `.../keyboard/type`        | `{ text, delay? }` | Type text char-by-char                   |
| POST   | `.../keyboard/insert-text` | `{ text }`         | Insert text instantly (supports unicode) |

### Mouse (low-level)

All mouse routes: `/api/browsers/:profileId/actions/mouse/*`

| Method | Path                 | Body                             | Description                  |
| ------ | -------------------- | -------------------------------- | ---------------------------- |
| POST   | `.../mouse/click`    | `{ x, y, button?, clickCount? }` | Click at coordinates         |
| POST   | `.../mouse/move`     | `{ x, y, steps? }`               | Move mouse to coordinates    |
| POST   | `.../mouse/dblclick` | `{ x, y, button? }`              | Double-click at coordinates  |
| POST   | `.../mouse/down`     | `{ button? }`                    | Press and hold mouse button  |
| POST   | `.../mouse/up`       | `{ button? }`                    | Release held mouse button    |
| POST   | `.../mouse/wheel`    | `{ deltaX, deltaY }`             | Scroll via mouse wheel delta |

### Page Configuration

| Method | Path                         | Body                      | Description                                             |
| ------ | ---------------------------- | ------------------------- | ------------------------------------------------------- |
| POST   | `.../set-extra-http-headers` | `{ headers: {key: val} }` | Set custom HTTP headers for all subsequent requests     |
| POST   | `.../add-init-script`        | `{ script }`              | Inject JS to run before every page load on this context |

### Script Execution

| Method | Path             | Body             | Description                                                                               |
| ------ | ---------------- | ---------------- | ----------------------------------------------------------------------------------------- |
| POST   | `.../evaluate`   | `{ expression }` | Eval JS expression → `{ result }`                                                         |
| POST   | `.../run-script` | `{ script }`     | Run multi-line script; globals: `page` (Playwright Page), `context` (BrowserContext), `profileId`, `log(msg)` → `{ output }` |

### Generic Playwright Execution

**POST** `/api/browsers/:profileId/execute`

Execute any Playwright Page method dynamically. This endpoint covers Playwright methods not exposed through specific REST endpoints.

**Request Body:**

```json
{
  "method": "string",        // Playwright Page method name
  "args": [...],             // Array of arguments (optional)
  "chain": [                 // Optional: chain methods on the result
    { "method": "click", "args": [] }
  ]
}
```

**Common use cases:**

| Method             | Args                         | Description                    | Example                                                                                                      |
| ------------------ | ---------------------------- | ------------------------------ | ------------------------------------------------------------------------------------------------------------ |
| `getByText`        | `["text", options?]`         | Get element by visible text    | `{ "method": "getByText", "args": ["Submit"], "chain": [{"method": "click"}] }`                              |
| `getByRole`        | `["role", options?]`         | Get element by ARIA role       | `{ "method": "getByRole", "args": ["button", {"name": "Login"}], "chain": [{"method": "click"}] }`           |
| `getByLabel`       | `["text", options?]`         | Get input by label text        | `{ "method": "getByLabel", "args": ["Email"], "chain": [{"method": "fill", "args": ["user@example.com"]}] }` |
| `getByPlaceholder` | `["text", options?]`         | Get input by placeholder       | `{ "method": "getByPlaceholder", "args": ["Search..."], "chain": [{"method": "fill", "args": ["query"]}] }`  |
| `getByTestId`      | `["testId"]`                 | Get element by test ID         | `{ "method": "getByTestId", "args": ["submit-btn"], "chain": [{"method": "click"}] }`                        |
| `getByTitle`       | `["text", options?]`         | Get element by title attribute | `{ "method": "getByTitle", "args": ["Close"], "chain": [{"method": "click"}] }`                              |
| `getByAltText`     | `["text", options?]`         | Get image by alt text          | `{ "method": "getByAltText", "args": ["Logo"], "chain": [{"method": "click"}] }`                             |
| `locator`          | `["selector"]`               | Get locator by selector        | `{ "method": "locator", "args": [".submit-button"], "chain": [{"method": "click"}] }`                        |
| `frameLocator`     | `["selector"]`               | Get frame locator              | `{ "method": "frameLocator", "args": ["iframe#content"] }`                                                   |
| `waitForFunction`  | `["pageFunction", options?]` | Wait for JS condition          | `{ "method": "waitForFunction", "args": ["() => document.title === 'Ready'"] }`                              |
| `addStyleTag`      | `[{ content }]`              | Inject CSS                     | `{ "method": "addStyleTag", "args": [{"content": "body { margin: 0 }"}] }`                                   |
| `addScriptTag`     | `[{ content }]`              | Inject JavaScript              | `{ "method": "addScriptTag", "args": [{"content": "console.log('loaded')"}] }`                               |
| `setViewportSize`  | `[{ width, height }]`        | Resize viewport                | `{ "method": "setViewportSize", "args": [{"width": 1920, "height": 1080}] }`                                 |
| `emulateMedia`     | `[{ media?, colorScheme? }]` | Emulate media type             | `{ "method": "emulateMedia", "args": [{"colorScheme": "dark"}] }`                                            |
| `pdf`              | `[options?]`                 | Generate PDF (headless)        | `{ "method": "pdf", "args": [{"path": "page.pdf"}] }`                                                        |
| `bringToFront`     | `[]`                         | Bring page to front            | `{ "method": "bringToFront" }`                                                                               |
| `pause`            | `[]`                         | Pause for debugging            | `{ "method": "pause" }`                                                                                      |

**Response:**

```json
{
  "success": true,
  "result": "..." // Return value (null for void methods, metadata for non-serializable types)
}
```

**Notes:**

- Locators and ElementHandles return metadata instead of actual objects (not serializable over REST)
- Use `chain` to call methods on the result (e.g., get element then click it)
- For complex interactions, consider using the `/run-script` endpoint instead

### Cookies

| Method | Path          | Body / Query         | Description                              |
| ------ | ------------- | -------------------- | ---------------------------------------- |
| GET    | `.../cookies` | `?urls=url1,url2`    | Get cookies (optionally filtered by URL) |
| POST   | `.../cookies` | `{ cookies: [...] }` | Add cookies to context                   |
| DELETE | `.../cookies` | —                    | Clear all cookies from context           |

**Cookie object:** `{ name, value, domain?, path?, expires?, httpOnly?, secure?, sameSite? }`

---

## Browser Context `/api/browsers/:profileId/context`

Context-level operations affecting all pages in the browser session.

| Method | Path                             | Body                                 | Description                                           |
| ------ | -------------------------------- | ------------------------------------ | ----------------------------------------------------- |
| GET    | `.../context/storage-state`      | —                                    | Export cookies + localStorage for session reuse       |
| POST   | `.../context/new-page`           | —                                    | Open a new tab in the browser context                 |
| GET    | `.../context/pages`              | —                                    | `{ count, urls[] }` — list all open tabs              |
| POST   | `.../context/extra-http-headers` | `{ headers: {key: val} }`            | Set headers for all pages in context                  |
| POST   | `.../context/grant-permissions`  | `{ permissions[], origin? }`         | Grant browser permissions (geolocation, camera, etc.) |
| POST   | `.../context/clear-permissions`  | —                                    | Revoke all granted permissions                        |
| POST   | `.../context/geolocation`        | `{ latitude, longitude, accuracy? }` | Spoof GPS location                                    |

**permissions:** `geolocation` | `camera` | `microphone` | `notifications` | `clipboard-read` | `clipboard-write`

---

## Tasks `/api/tasks`

| Method | Path                    | Body                                             | Description                          |
| ------ | ----------------------- | ------------------------------------------------ | ------------------------------------ |
| GET    | `/api/tasks`            | `?profileId=`                                    | List tasks (optional profile filter) |
| POST   | `/api/tasks`            | `{ profileId, name, scriptType, scriptContent }` | Create task (201)                    |
| POST   | `/api/tasks/:id/run`    | —                                                | Enqueue task for execution           |
| POST   | `/api/tasks/:id/cancel` | —                                                | Cancel queued/running task           |
| DELETE | `/api/tasks/:id`        | —                                                | Delete task record (204)             |

**scriptType:** `"file"` (path to script file) or `"inline"` (script string)

---

## Proxies `/api/proxies`

| Method | Path                      | Body                                                   | Description                                 |
| ------ | ------------------------- | ------------------------------------------------------ | ------------------------------------------- |
| GET    | `/api/proxies`            | —                                                      | List all proxies                            |
| GET    | `/api/proxies/unassigned` | —                                                      | List proxies not assigned to any profile    |
| POST   | `/api/proxies`            | `{ type, host, port, name?, username?, password? }`    | Create proxy (201)                          |
| PUT    | `/api/proxies/:id`        | `{ type?, host?, port?, name?, username?, password? }` | Update proxy                                |
| DELETE | `/api/proxies/:id`        | —                                                      | Delete proxy (204)                          |
| POST   | `/api/proxies/:id/assign` | `{ profileId }`                                        | Assign proxy to profile                     |
| POST   | `/api/proxies/unassign`   | `{ profileId }`                                        | Remove proxy from profile (returns to pool) |

**type:** `http` | `https` | `socks4` | `socks5`

---

## Scripts `/api/scripts`

| Method | Path               | Body                                                                                      | Description                             |
| ------ | ------------------ | ----------------------------------------------------------------------------------------- | --------------------------------------- |
| GET    | `/api/scripts`     | —                                                                                         | List all scripts                        |
| POST   | `/api/scripts`     | `{ name, content, description?, headless?, cronSchedule?, cronEnabled?, cronProfileId? }` | Create script (201)                     |
| PUT    | `/api/scripts/:id` | same fields (all optional)                                                                | Update script                           |
| DELETE | `/api/scripts/:id` | —                                                                                         | Delete script and cancel cron job (204) |

**cronSchedule:** standard cron expression e.g. `"0 * * * *"` (hourly)

Scripts have access to: `page` (Playwright Page), `context` (BrowserContext), `profileId`, `log(msg)`

---

## Fingerprints `/api/fingerprints`

| Method | Path                                    | Body                                  | Description                              |
| ------ | --------------------------------------- | ------------------------------------- | ---------------------------------------- |
| POST   | `/api/fingerprints/preview`             | `{ os?, browser?, device?, locale? }` | Generate fingerprint preview (not saved) |
| POST   | `/api/fingerprints/:profileId/generate` | `{ os?, browser?, device?, locale? }` | Generate + save fingerprint for profile  |
| PUT    | `/api/fingerprints/:profileId`          | FingerprintConfig object              | Save manually-edited fingerprint config  |

**os:** `windows` | `macos` | `linux`
**browser:** `chrome` | `firefox` | `edge`
**device:** `desktop` | `mobile`

---

## Inline Script Context

### REST `/run-script` endpoint globals

```js
// page — Playwright Page object for the profile's active browser tab
await page.goto("https://example.com");
const title = await page.title();

// context — Playwright BrowserContext (all pages in session)
const cookies = await context.cookies();

// profileId — string ID of the current profile
log(`Running on profile: ${profileId}`);

// log(msg) — append message to script output (returned in { output })
log("Step 1 complete");
```

> **Note:** Task-based scripts (via `/api/tasks`) receive `cdp` (CDPAdapter) and `require` instead of `context`. Use the Script Writing Guide for task script globals.

---

## Workflow Example

```bash
PORT=3000

# 1. Create a profile
PROFILE=$(curl -s -X POST http://localhost:$PORT/api/profiles \
  -H 'Content-Type: application/json' \
  -d '{"name":"my-profile"}')
PROFILE_ID=$(echo $PROFILE | jq -r '.id')

# 2. Launch browser
curl -X POST http://localhost:$PORT/api/browsers/$PROFILE_ID/launch \
  -H 'Content-Type: application/json' \
  -d '{"headless": true}'

# 3. Navigate and extract
curl -X POST http://localhost:$PORT/api/browsers/$PROFILE_ID/actions/navigate \
  -H 'Content-Type: application/json' \
  -d '{"url":"https://example.com"}'

curl -X POST http://localhost:$PORT/api/browsers/$PROFILE_ID/actions/get-text \
  -H 'Content-Type: application/json' \
  -d '{"selector":"h1"}'

# 4. Close browser
curl -X POST http://localhost:$PORT/api/browsers/$PROFILE_ID/close
```
