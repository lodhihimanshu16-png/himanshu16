import { Link } from 'react-router-dom'

export default function CourseCard({ course }) {
  return (
    <Link to={`/courses/${course.id}`} className="card">
      <span className="tag">{course.category}</span>
      <h3>{course.title}</h3>
      <p>{course.shortDescription}</p>
      <div className="meta">
        <span>{course.instructor}</span>
        <span>{course.level} · {course.lessonCount} lessons</span>
      </div>
    </Link>
  )
}
