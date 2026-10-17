import { NavLink } from 'react-router-dom'

export default function NavBar() {
  return (
    <nav className="navbar">
      <div className="container navbar-inner">
        <span className="brand">LearnHub</span>
        <NavLink to="/" end>Courses</NavLink>
        <NavLink to="/my-courses">My Courses</NavLink>
        <NavLink to="/progress">My Progress</NavLink>
      </div>
    </nav>
  )
}
