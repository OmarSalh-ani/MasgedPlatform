import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Plus } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useTips } from '@/hooks/useTips'
import { TipsTable } from '@/pages/tips/TipsTable'
import { DeleteTipDialog } from '@/pages/tips/dialogs/DeleteTipDialog'

export function TipsPage() {
  const { query, deleteMutation } = useTips()
  const [deleteId, setDeleteId] = useState<number | null>(null)

  const handleDeleteConfirm = () => {
    if (deleteId === null) return
    deleteMutation.mutate(deleteId, {
      onSettled: () => setDeleteId(null),
    })
  }

  if (query.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (query.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل قائمة النصائح والإرشادات. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  const items = query.data ?? []

  return (
    <div>
      <PageHeader
        title="نصائح وأرشادات"
        description="إدارة النصائح والإرشادات المعروضة في الصفحة الرئيسية"
        actions={
          <Link
            to="/tips/new"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <Plus className="size-4" />
            إضافة نصيحة
          </Link>
        }
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف النصيحة. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <TipsTable items={items} emptyMessage="لا توجد نصائح" onDelete={setDeleteId} />

      <DeleteTipDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
