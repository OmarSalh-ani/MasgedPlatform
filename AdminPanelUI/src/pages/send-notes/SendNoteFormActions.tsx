import { Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'

interface SendNoteFormActionsProps {
  isPending: boolean
}

export function SendNoteFormActions({ isPending }: SendNoteFormActionsProps) {
  return (
    <div className="flex justify-end gap-2 border-t border-slate-100 pt-5">
      <Link to="/send-notes">
        <Button type="button" variant="outline">
          إلغاء
        </Button>
      </Link>
      <Button
        type="submit"
        className="bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] hover:opacity-90"
        disabled={isPending}
      >
        {isPending ? 'جاري الحفظ...' : 'حفظ الملاحظة'}
      </Button>
    </div>
  )
}
