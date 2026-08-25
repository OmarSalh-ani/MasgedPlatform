import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Plus } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useEventPages } from '@/hooks/useEventPages'
import { DeleteEventPageDialog } from '@/pages/event-pages/dialogs/DeleteEventPageDialog'
import { EventPagesTable } from '@/pages/event-pages/EventPagesTable'

export function EventPagesPage() {
  const { query, deleteMutation } = useEventPages()
  const [deleteId, setDeleteId] = useState<number | null>(null)
  const [copied, setCopied] = useState(false)

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
      <Alert variant="destructive">تعذر تحميل صفحات التسجيل. يرجى المحاولة مرة أخرى.</Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title="صفحات التسجيل"
        description="إنشاء صفحات عامة للدورات مع نموذج تسجيل ديناميكي"
        actions={
          <Link
            to="/event-pages/new"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <Plus className="size-4" />
            إضافة صفحة
          </Link>
        }
      />

      {copied && (
        <Alert className="mb-4">تم نسخ رابط الصفحة.</Alert>
      )}
      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف الصفحة. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <EventPagesTable
        items={query.data ?? []}
        emptyMessage="لا توجد صفحات"
        onDelete={setDeleteId}
        onCopied={() => {
          setCopied(true)
          window.setTimeout(() => setCopied(false), 2500)
        }}
      />

      <DeleteEventPageDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
