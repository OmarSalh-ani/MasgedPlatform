import { Trash2 } from 'lucide-react'
import { useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { useCircles } from '@/hooks/useCircles'
import { getAdminSession, isAdmin } from '@/lib/authStorage'
import { CirclesFilters } from '@/pages/circles/CirclesFilters'
import { CirclesTable } from '@/pages/circles/CirclesTable'
import { DeleteCircleDialog } from '@/pages/circles/dialogs/DeleteCircleDialog'
import { DeleteCirclePlansDialog } from '@/pages/circles/dialogs/DeleteCirclePlansDialog'
import { getCirclesEmptyMessage } from '@/types/circle'

export function CirclesPage() {
  const [searchParams] = useSearchParams()
  const teacherParam = searchParams.get('teacher')
  const teacherId = teacherParam ? Number(teacherParam) : undefined
  const { query, deleteMutation, deletePlansMutation, exportMutation } = useCircles(
    Number.isFinite(teacherId) ? teacherId : undefined,
  )
  const [search, setSearch] = useState('')
  const [deleteId, setDeleteId] = useState<number | null>(null)
  const [selectedIds, setSelectedIds] = useState<number[]>([])
  const [deletePlansOpen, setDeletePlansOpen] = useState(false)
  const canModify = getAdminSession()?.isViewOnly !== true
  const userIsAdmin = isAdmin()

  const items = query.data ?? []
  const filteredItems = useMemo(() => {
    const term = search.trim().toLowerCase()
    if (!term) return items
    return items.filter(
      (item) =>
        item.name.toLowerCase().includes(term) ||
        item.teacherName.toLowerCase().includes(term),
    )
  }, [items, search])

  const handleToggleSelect = (id: number, checked: boolean) => {
    setSelectedIds((prev) => (checked ? [...prev, id] : prev.filter((x) => x !== id)))
  }

  const handleDeleteConfirm = () => {
    if (deleteId === null) return
    deleteMutation.mutate(deleteId, {
      onSettled: () => setDeleteId(null),
    })
  }

  const handleDeletePlansConfirm = () => {
    if (selectedIds.length === 0) return
    deletePlansMutation.mutate(selectedIds, {
      onSuccess: () => {
        setSelectedIds([])
        setDeletePlansOpen(false)
      },
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
        تعذر تحميل قائمة الحلقات. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  const hasSearch = search.trim().length > 0

  return (
    <div>
      <PageHeader title="قائمة الحلقات" description="إدارة حلقات القرآن الكريم" />

      <CirclesFilters
        search={search}
        onSearchChange={setSearch}
        canModify={canModify}
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف الحلقة. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {deletePlansMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف الخطط. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {exportMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر تصدير الحلقات. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <CirclesTable
        items={filteredItems}
        emptyMessage={getCirclesEmptyMessage(hasSearch)}
        canModify={canModify}
        showSelection={userIsAdmin}
        selectedIds={selectedIds}
        onToggleSelect={handleToggleSelect}
        onDelete={setDeleteId}
        onExport={() => exportMutation.mutate()}
        isExporting={exportMutation.isPending}
        toolbar={
          userIsAdmin ? (
            <Button
              type="button"
              variant="outline"
              className="border-red-200 text-red-600 hover:bg-red-50"
              disabled={selectedIds.length === 0 || deletePlansMutation.isPending}
              onClick={() => setDeletePlansOpen(true)}
            >
              <Trash2 className="size-4" />
              حذف الخطط
            </Button>
          ) : undefined
        }
      />

      <DeleteCircleDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />

      <DeleteCirclePlansDialog
        open={deletePlansOpen}
        onOpenChange={setDeletePlansOpen}
        onConfirm={handleDeletePlansConfirm}
        isPending={deletePlansMutation.isPending}
      />
    </div>
  )
}
