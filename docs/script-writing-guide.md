# Script Writing Guide

Scripts are async JavaScript functions executed inside a Playwright browser context tied to a profile. They receive four injected globals — no imports needed.

---

## Globals

| Name | Type | Description |
|------|------|-------------|
| `page` | `Page` | Playwright Page — high-level browser automation API |
| `cdp` | `CDPAdapter` | Raw Chrome DevTools Protocol access |
| `profileId` | `string` | ID of the active profile |
| `log` | `(msg: string) => void` | Streams a message to the task log panel |

---

## `page` — Playwright Page

Scripts run in a top-level `async` scope. All `page.*` calls must be `await`ed.

### Navigation
```js
await page.goto('https://example.com');
await page.goto('https://example.com', { waitUntil: 'networkidle' });
await page.reload();
await page.goBack();
await page.goForward();
```

### Interaction
```js
await page.click('#submit-btn');
await page.fill('#username', 'myuser');
await page.type('#search', 'query', { delay: 50 }); // human-like typing
await page.waitForSelector('.result', { state: 'visible', timeout: 5000 });
await page.waitForNavigation({ waitUntil: 'load' });
await page.waitForTimeout(1000); // ms
```

### Extraction
```js
const title = await page.title();
const url = page.url();
const text = await page.textContent('.price');
const el = await page.$('.card');      // first match (ElementHandle)
const els = await page.$$('.card');    // all matches
const visible = await page.isVisible('#modal');
```

### Evaluate (run JS in page)
```js
const href = await page.evaluate(() => document.location.href);
const count = await page.evaluate(() => document.querySelectorAll('li').length);
```

### Screenshots
```js
await page.screenshot({ path: 'shot.png', fullPage: true });
```

### Cookies
```js
const cookies = await page.cookies();
await page.setCookie({ name: 'token', value: 'abc', domain: 'example.com', path: '/' });
```

---

## `cdp` — Chrome DevTools Protocol

Use `cdp.send(method, params)` for anything not covered by the Playwright API.

```js
// Enable network tracking
await cdp.send('Network.enable');

// Block certain URLs
await cdp.send('Network.setBlockedURLs', { urls: ['*ads*'] });

// Emulate offline
await cdp.send('Network.emulateNetworkConditions', {
  offline: true, latency: 0, downloadThroughput: 0, uploadThroughput: 0,
});

// Read cookies via CDP
const { cookies } = await cdp.send('Network.getAllCookies');
log('Cookie count: ' + cookies.length);

// Take a DOM snapshot
const { data } = await cdp.send('Page.captureScreenshot', { format: 'png' });

// Evaluate JS in page context
const result = await cdp.evaluate('document.title');
```

Full CDP method reference: https://chromedevtools.github.io/devtools-protocol/

---

## `log` — Task Logging

Messages sent to `log()` appear in real-time in the task log panel.

```js
log('Starting login flow');
log('Current URL: ' + page.url());
log('Done — found ' + items.length + ' items');
```

> `log()` is the only output mechanism. `console.log` does not appear in the UI.

---

## Script Structure

Scripts execute as if wrapped in:
```js
(async () => {
  // your code here
})();
```

So top-level `await` works directly:
```js
await page.goto('https://example.com');
log('Title: ' + await page.title());
```

Use `try/catch` to handle errors gracefully:
```js
try {
  await page.goto('https://example.com');
  await page.click('#btn', { timeout: 3000 });
  log('Clicked');
} catch (err) {
  log('Error: ' + err.message);
}
```

---

## Examples

### Login to a site
```js
await page.goto('https://app.example.com/login');
await page.fill('#email', 'user@example.com');
await page.fill('#password', 'secret123');
await page.click('[type=submit]');
await page.waitForNavigation();
log('Logged in. URL: ' + page.url());
```

### Scrape a list
```js
await page.goto('https://example.com/products');
await page.waitForSelector('.product-card');

const names = await page.evaluate(() =>
  Array.from(document.querySelectorAll('.product-card h2')).map(el => el.textContent.trim())
);

for (const name of names) {
  log(name);
}
```

### Block ads then navigate
```js
await cdp.send('Network.enable');
await cdp.send('Network.setBlockedURLs', { urls: ['*doubleclick*', '*googlesyndication*'] });
await page.goto('https://news.ycombinator.com');
log('Page loaded without ads');
```

### Wait for dynamic content
```js
await page.goto('https://example.com/dashboard');
await page.waitForSelector('.data-loaded', { state: 'visible', timeout: 10000 });
const value = await page.textContent('.balance');
log('Balance: ' + value);
```

### Use profileId for conditional logic
```js
const configs = {
  'profile-1': { url: 'https://site-a.com' },
  'profile-2': { url: 'https://site-b.com' },
};

const config = configs[profileId];
if (!config) {
  log('No config for profile: ' + profileId);
} else {
  await page.goto(config.url);
  log('Opened ' + config.url);
}
```

---

## Notes

- Scripts share the profile's persistent browser context — cookies, localStorage, and session data persist across runs.
- `page` is a new page opened on top of the profile context. Previous pages in the context remain open.
- Errors thrown by the script are caught and shown in the task log as `[script] Error: ...`.
- A script that completes without throwing logs `[script] Completed successfully`.
