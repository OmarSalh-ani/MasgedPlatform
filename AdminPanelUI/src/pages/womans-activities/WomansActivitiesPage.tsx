import { useMemo, useState } from 'react'
import { Pencil, Trash2 } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { canModify, getAdminSession } from '@/lib/authStorage'
import { useWomansActivities } from '@/hooks/useWomansActivities'
import { DeleteWomanActivityDialog } from '@/pages/womans-activities/dialogs/DeleteWomanActivityDialog'
import { SaveWomanActivityDialog } from '@/pages/womans-activities/dialogs/SaveWomanActivityDialog'
import type { SaveWomanActivityFormValues } from '@/pages/womans-activities/saveWomanActivitySchema'
import { WomansActivitiesFilters } from '@/pages/womans-activities/WomansActivitiesFilters'
import { WomanActivityCard } from '@/pages/womans-activities/WomanActivityCard'
import type { WomanActivityListItem } from '@/types/womansActivity'

function getColumns(
  userCanModify: boolean,
  feminineTheme: boolean,
  onEdit: (activity: WomanActivityListItem) => void,
  onDelete: (id: number) => void,
): DataTableColumn<WomanActivityListItem>[] {
  const editBtnClass = feminineTheme
    ? 'bg-gradient-to-br from-pink-400 to-pink-600 hover:opacity-90'
    : 'bg-gradient-to-br from-cyan-600 to-cyan-700 hover:opacity-90'

  return [
    {
      id: 'name',
      header: 'النشاط',
      accessor: 'name',
    },
    {
      id: 'status',
      header: 'الحالة',
      cell: (row) => (
        <span className="inline-flex rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-700">
          {row.isVisible ? 'مرئي' : 'مخفي'}
        </span>
      ),
    },
    {
      id: 'id',
      header: 'رقم النشاط',
      accessor: 'id',
    },
    {
      id: 'actions',
      header: 'إجراءات',
      className: 'text-center',
      cell: (row) =>
        userCanModify ? (
          <div className="flex flex-nowrap justify-center gap-2">
            <Button
              type="button"
              className={`px-3 py-1.5 text-xs text-white ${editBtnClass}`}
              onClick={() => onEdit(row)}
            >
              <Pencil className="size-3.5" />
              تعديل
            </Button>
            <Button
              type="button"
              className="bg-red-600 px-3 py-1.5 text-xs text-white hover:bg-red-700"
              onClick={() => onDelete(row.id)}
            >
              <Trash2 className="size-3.5" />
              حذف
            </Button>
          </div>
        ) : (
          '—'
        ),
    },
  ]
}

export function WomansActivitiesPage() {
  const { query, saveMutation, deleteMutation, exportMutation } = useWomansActivities()
  const [search, setSearch] = useState('')
  const [deleteId, setDeleteId] = useState<number | null>(null)
  const [saveOpen, setSaveOpen] = useState(false)
  const [editingActivity, setEditingActivity] = useState<WomanActivityListItem | null>(null)
  const userCanModify = canModify()
  const feminineTheme = getAdminSession()?.isGirlTeacher === true

  const items = query.data ?? []
  const filteredItems = useMemo(() => {
    const term = search.trim().toLowerCase()
    if (!term) return items

    return items.filter((item) => {
      const status = item.isVisible ? 'مرئي' : 'مخفي'
      return item.name.toLowerCase().includes(term) || status.includes(term)
    })
  }, [items, search])

  const handleSave = (values: SaveWomanActivityFormValues) => {
    saveMutation.mutate(
      {
        id: editingActivity?.id,
        payload: { name: values.name.trim(), isVisible: values.isVisible },
      },
      { onSuccess: () => setSaveOpen(false) },
    )
  }

  const handleDeleteConfirm = () => {
    if (deleteId === null) return
    deleteMutation.mutate(deleteId, {
      onSettled: () => setDeleteId(null),
    })
  }

  const handleEdit = (activity: WomanActivityListItem) => {
    setEditingActivity(activity)
    setSaveOpen(true)
  }

  const hasSearch = search.trim().length > 0
  const emptyMessage = hasSearch
    ? 'لا توجد نتائج مطابقة للبحث'
    : 'لا توجد نشاطات متاحة'

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
        تعذر تحميل قائمة النشاطات. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title="قائمة النشاطات"
        description="إدارة نشاطات النساء والأنشطة المختلفة"
        {...(feminineTheme
          ? { gradientClassName: 'bg-gradient-to-br from-pink-600 to-pink-800' }
          : {})}
      />

      <WomansActivitiesFilters
        search={search}
        onSearchChange={setSearch}
        canModify={userCanModify}
        feminineTheme={feminineTheme}
        onAdd={() => {
          setEditingActivity(null)
          setSaveOpen(true)
        }}
        onExport={() => exportMutation.mutate()}
        isExporting={exportMutation.isPending}
      />

      {saveMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حفظ النشاط. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف النشاط. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {exportMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر تصدير النشاطات. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <DataTable
        data={filteredItems}
        columns={getColumns(userCanModify, feminineTheme, handleEdit, setDeleteId)}
        getRowKey={(row) => String(row.id)}
        emptyMessage={emptyMessage}
        title="قائمة النشاطات"
        defaultViewMode="card"
        showExport={false}
        renderCard={(row) => (
          <WomanActivityCard
            item={row}
            canModify={userCanModify}
            feminineTheme={feminineTheme}
            onEdit={handleEdit}
            onDelete={setDeleteId}
          />
        )}
      />

      <SaveWomanActivityDialog
        open={saveOpen}
        activity={editingActivity}
        feminineTheme={feminineTheme}
        isPending={saveMutation.isPending}
        onOpenChange={(open) => {
          setSaveOpen(open)
          if (!open) setEditingActivity(null)
        }}
        onSubmit={handleSave}
      />

      <DeleteWomanActivityDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
