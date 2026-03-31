// @ts-check
const { test, expect } = require('@playwright/test');

test.describe('AKS Three Tier App E2E Tests', () => {

  test('frontend loads and displays the home page', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('h1')).toHaveText('Welcome to .NET');
  });

  test('frontend displays FE Infos section with environment data', async ({ page }) => {
    await page.goto('/');

    // Wait for Blazor WebAssembly to load and render data
    await expect(page.locator('h2').first()).toHaveText('FE Infos', { timeout: 30000 });

    // Wait for the data to load (table should appear after API call completes)
    const feTable = page.locator('table').first();
    await expect(feTable).toBeVisible({ timeout: 30000 });

    // Verify FE infos contain .NET version
    await expect(feTable.locator('td', { hasText: '.NET version' })).toBeVisible();
    await expect(feTable.locator('td', { hasText: '.NET 10' })).toBeVisible();

    // Verify other FE info fields
    await expect(feTable.locator('td', { hasText: 'Operating system' })).toBeVisible();
    await expect(feTable.locator('td', { hasText: 'Containerized' })).toBeVisible();
    await expect(feTable.locator('td', { hasText: 'Host name' })).toBeVisible();
  });

  test('frontend displays BE Infos section from backend API', async ({ page }) => {
    await page.goto('/');

    // Wait for BE Infos section to render (proves frontend called backend)
    const beHeading = page.locator('h2', { hasText: 'BE Infos' });
    await expect(beHeading).toBeVisible({ timeout: 30000 });

    // The BE table is the second table on the page
    const beTables = page.locator('table');
    const beTable = beTables.nth(1);
    await expect(beTable).toBeVisible({ timeout: 30000 });

    // Verify BE infos contain .NET version from the API server
    await expect(beTable.locator('td', { hasText: '.NET version' })).toBeVisible();
    await expect(beTable.locator('td', { hasText: '.NET 10' })).toBeVisible();

    // Verify containerized field shows the API is running in a container
    await expect(beTable.locator('td', { hasText: 'Containerized' })).toBeVisible();
    await expect(beTable.locator('td', { hasText: 'true' })).toBeVisible();
  });

  test('backend API returns data from Redis database', async ({ page }) => {
    await page.goto('/');

    // Wait for Database Infos section (proves backend called Redis)
    const dbHeading = page.locator('h2', { hasText: 'Database Infos' });
    await expect(dbHeading).toBeVisible({ timeout: 30000 });

    // Verify the DB entities table is rendered
    const dbTable = page.locator('table').nth(2);
    await expect(dbTable).toBeVisible({ timeout: 30000 });

    // Should show at least one HostName entry from Redis
    await expect(dbTable.locator('td', { hasText: 'HostName' }).first()).toBeVisible();

    // Should show at least one UTC Date entry from Redis
    await expect(dbTable.locator('td', { hasText: 'UTC Date' }).first()).toBeVisible();
  });

  test('full three-tier flow: frontend → backend → Redis', async ({ page }) => {
    await page.goto('/');

    // Wait for all sections to load (proves complete three-tier flow)
    await expect(page.locator('h2', { hasText: 'FE Infos' })).toBeVisible({ timeout: 30000 });
    await expect(page.locator('h2', { hasText: 'BE Infos' })).toBeVisible({ timeout: 30000 });
    await expect(page.locator('h2', { hasText: 'Database Infos' })).toBeVisible({ timeout: 30000 });

    // Verify we have 3 tables (FE, BE, DB)
    const tables = page.locator('table');
    await expect(tables).toHaveCount(3, { timeout: 30000 });
  });

  test('counter page works', async ({ page }) => {
    await page.goto('/counter');

    await expect(page.locator('h1')).toHaveText('Counter');
    await expect(page.locator('p[role="status"]')).toHaveText('Current count: 0');

    // Click the increment button
    await page.locator('button', { hasText: 'Click me' }).click();
    await expect(page.locator('p[role="status"]')).toHaveText('Current count: 1');

    // Click again
    await page.locator('button', { hasText: 'Click me' }).click();
    await expect(page.locator('p[role="status"]')).toHaveText('Current count: 2');
  });

  test('navigation between pages works', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('h1')).toHaveText('Welcome to .NET');

    // Navigate to counter
    await page.locator('a', { hasText: 'Counter' }).click();
    await expect(page.locator('h1')).toHaveText('Counter');

    // Navigate back to home
    await page.locator('a', { hasText: 'Home' }).click();
    await expect(page.locator('h1')).toHaveText('Welcome to .NET');
  });

  test('API endpoint returns valid JSON', async ({ request }) => {
    const response = await request.get('http://localhost:5101/api/Infos/GetEnvironmentInfos');
    expect(response.ok()).toBeTruthy();

    const data = await response.json();
    expect(data.frameworkDescription).toContain('.NET 10');
    expect(data.hostName).toBeTruthy();
    expect(data.containerized).toBe('true');
    expect(data.dbEntities).toBeDefined();
    expect(data.dbEntities.length).toBeGreaterThan(0);
  });

  test('frontend Infos endpoint returns complete data', async ({ request }) => {
    const response = await request.get('http://localhost:5100/Infos');
    expect(response.ok()).toBeTruthy();

    const data = await response.json();

    // Frontend info should be present
    expect(data.frontendInfos).toBeDefined();
    expect(data.frontendInfos.frameworkDescription).toContain('.NET 10');
    expect(data.frontendInfos.containerized).toBe('true');

    // API info should be present (proves frontend called backend)
    expect(data.apiInfos).toBeDefined();
    expect(data.apiInfos.frameworkDescription).toContain('.NET 10');

    // DB entities should be present (proves backend called Redis)
    expect(data.apiInfos.dbEntities).toBeDefined();
    expect(data.apiInfos.dbEntities.length).toBeGreaterThan(0);
    expect(data.apiInfos.dbEntities[0].hostName).toBeTruthy();
  });
});
