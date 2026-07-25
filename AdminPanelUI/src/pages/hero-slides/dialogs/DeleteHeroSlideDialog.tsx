import { Button } from '@/components/ui/button'

interface DeleteHeroSlideDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: () => void
  isPending: boolean
}

export function DeleteHeroSlideDialog({
  open,
  onOpenChange,
  onConfirm,
  isPending,
}: DeleteHeroSlideDialogProps) {
  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-lg">
        <h2 className="mb-2 text-lg font-bold">تأكيد الحذف</h2>
        <p className="mb-6 text-slate-600">حذف هذه الصورة؟</p>
        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
            إلغاء
          </Button>
          <Button
            type="button"
            className="bg-red-600 hover:bg-red-700"
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
