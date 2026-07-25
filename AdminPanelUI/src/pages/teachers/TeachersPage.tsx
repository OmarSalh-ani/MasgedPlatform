import { useMemo, useState } from 'react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { canModify } from '@/lib/authStorage'
import { useTeachers } from '@/hooks/useTeachers'
import { DeleteTeacherDialog } from '@/pages/teachers/dialogs/DeleteTeacherDialog'
import { TeachersFilters } from '@/pages/teachers/TeachersFilters'
import { TeachersTable } from '@/pages/teachers/TeachersTable'

export function TeachersPage() {
  const { query, deleteMutation, exportMutation } = useTeachers()
  const [search, setSearch] = useState('')
  const [deleteTarget, setDeleteTarget] = useState<{ id: number; name: string } | null>(null)
  const userCanModify = canModify()

  const items = query.data ?? []
  const filteredItems = useMemo(() => {
    const term = search.trim().toLowerCase()
    if (!term) return items

    return items.filter(
      (item) =>
        item.name.toLowerCase().includes(term) ||
        String(item.id).includes(term) ||
        (item.mobile ?? '').toLowerCase().includes(term) ||
        item.email.toLowerCase().includes(term),
    )
  }, [items, search])

  const handleDeleteConfirm = () => {
    if (!deleteTarget) return
    deleteMutation.mutate(deleteTarget.id, {
      onSettled: () => setDeleteTarget(null),
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
        تعذر تحميل قائمة المعلمين. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  const hasSearch = search.trim().length > 0
  const emptyMessage = hasSearch
    ? 'لا توجد نتائج مطابقة للبحث'
    : 'لا يوجد معلمين'

  return (
    <div>
      <PageHeader title="قائمة المعلمين" description="إدارة بيانات المعلمين والحلقات" />

      <TeachersFilters
        search={search}
        onSearchChange={setSearch}
        canModify={userCanModify}
        onExport={() => exportMutation.mutate()}
        isExporting={exportMutation.isPending}
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف المعلم. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {exportMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر تصدير المعلمين. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <TeachersTable
        items={filteredItems}
        emptyMessage={emptyMessage}
        canModify={userCanModify}
        onDelete={(id, name) => setDeleteTarget({ id, name })}
      />

      <DeleteTeacherDialog
        open={deleteTarget !== null}
        teacherName={deleteTarget?.name ?? null}
        onOpenChange={(open) => !open && setDeleteTarget(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
