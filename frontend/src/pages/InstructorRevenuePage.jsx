import { useEffect, useState } from 'react'
import { getRevenue } from '../api.js'

export default function InstructorRevenuePage() {
  const [rows, setRows] = useState([])
  const [error, setError] = useState('')

  useEffect(() => {
    getRevenue().then(setRows).catch((e) => setError(e.message))
  }, [])

  return (
    <>
      <h1>Instructor Revenue</h1>
      {error && <p className="message error">{error}</p>}
      <table className="panel data-table">
        <thead>
          <tr><th>Instructor</th><th>Gross sales</th><th>Refunds</th><th>Instructor share</th></tr>
        </thead>
        <tbody>
          {rows.map((r) => (
            <tr key={r.instructor}>
              <td>{r.instructor}</td>
              <td>{r.grossSales.toFixed(2)}</td>
              <td>{r.refunds.toFixed(2)}</td>
              <td>{r.instructorShare.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  )
}
