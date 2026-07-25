import { useRef, useState } from 'react'
import { Trash2, Upload } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { resolveImageUrl } from '@/lib/resolveImageUrl'

interface TeacherImageUploadFieldProps {
  currentImageUrl: string | null
  onFileChange: (file: File | undefined) => void
  onRemoveImage: () => void
}

export function TeacherImageUploadField({
  currentImageUrl,
  onFileChange,
  onRemoveImage,
}: TeacherImageUploadFieldProps) {
  const inputRef = useRef<HTMLInputElement>(null)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const displayUrl = previewUrl ?? (currentImageUrl ? resolveImageUrl(currentImageUrl) : null)

  const handleFileChange = (file?: File) => {
    onFileChange(file)
    if (previewUrl) URL.revokeObjectURL(previewUrl)
    setPreviewUrl(file ? URL.createObjectURL(file) : null)
  }

  const handleRemove = () => {
    handleFileChange(undefined)
    if (inputRef.current) inputRef.current.value = ''
    onRemoveImage()
  }

  return (
    <div className="rounded-xl border-2 border-dashed border-slate-200 bg-slate-50 p-6 text-center">
      {displayUrl && (
        <img
          src={displayUrl}
          alt="صورة المعلم"
          className="mx-auto mb-4 max-h-48 max-w-full rounded-lg border object-cover"
        />
      )}
      {!displayUrl && (
        <div className="mb-4 flex flex-col items-center gap-2 text-slate-500">
          <Upload className="size-10 text-[var(--color-primary)]" />
          <p>اضغط هنا لاختيار صورة أو اسحب الصورة هنا</p>
        </div>
      )}
      <input
        ref={inputRef}
        type="file"
        accept="image/*"
        className="mx-auto block text-sm"
        onChange={(e) => handleFileChange(e.target.files?.[0])}
      />
      {displayUrl && (
        <Button type="button" variant="destructive" className="mt-4" onClick={handleRemove}>
          <Trash2 className="size-4" />
          إزالة الصورة
        </Button>
      )}
    </div>
  )
}
