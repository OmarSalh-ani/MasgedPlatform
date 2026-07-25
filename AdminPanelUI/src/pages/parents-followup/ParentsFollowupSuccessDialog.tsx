type Props = {
  open: boolean
  onClose: () => void
}

export function ParentsFollowupSuccessDialog({ open, onClose }: Props) {
  if (!open) return null

  return (
    <div className="fixed inset-0 z-[1000] flex items-center justify-center bg-black/50">
      <div className="min-w-[300px] rounded-lg bg-white p-8 text-center shadow-lg">
        <h3 className="mb-4 text-lg font-bold text-[#2e86ab]">✅ تم الإرسال بنجاح</h3>
        <p className="mb-5 leading-relaxed">
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
