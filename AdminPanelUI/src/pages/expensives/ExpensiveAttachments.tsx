import { Download, File, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { formatDateGeorgian } from '@/lib/utils'
import type { ExpensiveAttachment } from '@/types/expensives'

interface ExpensiveAttachmentsProps {
  attachments: ExpensiveAttachment[]
  readOnly: boolean
  isDeleting: boolean
  onDownload: (fileName: string) => void
  onDelete: (fileName: string) => void
}

export function ExpensiveAttachments({
  attachments,
  readOnly,
  isDeleting,
  onDownload,
  onDelete,
}: ExpensiveAttachmentsProps) {
  if (attachments.length === 0) return null

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      {attachments.map((attachment) => (
        <div
          key={attachment.fileName}
          className="rounded-lg border bg-slate-50 p-4 transition-shadow hover:shadow-md"
        >
          <File className="mb-2 size-8 text-[#7C8738]" />
          <p className="break-words font-semibold">{attachment.fileName}</p>
          <p className="mb-3 text-sm text-slate-500">
            {formatDateGeorgian(attachment.uploadDate)}
          </p>
          <div className="flex flex-wrap gap-2">
            <Button
              type="button"
              variant="outline"
              className="h-8 px-3 text-sm"
              onClick={() => onDownload(attachment.fileName)}
            >
              <Download className="ms-1 size-4" />
              تحميل
            </Button>
            {!readOnly && (
              <Button
                type="button"
                variant="outline"
                className="h-8 border-red-200 px-3 text-sm text-red-600 hover:bg-red-50"
                disabled={isDeleting}
                onClick={() => {
                  if (!window.confirm('هل أنت متأكد من حذف هذا الملف؟')) return
                  onDelete(attachment.fileName)
                }}
              >
                <Trash2 className="ms-1 size-4" />
                حذف
              </Button>
            )}
          </div>
        </div>
      ))}
    </div>
  )
}
