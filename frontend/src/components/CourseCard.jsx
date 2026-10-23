import { useState } from 'react'
import { Link } from 'react-router-dom'
import { addToCart, formatPrice, getSavedName } from '../api.js'

export default function CourseCard({ course }) {
  const [message, setMessage] = useState(null) // { type, text }

  async function handleAdd(e) {
    e.preventDefault()
    e.stopPropagation()
    const name = getSavedName() || window.prompt('Your name?') || ''
    if (!name.trim()) return
    try {
      await addToCart(name.trim(), course.id)
      setMessage({ type: 'ok', text: 'Added to cart.' })
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    }
  }

  return (
    <Link to={`/courses/${course.id}`} className="card">
      <span className="tag">{course.category}</span>
      <h3>{course.title}</h3>
      <p>{course.shortDescription}</p>
      <div className="meta">
        <span>{course.instructor}</span>
        <span>{course.level} · {course.lessonCount} lessons</span>
      </div>
      <div className="row">
        <strong>{formatPrice(course.price)}</strong>
        {course.price > 0 && <button type="button" onClick={handleAdd}>Add to cart</button>}
      </div>
      {message && <p className={`message ${message.type}`}>{message.text}</p>}
    </Link>
  )
}
