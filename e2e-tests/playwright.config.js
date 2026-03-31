// @ts-check
const { defineConfig } = require('@playwright/test');

module.exports = defineConfig({
  testDir: './tests',
  timeout: 60000,
  expect: {
    timeout: 15000,
  },
  fullyParallel: false,
  retries: 1,
  reporter: 'list',
  use: {
    baseURL: 'http://localhost:5100',
    trace: 'on-first-retry',
  },
  projects: [
    {
      name: 'chromium',
      use: { browserName: 'chromium' },
    },
  ],
});
