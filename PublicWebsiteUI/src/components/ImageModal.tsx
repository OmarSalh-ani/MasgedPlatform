import { useCallback, useEffect, useState } from 'react'

export function ImageModal() {
  const [imageUrl, setImageUrl] = useState('')
  const [open, setOpen] = useState(false)

  const close = useCallback(() => {
    setOpen(false)
    setImageUrl('')
  }, [])

  useEffect(() => {
    const handler = (event: Event) => {
      const custom = event as CustomEvent<string>
      setImageUrl(custom.detail)
      setOpen(true)
    }
    window.addEventListener('open-image-modal', handler as EventListener)
    return () => window.removeEventListener('open-image-modal', handler as EventListener)
  }, [])

  useEffect(() => {
    if (!open) return
    const onKey = (e: KeyboardEvent) => e.key === 'Escape' && close()
    document.body.style.overflow = 'hidden'
    window.addEventListener('keydown', onKey)
    return () => {
      document.body.style.overflow = ''
      window.removeEventListener('keydown', onKey)
    }
  }, [open, close])

  if (!open) return null

  return (
    <div
      className={`lightbox lightbox--open`}
      role="dialog"
      aria-modal="true"
      aria-label="عرض الصورة"
      onClick={close}
    >
      <button type="button" className="lightbox-close" aria-label="إغلاق" onClick={close}>
        <i className="fas fa-times" />
      </button>
      <img
        src={imageUrl}
        alt=""
        onClick={(e) => e.stopPropagation()}
      />
    </div>
  )
}

export function openImageModal(url: string) {
  window.dispatchEvent(new CustomEvent('open-image-modal', { detail: url }))
}
