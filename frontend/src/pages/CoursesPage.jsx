import { useEffect, useState } from 'react'
import { getCourses } from '../api.js'
import CourseCard from '../components/CourseCard.jsx'

export default function CoursesPage() {
  const [courses, setCourses] = useState([])
  const [error, setError] = useState('')

  useEffect(() => {
    getCourses().then(setCourses).catch((e) => setError(e.message))
  }, [])

  return (
    <>
      <h1>Courses</h1>
      {error && <p className="message error">{error}</p>}
      <div className="grid">
        {courses.map((c) => (
          <CourseCard key={c.id} course={c} />
        ))}
      </div>
    </>
  )
}
