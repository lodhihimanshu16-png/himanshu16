import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getCourse, enroll, addToCart, formatPrice, markLessonComplete, getProgress, getSavedName, saveName } from '../api.js'

export default function CourseDetailPage() {
  const { id } = useParams()
  const [course, setCourse] = useState(null)
  const [name, setName] = useState(getSavedName())
  const [message, setMessage] = useState(null) // { type: 'ok' | 'error', text }
  const [error, setError] = useState('')
  const [doneIds, setDoneIds] = useState([])

  useEffect(() => {
    getCourse(id).then(setCourse).catch((e) => setError(e.message))
  }, [id])

  // Load which lessons the saved student has finished.
  useEffect(() => {
    if (!getSavedName()) return
    getProgress(getSavedName())
      .then((list) => setDoneIds(list.find((p) => p.courseId === Number(id))?.completedLessonIds || []))
      .catch(() => {})
  }, [id])

  async function handleComplete(lessonId) {
    try {
      await markLessonComplete(name, course.id, lessonId)
      saveName(name.trim())
      setDoneIds((ids) => (ids.includes(lessonId) ? ids : [...ids, lessonId]))
      setMessage({ type: 'ok', text: 'Lesson marked complete.' })
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    }
  }

  async function handleEnroll(e) {
    e.preventDefault()
    try {
      if (course.price > 0) {
        await addToCart(name, course.id)
        setMessage({ type: 'ok', text: 'Added to cart.' })
      } else {
        await enroll(course.id, name)
        setMessage({ type: 'ok', text: 'You are enrolled!' })
      }
      saveName(name.trim())
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    }
  }

  if (error) return <p className="message error">{error}</p>
  if (!course) return <p>Loading...</p>

  return (
    <>
      <Link to="/">&larr; All courses</Link>
      <h1>{course.title}</h1>
      <p>{course.shortDescription}</p>
      <p className="meta">
        <span>{course.instructor}</span>
        <span>{course.category} · {course.level}</span>
        <span className="tag">{formatPrice(course.price)}</span>
      </p>

      <form className="panel" onSubmit={handleEnroll}>
        <h2>{course.price > 0 ? 'Buy' : 'Enroll'}</h2>
        <div className="row">
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Your name"
          />
          <button type="submit">{course.price > 0 ? 'Add to cart' : 'Enroll'}</button>
        </div>
        {message && <p className={`message ${message.type}`}>{message.text}</p>}
      </form>

      <h2>Lessons</h2>
      <ol className="lessons">
        {course.lessons.map((l) => (
          <li key={l.id}>
            <strong>{l.title}</strong>
            <span className="muted"> · {l.durationMinutes} min</span>
            <p>{l.content}</p>
            <div className="row">
              <Link to={`/lessons/${l.id}/quiz`}>Take quiz</Link>
              {doneIds.includes(l.id)
                ? <span className="tag">Completed</span>
                : <button type="button" onClick={() => handleComplete(l.id)}>Mark complete</button>}
            </div>
          </li>
        ))}
      </ol>
    </>
  )
}
