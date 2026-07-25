import { useMemo, useState } from 'react'
import { FileText } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { getAdminSession } from '@/lib/authStorage'
import { useFilesManagers } from '@/hooks/useFilesManagers'
import { FilesManagerCard } from '@/pages/files-manager/FilesManagerCard'
import { FilesManagersFilters } from '@/pages/files-manager/FilesManagersFilters'
import { DeleteFilesManagerDialog } from '@/pages/files-manager/dialogs/DeleteFilesManagerDialog'

export function FilesManagersPage() {
  const { query, deleteMutation, exportMutation } = useFilesManagers()
  const [search, setSearch] = useState('')
  const [deleteId, setDeleteId] = useState<number | null>(null)
  const canModify = getAdminSession()?.isViewOnly !== true

  const items = query.data ?? []
  const filteredItems = useMemo(() => {
    const term = search.trim().toLowerCase()
    if (!term) return items

    return items.filter(
      (item) =>
        item.name.toLowerCase().includes(term) || item.fileUrl.toLowerCase().includes(term),
    )
  }, [items, search])

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
        تعذر تحميل قائمة الملفات. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader title="قائمة الملفات" description="إدارة الملفات والوثائق" />

      <FilesManagersFilters
        search={search}
        onSearchChange={setSearch}
        canModify={canModify}
        onExport={() => exportMutation.mutate()}
        isExporting={exportMutation.isPending}
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف الملف. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {exportMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر تصدير الملفات. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {filteredItems.length > 0 ? (
        <div className="grid gap-5 sm:grid-cols-2">
          {filteredItems.map((item) => (
            <FilesManagerCard
              key={item.id}
              item={item}
              canModify={canModify}
              onDelete={setDeleteId}
            />
          ))}
        </div>
      ) : (
        <div className="rounded-xl bg-white p-10 text-center text-slate-500 shadow-md">
          <FileText className="mx-auto mb-3 size-12 opacity-40" />
          <p className="font-medium">لا توجد ملفات متاحة</p>
          <p className="mt-1 text-sm text-slate-400">
            {search ? 'لا توجد نتائج مطابقة للبحث' : 'قم برفع ملف جديد للبدء'}
          </p>
        </div>
      )}

      <DeleteFilesManagerDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
