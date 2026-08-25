import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowRight } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { Form } from '@/components/ui/form'
import { Skeleton } from '@/components/ui/skeleton'
import { useEventPageForm } from '@/hooks/useEventPageForm'
import { EventPageFormFieldsEditor } from '@/pages/event-pages/EventPageFormFieldsEditor'
import { EventPageImageField } from '@/pages/event-pages/EventPageImageField'
import { EventPageLandingFields } from '@/pages/event-pages/EventPageLandingFields'
import { EventPageMetaFields } from '@/pages/event-pages/EventPageMetaFields'
import { EventPageTracksEditor } from '@/pages/event-pages/EventPageTracksEditor'
import {
  toEventPageFormValues,
  toSaveEventPagePayload,
} from '@/pages/event-pages/eventPageFormMappers'
import {
  eventPageFormDefaults,
  eventPageFormSchema,
  type EventPageFormValues,
} from '@/pages/event-pages/eventPageFormSchema'

export function EventPageFormPage() {
  const { id } = useParams()
  const eventPageId = id ? Number(id) : undefined
  const isValidId = eventPageId !== undefined && !Number.isNaN(eventPageId)
  const { isEdit, eventPageQuery, saveMutation, deleteMutation } = useEventPageForm(
    isValidId ? eventPageId : undefined,
  )
  const [currentImageUrl, setCurrentImageUrl] = useState<string | null>(null)
  const form = useForm<EventPageFormValues>({
    resolver: zodResolver(eventPageFormSchema),
    defaultValues: eventPageFormDefaults,
  })

  useEffect(() => {
    if (!eventPageQuery.data) return
    form.reset(toEventPageFormValues(eventPageQuery.data))
    setCurrentImageUrl(eventPageQuery.data.imageUrl)
  }, [eventPageQuery.data, form])

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف الصفحة غير صالح.</Alert>
  }

  if (isEdit && eventPageQuery.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && eventPageQuery.isError) {
    return <Alert variant="destructive">تعذر تحميل بيانات الصفحة.</Alert>
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل صفحة تسجيل' : 'إضافة صفحة تسجيل'}
        actions={
          <Link
            to="/event-pages"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <ArrowRight className="size-4" />
            العودة للقائمة
          </Link>
        }
      />
      {(saveMutation.isError || deleteMutation.isError) && (
        <Alert variant="destructive" className="mb-4">
          تعذر إتمام العملية. يرجى المحاولة مرة أخرى.
        </Alert>
      )}
      <Card className="p-6">
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit((values) => saveMutation.mutate(toSaveEventPagePayload(values)))}
            className="space-y-8"
          >
            <EventPageMetaFields control={form.control} />
            <EventPageLandingFields control={form.control} />
            <EventPageImageField control={form.control} currentImageUrl={currentImageUrl} />
            <EventPageTracksEditor control={form.control} />
            <EventPageFormFieldsEditor control={form.control} />
            <div className="flex flex-wrap gap-2">
              <Button type="submit" disabled={saveMutation.isPending}>
                {saveMutation.isPending ? 'جاري الحفظ...' : 'حفظ'}
              </Button>
              {isEdit && (
                <Button
                  type="button"
                  variant="outline"
                  className="border-red-200 text-red-600 hover:bg-red-50"
                  disabled={deleteMutation.isPending}
                  onClick={() => {
                    if (window.confirm('حذف هذه الصفحة وجميع الردود؟')) deleteMutation.mutate()
                  }}
                >
                  {deleteMutation.isPending ? 'جاري الحذف...' : 'حذف'}
                </Button>
              )}
            </div>
          </form>
        </Form>
      </Card>
    </div>
  )
}
