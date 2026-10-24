import { test, expect } from '@playwright/test'
import { REFUND_COURSE_ID, ageOrder, buyCourse, completeLessons, dbProgressFor, openAs, uniqueName } from './helpers.js'

// Every test makes its own student and order, so no seeded data is changed.
const COURSE_TITLE = 'Deploying Apps with Containers'

function orderCard(page, orderId) {
  return page.locator('.panel').filter({ hasText: `Order ${orderId}` })
}

test('a compliant order is refunded and the course leaves My Progress', async ({ page, request }) => {
  const name = uniqueName('E2E Refund')
  const order = await buyCourse(request, name, REFUND_COURSE_ID)

  await openAs(page, '/orders', name)
  const card = orderCard(page, order.id)
  await expect(card.locator('.order-status')).toHaveText('Paid')
  await card.getByRole('button', { name: 'Refund', exact: true }).click()

  await expect(card.locator('.order-status')).toHaveText('Refunded')
  await expect(card.getByRole('button', { name: 'Refund', exact: true })).toHaveCount(0)

  await openAs(page, '/progress', name)
  await expect(page.getByText('No courses yet. Enroll in a course first.')).toBeVisible()
  await expect(page.getByRole('link', { name: COURSE_TITLE, exact: true })).toHaveCount(0)
})

test('a refund is refused when progress is 30% or more', async ({ page, request }) => {
  const name = uniqueName('E2E Refund')
  const order = await buyCourse(request, name, REFUND_COURSE_ID)
  // 3 of 10 lessons is exactly 30%, which the policy refuses (it refuses at 30% itself).
  await completeLessons(request, name, REFUND_COURSE_ID, 3)
  expect(dbProgressFor(name).map((r) => r.percent)).toEqual([30])

  await openAs(page, '/orders', name)
  const card = orderCard(page, order.id)
  await card.getByRole('button', { name: 'Refund', exact: true }).click()

  await expect(card.getByText('Refunds need progress under 30%')).toBeVisible()
  await expect(card.locator('.order-status')).toHaveText('Paid')
})

test('a refund is refused after 7 days', async ({ page, request }) => {
  const name = uniqueName('E2E Refund')
  const order = await buyCourse(request, name, REFUND_COURSE_ID)
  // The API clock is pinned, so age the order in MySQL to 8 days before the pinned "now".
  ageOrder(order.id)

  await openAs(page, '/orders', name)
  const card = orderCard(page, order.id)
  await card.getByRole('button', { name: 'Refund', exact: true }).click()

  await expect(card.getByText('within 7 days')).toBeVisible()
  await expect(card.locator('.order-status')).toHaveText('Paid')
})
