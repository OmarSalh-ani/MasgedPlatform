import { Link, useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowRight, ClipboardPlus } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Form } from '@/components/ui/form'
import { Skeleton } from '@/components/ui/skeleton'
import {
  useCircleVisitNumber,
  useCircleVisitRatingCircles,
  useCircleVisitRatingTeachers,
  useCreateCircleVisitRating,
} from '@/hooks/useCircleVisitRatings'
import { CircleRatingChecklist } from '@/pages/circle-ratings/CircleRatingChecklist'
import { CircleRatingMetaFields } from '@/pages/circle-ratings/CircleRatingMetaFields'
import {
  buildDefaultCircleRatingFormValues,
  circleRatingFormSchema,
  type CircleRatingFormValues,
} from '@/pages/circle-ratings/circleRatingFormSchema'

export function CircleRatingFormPage() {
  const navigate = useNavigate()
  const teachersQuery = useCircleVisitRatingTeachers()
  const createMutation = useCreateCircleVisitRating()

  const form = useForm<CircleRatingFormValues>({
    resolver: zodResolver(circleRatingFormSchema),
    defaultValues: buildDefaultCircleRatingFormValues(),
  })

  const teacherIdRaw = form.watch('teacherId')
  const visitDate = form.watch('visitDate')
  const teacherId = teacherIdRaw ? Number(teacherIdRaw) : null

  const circlesQuery = useCircleVisitRatingCircles(teacherId)
  const visitNumberQuery = useCircleVisitNumber(teacherId, visitDate)

  const onTeacherChange = () => {
    form.setValue('quranCircleId', '')
  }

  const onSubmit = (values: CircleRatingFormValues) => {
    createMutation.mutate(
      {
        teacherId: Number(values.teacherId),
        quranCircleId: Number(values.quranCircleId),
        visitDate: values.visitDate,
        visitTime: values.visitTime.length === 5 ? `${values.visitTime}:00` : values.visitTime,
        items: values.items.map((item) => ({
          sequence: item.sequence,
          criterion: item.criterion,
          rating: item.rating,
          notes: item.notes?.trim() || null,
        })),
      },
      { onSuccess: () => navigate('/circle-ratings') },
    )
  }

  return (
    <div className="mx-auto max-w-7xl space-y-8">
      <PageHeader
        icon={ClipboardPlus}
        title="تقييم جديد"
        description="تسجيل تقييم زيارة حلقة القرآن الكريم"
        actions={
          <Link
            to="/circle-ratings"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-white/30"
          >
            <ArrowRight className="size-4" strokeWidth={1.5} absoluteStrokeWidth />
            العودة للقائمة
          </Link>
        }
      />

      {teachersQuery.isLoading && <Skeleton className="h-48 w-full rounded-2xl" />}

      {teachersQuery.isError && (
        <Alert variant="destructive">تعذر تحميل قائمة المعلمين.</Alert>
      )}

      {createMutation.isError && (
        <Alert variant="destructive">تعذر حفظ التقييم. يرجى المحاولة مرة أخرى.</Alert>
      )}

      {teachersQuery.data && (
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <CircleRatingMetaFields
              form={form}
              teachers={teachersQuery.data}
              circles={circlesQuery.data ?? []}
              visitNumber={visitNumberQuery.data}
              onTeacherChange={onTeacherChange}
            />
            <CircleRatingChecklist form={form} />
            <div className="flex justify-end">
              <Button type="submit" disabled={createMutation.isPending}>
                {createMutation.isPending ? 'جاري الحفظ...' : 'حفظ التقييم'}
              </Button>
            </div>
          </form>
        </Form>
      )}
    </div>
  )
}
