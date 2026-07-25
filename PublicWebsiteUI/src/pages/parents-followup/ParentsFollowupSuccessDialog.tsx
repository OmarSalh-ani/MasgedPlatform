type Props = {
  open: boolean
  onClose: () => void
}

export function ParentsFollowupSuccessDialog({ open, onClose }: Props) {
  if (!open) return null

  return (
    <div className="parents-followup-dialog-overlay">
      <div className="parents-followup-dialog">
        <h3>✅ تم الإرسال بنجاح</h3>
        <p>
          تم إرسال استمارة تسجيل الطالب بنجاح.
          <br />
          سيتم التواصل معكم قريباً.
        </p>
        <button type="button" className="submit-btn" onClick={onClose}>
          حسناً
        </button>
      </div>
    </div>
  )
}
