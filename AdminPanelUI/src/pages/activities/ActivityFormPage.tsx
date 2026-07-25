import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { ArrowRight } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import { Skeleton } from '@/components/ui/skeleton'
import { resolveImageUrl } from '@/lib/resolveImageUrl'
import { Textarea } from '@/components/ui/textarea'
import { useActivityForm } from '@/hooks/useActivityForm'
import type { SaveActivityPayload } from '@/types/activity'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif']

const activityFormSchema = z.object({
  title: z.string().trim().min(1, 'العنوان مطلوب').max(200),
  description: z.string().optional(),
  sortOrder: z.number(),
  imageFile: z
    .instanceof(File)
    .optional()
    .refine(
      (file) => {
        if (!file) return true
        const ext = file.name.slice(file.name.lastIndexOf('.')).toLowerCase()
        return allowedExtensions.includes(ext)
      },
      { message: 'الامتدادات المسموحة: jpg, jpeg, png, gif' },
    ),
})

type ActivityFormValues = z.infer<typeof activityFormSchema>

export function ActivityFormPage() {
  const { id } = useParams()
  const activityId = id ? Number(id) : undefined
  const isValidId = activityId !== undefined && !Number.isNaN(activityId)
  const { isEdit, activityQuery, nextSortOrderQuery, saveMutation, deleteMutation } =
    useActivityForm(isValidId ? activityId : undefined)

  const [currentImageUrl, setCurrentImageUrl] = useState<string | null>(null)

  const form = useForm<ActivityFormValues>({
    resolver: zodResolver(activityFormSchema),
    defaultValues: { title: '', description: '', sortOrder: 0 },
  })

  useEffect(() => {
    if (activityQuery.data) {
      form.reset({
        title: activityQuery.data.title,
        description: activityQuery.data.description ?? '',
        sortOrder: activityQuery.data.sortOrder,
      })
      setCurrentImageUrl(activityQuery.data.imageUrl)
    }
  }, [activityQuery.data, form])

  useEffect(() => {
    if (!isEdit && nextSortOrderQuery.data !== undefined) {
      form.setValue('sortOrder', nextSortOrderQuery.data)
    }
  }, [isEdit, nextSortOrderQuery.data, form])

  const onSubmit = (values: ActivityFormValues) => {
    const payload: SaveActivityPayload = {
      title: values.title.trim(),
      description: values.description?.trim() ? values.description.trim() : null,
      sortOrder: values.sortOrder,
      imageFile: values.imageFile,
    }
    saveMutation.mutate(payload)
  }

  const handleDelete = () => {
    if (!window.confirm('حذف هذا النشاط؟')) return
    deleteMutation.mutate()
  }

  const isLoading = isEdit ? activityQuery.isLoading : nextSortOrderQuery.isLoading

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف النشاط غير صالح.</Alert>
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && activityQuery.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل بيانات النشاط. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل نشاط' : 'إضافة نشاط'}
        actions={
          <Link
            to="/activities"
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
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
            <FormField
              control={form.control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>العنوان *</FormLabel>
                  <FormControl>
                    <Input maxLength={200} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>الوصف</FormLabel>
                  <FormControl>
                    <Textarea rows={3} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="imageFile"
              render={({ field: { onChange, ...field } }) => (
                <FormItem>
                  <FormLabel>صورة النشاط</FormLabel>
                  <FormControl>
                    <Input
                      type="file"
                      accept="image/*"
                      onChange={(e) => onChange(e.target.files?.[0])}
                      {...field}
                      value={undefined}
                    />
                  </FormControl>
                  <p className="text-sm text-slate-500">
                    الامتدادات المسموحة: jpg, jpeg, png, gif
                  </p>
                  {currentImageUrl && (
                    <div className="mt-2">
                      <span className="mb-1 block text-sm">الصورة الحالية:</span>
                      <img
                        src={resolveImageUrl(currentImageUrl)}
                        alt="صورة النشاط"
                        className="max-h-[120px] max-w-[120px] rounded-lg border-2 border-slate-200 object-contain"
                      />
                    </div>
                  )}
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="sortOrder"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>ترتيب العرض</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      {...field}
                      onChange={(e) => field.onChange(e.target.valueAsNumber || 0)}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

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
                  onClick={handleDelete}
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
