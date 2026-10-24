import { test, expect } from '@playwright/test'
import { dbProgressFor, openAs, runSql, uniqueName } from './helpers.js'

// Seeded students, chosen by querying the database for single or multi-course enrolments
// that cover each case. They are only read, never changed. No refund test uses them.
//   Noah Khan (id 3):  course 5 has 5 of 8 lessons = 62.5%. This is the banker's rounding case, which gives 62, not 63.
//   Ruth Patel (id 4): course 10 has 0 of 6 lessons = 0%.
//   Olga Costa (id 125): course 16 has 10 of 10 lessons = 100%.

async function assertProgressRows(page, studentName) {
  const expected = dbProgressFor(studentName)
  expect(expected.length, 'seeded student should have enrolments').toBeGreaterThan(0)

  await openAs(page, '/progress', studentName)
  const rows = page.locator('.panel')
  await expect(rows).toHaveCount(expected.length)

  for (const row of expected) {
    const panel = page.locator('.panel').filter({
      has: page.getByRole('link', { name: row.title, exact: true }),
    })
    await expect(panel.getByText(`${row.completed} of ${row.total} lessons · ${row.percent}%`)).toBeVisible()
    await expect(panel.locator('.progress-fill')).toHaveAttribute('style', new RegExp(`width:\\s*${row.percent}%`))
  }
}

test('0% progress shows an empty bar and a 0% label', async ({ page }) => {
  await assertProgressRows(page, 'Ruth Patel')
})

test('partial progress rounds like the server (62.5% shows as 62%)', async ({ page }) => {
  await assertProgressRows(page, 'Noah Khan')
  const [[title]] = runSql('SELECT title FROM courses WHERE id = 5')
  const panel = page.locator('.panel').filter({ has: page.getByRole('link', { name: title, exact: true }) })
  await expect(panel.getByText('5 of 8 lessons · 62%')).toBeVisible()
  await expect(panel.locator('.progress-fill')).toHaveAttribute('style', /width:\s*62%/)
})

test('100% progress shows a full bar', async ({ page }) => {
  await assertProgressRows(page, 'Olga Costa')
})

test('the dashboard shows the same bar for a course in progress', async ({ page }) => {
  const [[title]] = runSql('SELECT title FROM courses WHERE id = 5')
  await openAs(page, '/dashboard', 'Noah Khan')
  const link = page.getByRole('link', { name: title, exact: true })
  await expect(link).toBeVisible()
  const row = link.locator('xpath=..')
  await expect(row.getByText('· 5/8 lessons')).toBeVisible()
  await expect(row.locator('.progress-fill')).toHaveAttribute('style', /width:\s*62%/)
})

test('an unknown student sees the empty state', async ({ page }) => {
  await openAs(page, '/progress', uniqueName('E2E Nobody'))
  await expect(page.getByText('No courses yet. Enroll in a course first.')).toBeVisible()
  await expect(page.locator('.progress-fill')).toHaveCount(0)
})
