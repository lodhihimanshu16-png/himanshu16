// All calls to the backend live here.

async function request(url, options) {
  const res = await fetch(url, options)
  const body = await res.json().catch(() => null)
  if (!res.ok) {
    throw new Error(body?.error || `Request failed (${res.status})`)
  }
  return body
}

export const getCourses = (search = '') =>
  request(`/api/courses?search=${encodeURIComponent(search)}`)

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

export const getQuiz = (lessonId) => request(`/api/lessons/${lessonId}/quiz`)

export const submitQuiz = (lessonId, studentName, answers) =>
  request(`/api/lessons/${lessonId}/quiz/submit`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ studentName, answers }),
  })

export const markLessonComplete = (studentName, courseId, lessonId) =>
  request('/api/progress', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ studentName, courseId, lessonId }),
  })

export const getProgress = (studentName) =>
  request(`/api/progress?studentName=${encodeURIComponent(studentName)}`)
