import { useEffect, useState } from 'react'
import { getOrders, refundOrder, getSavedName, saveName } from '../api.js'

export default function MyOrdersPage() {
  const [name, setName] = useState(getSavedName())
  const [orders, setOrders] = useState([])
  const [searched, setSearched] = useState(false)
  const [refundErrors, setRefundErrors] = useState({}) // orderId -> reason
  const [error, setError] = useState('')

  async function load(studentName) {
    if (!studentName.trim()) return
    try {
      setOrders(await getOrders(studentName.trim()))
      setSearched(true)
      setError('')
      saveName(studentName.trim())
    } catch (err) {
      setError(err.message)
    }
  }

  useEffect(() => {
    load(name)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  async function handleRefund(id) {
    try {
      await refundOrder(id)
      setRefundErrors((r) => ({ ...r, [id]: '' }))
      await load(name)
    } catch (err) {
      setRefundErrors((r) => ({ ...r, [id]: err.message }))
    }
  }

  return (
    <>
      <h1>My Orders</h1>
      <form className="row" onSubmit={(e) => { e.preventDefault(); load(name) }}>
        <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Your name" />
        <button type="submit">Show my orders</button>
      </form>
      {error && <p className="message error">{error}</p>}
      {searched && orders.length === 0 && <p className="muted">No orders yet.</p>}
      {orders.map((o) => (
        <div key={o.id} className="panel">
          <div className="row">
            <strong>Order #{o.id}</strong>
            <span className="tag">{o.status}</span>
            <span className="muted">{new Date(o.createdAt).toLocaleString()}</span>
          </div>
          <ul>
            {o.lines.map((l) => (
              <li key={l.courseId}>{l.title} · {l.final.toFixed(2)}</li>
            ))}
          </ul>
          <p>
            Total: {o.total.toFixed(2)}
            {o.couponCode && <span className="muted"> (coupon {o.couponCode})</span>}
          </p>
          {o.status === 'Paid' && (
            <button type="button" onClick={() => handleRefund(o.id)}>Refund</button>
          )}
          {refundErrors[o.id] && <p className="message error">{refundErrors[o.id]}</p>}
        </div>
      ))}
    </>
  )
}
