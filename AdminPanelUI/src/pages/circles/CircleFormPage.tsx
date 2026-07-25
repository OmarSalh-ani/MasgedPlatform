import { useEffect, useState } from 'react'
import { Link, Navigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowRight } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Card } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import { canModify } from '@/lib/authStorage'
import { useCircleForm } from '@/hooks/useCircleForm'
import { CircleFormFields } from '@/pages/circles/CircleFormFields'
import {
  circleFormDefaultValues,
  circleFormSchema,
  toSaveCirclePayload,
  type CircleFormValues,
} from '@/pages/circles/circleFormSchema'
import { DeleteCircleDialog } from '@/pages/circles/dialogs/DeleteCircleDialog'

export function CircleFormPage() {
  const { id } = useParams()
  const isEdit = id !== undefined && id !== 'new'
  const circleId = isEdit ? Number(id) : undefined
  const isValidId = circleId !== undefined && !Number.isNaN(circleId)
  const [deleteOpen, setDeleteOpen] = useState(false)

  const {
    circleQuery,
    teachersQuery,
    saveMutation,
    deleteMutation,
    getSaveErrorMessage,
    getDeleteErrorMessage,
  } = useCircleForm(isValidId ? circleId : undefined)

  const form = useForm<CircleFormValues>({
    resolver: zodResolver(circleFormSchema),
    defaultValues: circleFormDefaultValues,
  })

  useEffect(() => {
    if (!circleQuery.data) return
    const circle = circleQuery.data
    form.reset({
      name: circle.name,
      teacherId: circle.teacherId ? String(circle.teacherId) : '',
      forGirls: circle.forGirls,
    })
  }, [circleQuery.data, form])

  if (!canModify()) {
    return <Navigate to="/circles" replace />
  }

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف الحلقة غير صالح.</Alert>
  }

  const isLoading = (isEdit && circleQuery.isLoading) || teachersQuery.isLoading

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && circleQuery.isError) {
    return (
      <Alert variant="destructive">تعذر تحميل بيانات الحلقة. يرجى المحاولة مرة أخرى.</Alert>
    )
  }

  const onSubmit = (values: CircleFormValues) => {
    saveMutation.mutate(toSaveCirclePayload(values))
  }

  const handleDeleteConfirm = () => {
    deleteMutation.mutate(undefined, { onSettled: () => setDeleteOpen(false) })
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل الحلقة' : 'إضافة حلقة جديدة'}
        description={isEdit ? 'تعديل بيانات الحلقة' : 'أدخل بيانات الحلقة الجديدة'}
        actions={
          <Link
            to="/circles"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <ArrowRight className="size-4" />
            العودة للقائمة
          </Link>
        }
      />

      {saveMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          {getSaveErrorMessage(saveMutation.error)}
        </Alert>
      )}

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          {getDeleteErrorMessage(deleteMutation.error)}
        </Alert>
      )}

      <Card className="p-6">
        <CircleFormFields
          form={form}
          teachers={teachersQuery.data ?? []}
          isEdit={isEdit}
          isSaving={saveMutation.isPending}
          onSubmit={onSubmit}
          onDelete={() => setDeleteOpen(true)}
        />
      </Card>

      <DeleteCircleDialog
        open={deleteOpen}
        onOpenChange={setDeleteOpen}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
