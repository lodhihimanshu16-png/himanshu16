---
name: add-react-page
description: Adds a new page to the LearnHub React frontend (page file, route, nav link, api helper, styling). Use when the user asks for a new screen, page, or view in the website, such as a progress page or a reviews page.
---

# Add a React page

Work in `frontend/src`. Copy the style of `pages/MyCoursesPage.jsx`. The code is plain JavaScript.

## Steps

1. Add fetch helpers to `api.js`.
   - Use the existing `request(url, options)` function.
   - Example: `export const getXxx = () => request('/api/xxx')`.
   - Do not call `fetch` inside pages.
2. Create `pages/XxxPage.jsx`.
   - Default export a function component named `XxxPage`.
   - Use `useState` and `useEffect`. Keep an `error` state.
   - Show errors with `<p className="message error">`.
   - Start with an `<h1>` title.
3. Reuse CSS classes from `index.css`: `container`, `panel`, `grid`, `row`, `message`, `muted`, `tag`, `meta`.
   Only add a new class to `index.css` if nothing fits.
4. Add the route in `App.jsx`.
   - Import the page.
   - Add `<Route path="/xxx" element={<XxxPage />} />` inside `<Routes>`.
5. Add a link in `components/NavBar.jsx`: `<NavLink to="/xxx">Xxx</NavLink>`.
   Skip this step for pages reached from another page (such as a detail page).
6. Build: `cd frontend && npm run build`. Fix any errors.
7. Tell the user the new URL, for example http://localhost:5173/xxx.
