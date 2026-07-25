import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Plus } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useActivities } from '@/hooks/useActivities'
import { ActivitiesTable } from '@/pages/activities/ActivitiesTable'
import { DeleteActivityDialog } from '@/pages/activities/dialogs/DeleteActivityDialog'

export function ActivitiesPage() {
  const { query, deleteMutation } = useActivities()
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
        تعذر تحميل قائمة الأنشطة. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  const items = query.data ?? []

  return (
    <div>
      <PageHeader
        title="الأنشطة"
        description="إدارة الأنشطة المعروضة في الصفحة الرئيسية"
        actions={
          <Link
            to="/activities/new"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <Plus className="size-4" />
            إضافة نشاط
          </Link>
        }
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف النشاط. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <ActivitiesTable items={items} emptyMessage="لا توجد أنشطة" onDelete={setDeleteId} />

      <DeleteActivityDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
