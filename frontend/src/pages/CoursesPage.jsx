import { useEffect, useState } from 'react'
import { getCourses } from '../api.js'
import CourseCard from '../components/CourseCard.jsx'

export default function CoursesPage() {
  const [courses, setCourses] = useState([])
  const [search, setSearch] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    getCourses(search)
      .then((data) => {
        setCourses(data)
        setError('')
      })
      .catch((e) => setError(e.message))
  }, [search])

  return (
    <>
      <h1>Courses</h1>
      <div className="row">
        <input
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search courses by title"
        />
      </div>
      {error && <p className="message error">{error}</p>}
      {courses.length === 0 && !error && <p className="muted">No courses found.</p>}
      <div className="grid">
        {courses.map((c) => (
          <CourseCard key={c.id} course={c} />
        ))}
      </div>
    </>
  )
}
