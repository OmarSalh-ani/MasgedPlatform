import { Link, useNavigate, useParams } from 'react-router-dom'
import { ArrowRight, MessageSquarePlus, Pencil } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useSendNoteForm } from '@/hooks/useSendNoteForm'
import { SendNoteCreateForm } from '@/pages/send-notes/SendNoteCreateForm'
import { SendNoteEditForm } from '@/pages/send-notes/SendNoteEditForm'
import type { CreateSendNotePayload, UpdateSendNotePayload } from '@/types/sendNote'

export function SendNoteFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const noteId = id ? Number(id) : undefined
  const isValidId = noteId !== undefined && !Number.isNaN(noteId)
  const isEdit = isValidId
  const { noteQuery, teachersQuery, createMutation, updateMutation } = useSendNoteForm(
    isEdit ? noteId : undefined,
  )
  const isSaving = isEdit ? updateMutation.isPending : createMutation.isPending
  const hasSaveError = isEdit ? updateMutation.isError : createMutation.isError
  const isLoading = isEdit ? noteQuery.isLoading : teachersQuery.isLoading

  const title = isEdit ? 'تعديل الملاحظة' : 'إضافة ملاحظة جديدة'
  const description = isEdit
    ? 'تعديل نص الملاحظة المرسلة للمعلم'
    : 'اختر المعلمين واكتب الملاحظة المراد إرسالها'

  if (isEdit && !isValidId) {
    return (
      <div className="mx-auto max-w-7xl">
        <Alert variant="destructive">معرّف الملاحظة غير صالح.</Alert>
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-7xl space-y-8">
      <PageHeader
        icon={isEdit ? Pencil : MessageSquarePlus}
        title={title}
        description={description}
        actions={
          <Link
            to="/send-notes"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-white/30"
          >
            <ArrowRight className="size-4" strokeWidth={1.5} absoluteStrokeWidth />
            العودة للقائمة
          </Link>
        }
      />

      {isLoading && (
        <div className="mt-8">
          <SendNoteFormPageSkeleton />
        </div>
      )}

      {isEdit && noteQuery.isError && (
        <Alert variant="destructive" className="mt-8">
          تعذر تحميل الملاحظة.{' '}
          <button type="button" className="underline" onClick={() => navigate('/send-notes')}>
            العودة للقائمة
          </button>
        </Alert>
      )}

      {hasSaveError && (
        <Alert variant="destructive" className="mt-8">
          تعذر إتمام العملية. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {!isLoading && !(isEdit && noteQuery.isError) && (
        <div className="mt-8 overflow-hidden rounded-2xl border border-slate-200 bg-white p-6 shadow-sm sm:p-8">
          {isEdit && noteQuery.data ? (
            <SendNoteEditForm
              key={noteQuery.data.id}
              teacherName={noteQuery.data.teacherName}
              defaultNote={noteQuery.data.note}
              isPending={isSaving}
              onSubmit={(payload: UpdateSendNotePayload) => updateMutation.mutate(payload)}
            />
          ) : (
            <SendNoteCreateForm
              teachers={teachersQuery.data ?? []}
              isPending={isSaving}
              onSubmit={(payload: CreateSendNotePayload) => createMutation.mutate(payload)}
            />
          )}
        </div>
      )}
    </div>
  )
}

function SendNoteFormPageSkeleton() {
  return (
    <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white p-6 shadow-sm sm:p-8">
      <div className="space-y-5">
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-40 w-full rounded-xl" />
        <Skeleton className="h-32 w-full" />
        <div className="flex justify-end">
          <Skeleton className="h-10 w-40" />
        </div>
      </div>
    </div>
  )
}
