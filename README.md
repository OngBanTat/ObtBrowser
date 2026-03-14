# ObtAntiDetectBrowser

An antidetect browser manager built on Electron + Playwright. Manage multiple browser profiles with unique fingerprints, proxy support, and automation scripting — all from a single desktop app.

## Documentation

- Official docs: https://browser.ongbantat.store/docs
- Download: https://browser.ongbantat.store/download
- License: https://browser.ongbantat.store/license

## Features

- **Multi-profile management** — create isolated browser profiles, each with its own user data directory and fingerprint
- **Fingerprint spoofing** — generates realistic Chrome/Firefox fingerprints using real-world browser data; overrides canvas, WebGL, audio, fonts, screen, battery, navigator, and media devices via CDP
- **Bot detection bypass** — uses `playwright` to eliminate the `Runtime.Enable` CDP leak (Cloudflare), plus 15+ additional bot signals
- **Firefox support** — Camoufox engine for Firefox-like fingerprinting
- **Proxy management** — per-profile HTTP/HTTPS/SOCKS4/SOCKS5 proxies; credentials encrypted at rest
- **Automation tasks** — run inline or file-based scripts against any profile; scripts receive `{ page, cdp, profileId, log }` globals
- **REST API** — optional Fastify server with Swagger UI for external automation and integration
- **Real-time logs** — streamed from the main process to the UI
- **Dark/Light theme support** — choose System, Light, or Dark mode in Settings; Monaco editor adapts accordingly

## Quick Start

Reference: https://browser.ongbantat.store/docs

1. Download and install ObtBrowser for your OS.
2. Open Settings and install the browser engine (first run only).
3. Activate your license from Settings -> License.
4. Create a profile from the Profiles tab.
5. Optionally set a proxy in the Proxies tab.
6. Run automation scripts from the Tasks tab.

## SDKs In This Repository

- C#: `sdk/csharp`
- Java: `sdk/java`
- Python: `sdk/python`

For API behavior, setup flow, and integration details, use the official docs as the source of truth:
https://browser.ongbantat.store/docs
