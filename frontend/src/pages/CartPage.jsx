import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { removeFromCart, previewCheckout, checkout, formatPrice, getSavedName, saveName } from '../api.js'

export default function CartPage() {
  const [name, setName] = useState(getSavedName())
  const [coupon, setCoupon] = useState('')
  const [preview, setPreview] = useState(null)
  const [message, setMessage] = useState(null)
  const [error, setError] = useState('')

  async function load(studentName, code) {
    if (!studentName.trim()) return
    try {
      setPreview(await previewCheckout(studentName.trim(), code.trim()))
      setError('')
      saveName(studentName.trim())
    } catch (err) {
      setError(err.message)
    }
  }

  // Refresh the breakdown when the name or coupon changes.
  useEffect(() => {
    const t = setTimeout(() => load(name, coupon), 300)
    return () => clearTimeout(t)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [name, coupon])

  async function handleRemove(courseId) {
    try {
      await removeFromCart(name.trim(), courseId)
      setMessage(null)
      await load(name, coupon)
    } catch (err) {
      setError(err.message)
    }
  }

  async function handlePay() {
    try {
      const order = await checkout(name.trim(), coupon.trim())
      setMessage({ type: 'ok', text: `Order #${order.id} paid. Total ${formatPrice(order.total)}.` })
      setCoupon('')
      setError('')
      await load(name, '')
    } catch (err) {
      setMessage(null)
      setError(err.message)
    }
  }

  const lines = preview?.lines || []

  return (
    <>
      <h1>Cart</h1>
      <div className="row">
        <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Your name" />
        <input value={coupon} onChange={(e) => setCoupon(e.target.value)} placeholder="Coupon code" />
      </div>
      {error && <p className="message error">{error}</p>}
      {message && <p className={`message ${message.type}`}>{message.text}</p>}
      {preview && lines.length === 0 && (
        <p className="muted">Your cart is empty. Pick a course from the <Link to="/">Courses page</Link>.</p>
      )}
      {lines.map((l) => (
        <div key={l.courseId} className="panel">
          <div className="row">
            <Link to={`/courses/${l.courseId}`}>{l.title}</Link>
            <span className="muted">
              {formatPrice(l.price)}
              {l.bundleDiscount > 0 && ` · bundle -${l.bundleDiscount.toFixed(2)}`}
              {l.couponDiscount > 0 && ` · coupon -${l.couponDiscount.toFixed(2)}`}
            </span>
            <strong>{l.final.toFixed(2)}</strong>
            <button type="button" onClick={() => handleRemove(l.courseId)}>Remove</button>
          </div>
        </div>
      ))}
      {lines.length > 0 && (
        <div className="panel">
          <p>Subtotal: {preview.subtotal.toFixed(2)}</p>
          <p>Bundle discount: -{preview.bundleDiscount.toFixed(2)}</p>
          <p>Coupon discount: -{preview.couponDiscount.toFixed(2)}</p>
          {preview.couponMessage && <p className="muted">{preview.couponMessage}</p>}
          <h2>Total: {preview.total.toFixed(2)}</h2>
          <button type="button" onClick={handlePay}>Pay</button>
        </div>
      )}
    </>
  )
}
