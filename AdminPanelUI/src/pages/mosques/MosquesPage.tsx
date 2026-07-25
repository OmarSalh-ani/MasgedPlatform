import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Plus } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useMosques } from '@/hooks/useMosques'
import { DeleteMosqueDialog } from '@/pages/mosques/dialogs/DeleteMosqueDialog'
import { MosquesTable } from '@/pages/mosques/MosquesTable'

export function MosquesPage() {
  const { query, deleteMutation } = useMosques()
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
        تعذر تحميل قائمة المساجد. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  const items = query.data ?? []

  return (
    <div>
      <PageHeader
        title="مساجدنا"
        description="إدارة مساجد الجمعية المعروضة في الصفحة الرئيسية"
        actions={
          <Link
            to="/mosques/new"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <Plus className="size-4" />
            إضافة مسجد
          </Link>
        }
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف المسجد. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <MosquesTable items={items} emptyMessage="لا توجد مساجد" onDelete={setDeleteId} />

      <DeleteMosqueDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
