import { Button } from '@/components/ui/button'

interface DeleteEventPageDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: () => void
  isPending: boolean
}

export function DeleteEventPageDialog({
  open,
  onOpenChange,
  onConfirm,
  isPending,
}: DeleteEventPageDialogProps) {
  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-lg" role="dialog">
        <p className="mb-6 text-center text-slate-700">حذف هذه الصفحة وجميع الردود المرتبطة بها؟</p>
        <div className="flex justify-center gap-3">
          <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
            إلغاء
          </Button>
          <Button
            type="button"
            className="bg-red-600 hover:opacity-90"
            disabled={isPending}
            onClick={onConfirm}
          >
            {isPending ? 'جاري الحذف...' : 'حذف'}
          </Button>
        </div>
      </div>
    </div>
  )
}
