import { useRef, useState } from 'react'
import { PHOTO_ALLOWED_TYPES, PHOTO_MAX_BYTES } from '@/pages/parents-followup/parentsFollowup.constants'
import {
  PHOTO_ASPECT_ERROR_MESSAGE,
  validatePhotoAspectRatio,
} from '@/pages/parents-followup/parentsFollowupPhotoValidation'

type Props = {
  previewUrl: string | null
  onFileSelected: (file: File) => void
  onValidationError: (message: string) => void
}

export function ParentsFollowupPhotoBox({ previewUrl, onFileSelected, onValidationError }: Props) {
  const inputRef = useRef<HTMLInputElement>(null)
  const [isChecking, setIsChecking] = useState(false)

  const handleChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0]
    if (!file) return

    if (!PHOTO_ALLOWED_TYPES.includes(file.type)) {
      onValidationError('نوع الملف غير مدعوم. يرجى اختيار صورة بصيغة JPG أو JPEG أو PNG')
      event.target.value = ''
      return
    }

    if (file.size > PHOTO_MAX_BYTES) {
      onValidationError('حجم الملف كبير جداً. يرجى اختيار صورة أقل من 1 ميجابايت')
      event.target.value = ''
      return
    }

    setIsChecking(true)
    try {
      const isValidAspect = await validatePhotoAspectRatio(file)
      if (!isValidAspect) {
        onValidationError(PHOTO_ASPECT_ERROR_MESSAGE)
        event.target.value = ''
        return
      }

      onValidationError('')
      onFileSelected(file)
    } finally {
      setIsChecking(false)
    }
  }

  return (
    <div
      className={`photo-box${previewUrl ? ' has-image' : ''}`}
      onClick={() => !isChecking && inputRef.current?.click()}
      onKeyDown={(e) => e.key === 'Enter' && !isChecking && inputRef.current?.click()}
      role="button"
      tabIndex={0}
      aria-busy={isChecking}
    >
      <input
        ref={inputRef}
        type="file"
        accept=".jpg,.jpeg,.png"
        className="hidden"
        disabled={isChecking}
        onChange={(event) => void handleChange(event)}
      />
      {previewUrl ? (
        <img src={previewUrl} alt="صورة الطالب" className="photo-preview" />
      ) : (
        <div className="photo-upload-text">
          <div>صورة شخصية</div>
          <div>٤/٦</div>
          <div className="photo-upload-hint">{isChecking ? 'جاري التحقق...' : 'اضغط للرفع'}</div>
        </div>
      )}
    </div>
  )
}
