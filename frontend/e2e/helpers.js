import { execFileSync } from 'node:child_process'
import { expect } from '@playwright/test'

// Local dev MySQL. The password can be overridden with LEARNHUB_DB_PASSWORD.
const DB_PASSWORD = process.env.LEARNHUB_DB_PASSWORD || 'Root123$'

// Course 6 (Deploying Apps with Containers, $60) has exactly 10 lessons, so 3 completed lessons is exactly 30%.
export const REFUND_COURSE_ID = 6

let counter = 0
// Unique per run and per test, so reruns never collide with earlier students.
export function uniqueName(prefix) {
  counter += 1
  return `${prefix} ${Date.now()}-${counter}`
}

// Runs a SQL statement through the mysql CLI and returns the tab-separated output rows.
// Arguments are passed as an array, so no shell string is built.
export function runSql(sql) {
  const out = execFileSync(
    'mysql',
    ['-h127.0.0.1', '-P3306', '-uroot', '-N', '-B', 'learnhub', '-e', sql],
    { env: { ...process.env, MYSQL_PWD: DB_PASSWORD }, encoding: 'utf8' },
  )
  return out
    .split(/\r?\n/)
    .filter((line) => line.length > 0)
    .map((line) => line.split('\t'))
}

function sqlString(value) {
  const escaped = String(value).split('\\').join('\\\\').split("'").join("''")
  return `'${escaped}'`
}

// C# Math.Round uses banker's rounding (MidpointRounding.ToEven) by default. JS Math.round does not.
export function roundHalfEven(x) {
  const floor = Math.floor(x)
  const diff = x - floor
  if (diff > 0.5) return floor + 1
  if (diff < 0.5) return floor
  return floor % 2 === 0 ? floor : floor + 1
}

export function expectedPercent(completed, total) {
  return total === 0 ? 0 : roundHalfEven((100 * completed) / total)
}

// Reads each enrolment for a seeded student straight from the database, in the same order the API uses.
export function dbProgressFor(studentName) {
  const rows = runSql(`
    SELECT c.title,
           (SELECT COUNT(*) FROM lesson_progress p WHERE p.student_id = e.student_id AND p.course_id = e.course_id),
           (SELECT COUNT(*) FROM lessons l WHERE l.course_id = e.course_id)
    FROM enrollments e
    JOIN students s ON s.id = e.student_id
    JOIN courses c ON c.id = e.course_id
    WHERE s.full_name = ${sqlString(studentName)}
    ORDER BY e.id`)
  return rows.map(([title, completed, total]) => {
    const done = Number(completed)
    const all = Number(total)
    return { title, completed: done, total: all, percent: expectedPercent(done, all) }
  })
}

// Pushes an order's created_at back 8 days so the 7-day refund window has passed.
// The API clock is pinned, so this is the only way to reach that case.
export function ageOrder(orderId) {
  if (!Number.isInteger(orderId)) throw new Error(`Bad order id: ${orderId}`)
  runSql(`UPDATE orders SET created_at = DATE_SUB(created_at, INTERVAL 8 DAY), updated_at = NOW() WHERE id = ${orderId}`)
}

// Buys one course for a student through the API (add to cart, then checkout). Returns the order.
export async function buyCourse(request, studentName, courseId) {
  const added = await request.post('/api/cart', { data: { studentName, courseId } })
  expect(added.ok(), `add to cart: ${added.status()}`).toBeTruthy()
  const checkedOut = await request.post('/api/checkout', { data: { studentName, couponCode: null } })
  expect(checkedOut.ok(), `checkout: ${checkedOut.status()}`).toBeTruthy()
  return checkedOut.json()
}

// Marks the first `count` lessons of a course complete for a student through the API.
export async function completeLessons(request, studentName, courseId, count) {
  const course = await (await request.get(`/api/courses/${courseId}`)).json()
  for (const lesson of course.lessons.slice(0, count)) {
    const res = await request.post('/api/progress', { data: { studentName, courseId, lessonId: lesson.id } })
    expect(res.ok(), `mark lesson ${lesson.id}: ${res.status()}`).toBeTruthy()
  }
}

// Opens a page and types the student name into the page's own name box, as a student would.
export async function openAs(page, path, studentName) {
  await page.goto(path)
  await page.getByPlaceholder('Your name').fill(studentName)
  await page.getByRole('button', { name: /^Show my / }).click()
}
