// All calls to the backend live here.

async function request(url, options) {
  const res = await fetch(url, options)
  const body = await res.json().catch(() => null)
  if (!res.ok) {
    throw new Error(body?.error || `Request failed (${res.status})`)
  }
  return body
}

export const getCourses = () => request('/api/courses')

export const getCourse = (id) => request(`/api/courses/${id}`)

export const enroll = (courseId, studentName) =>
  request('/api/enrollments', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ courseId, studentName }),
  })

export const getEnrollments = (studentName) =>
  request(`/api/enrollments?studentName=${encodeURIComponent(studentName)}`)

// The student name is remembered in the browser.
export const getSavedName = () => localStorage.getItem('studentName') || ''
export const saveName = (name) => localStorage.setItem('studentName', name)
