import { useState } from 'react'
import { Link } from 'react-router-dom'
import { ClipboardList, MessageSquarePlus } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useSendNotes } from '@/hooks/useSendNotes'
import { SendNotesTable } from '@/pages/send-notes/SendNotesTable'
import { DeleteSendNoteDialog } from '@/pages/send-notes/dialogs/DeleteSendNoteDialog'

export function SendNotesPage() {
  const [pageNumber, setPageNumber] = useState(1)
  const [deleteId, setDeleteId] = useState<number | null>(null)
  const { query, deleteMutation } = useSendNotes(pageNumber)

  const handleDeleteConfirm = () => {
    if (deleteId === null) return
    deleteMutation.mutate(deleteId, {
      onSettled: () => setDeleteId(null),
    })
  }

  const result = query.data
  const items = result?.items ?? []

  return (
    <div className="mx-auto max-w-7xl space-y-8">
      <PageHeader
        icon={ClipboardList}
        title="إرسال ملاحظات للمعلمين"
        description="إرسال ومتابعة الملاحظات المرسلة للمعلمين"
        actions={
          <Link
            to="/send-notes/new"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-white/30"
          >
            <MessageSquarePlus className="size-4" strokeWidth={1.5} absoluteStrokeWidth />
            إضافة ملاحظة جديدة
          </Link>
        }
      />

      {query.isLoading && (
        <div className="mt-8">
          <SendNotesPageSkeleton />
        </div>
      )}

      {query.isError && (
        <Alert variant="destructive" className="mt-8">
          تعذر تحميل سجل الملاحظات. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mt-8">
          تعذر حذف الملاحظة. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {result && (
        <div className="mt-8">
          <SendNotesTable
            items={items}
            pageNumber={pageNumber}
            pageSize={result.pageSize}
            totalCount={result.totalCount}
            totalPages={result.totalPages}
            onPageChange={setPageNumber}
            onDelete={setDeleteId}
          />
        </div>
      )}

      <DeleteSendNoteDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}

function SendNotesPageSkeleton() {
  return (
    <div className="space-y-4">
      <Skeleton className="h-12 w-56" />
      <Skeleton className="h-64 rounded-2xl" />
    </div>
  )
}
