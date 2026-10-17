import { Routes, Route } from 'react-router-dom'
import NavBar from './components/NavBar.jsx'
import CoursesPage from './pages/CoursesPage.jsx'
import CourseDetailPage from './pages/CourseDetailPage.jsx'
import MyCoursesPage from './pages/MyCoursesPage.jsx'
import QuizPage from './pages/QuizPage.jsx'
import ProgressPage from './pages/ProgressPage.jsx'

export default function App() {
  return (
    <>
      <NavBar />
      <main className="container">
        <Routes>
          <Route path="/" element={<CoursesPage />} />
          <Route path="/courses/:id" element={<CourseDetailPage />} />
          <Route path="/my-courses" element={<MyCoursesPage />} />
          <Route path="/lessons/:lessonId/quiz" element={<QuizPage />} />
          <Route path="/progress" element={<ProgressPage />} />
        </Routes>
      </main>
    </>
  )
}
