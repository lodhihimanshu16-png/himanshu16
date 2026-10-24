import { defineConfig, devices } from '@playwright/test'

// The DB password is read from the environment and falls back to the local dev value.
const dbPassword = process.env.LEARNHUB_DB_PASSWORD || 'Root123$'

export default defineConfig({
  testDir: './e2e',
  // One worker: the tests share one MySQL database and a single API process.
  workers: 1,
  fullyParallel: false,
  retries: 0,
  timeout: 60_000,
  expect: { timeout: 10_000 },
  reporter: [['list']],
  use: {
    baseURL: 'http://localhost:5173',
    trace: 'retain-on-failure',
  },
  projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
  webServer: [
    {
      command: 'dotnet run --urls http://localhost:5080 --no-launch-profile',
      cwd: '../backend/LearnHub.Api',
      url: 'http://localhost:5080/api/courses',
      reuseExistingServer: true,
      timeout: 120_000,
      env: {
        LEARNHUB_NOW: '2026-10-01T12:00:00Z',
        LEARNHUB_DB: `Server=127.0.0.1;Port=3306;User ID=root;Password=${dbPassword};Database=learnhub`,
      },
    },
    {
      command: 'npm run dev',
      url: 'http://localhost:5173',
      reuseExistingServer: true,
      timeout: 60_000,
    },
  ],
})
