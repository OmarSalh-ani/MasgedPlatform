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
import { useCompetitionForm } from '@/hooks/useCompetitionForm'
import type { SaveCompetitionPayload } from '@/types/competition'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp']

const competitionFormSchema = z.object({
  title: z.string().trim().min(1).max(200),
  description: z.string().optional(),
  linkUrl: z.string().optional(),
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
      { message: 'الامتدادات المسموحة: jpg, jpeg, png, gif, webp' },
    ),
})

type CompetitionFormValues = z.infer<typeof competitionFormSchema>

export function CompetitionFormPage() {
  const { id } = useParams()
  const competitionId = id ? Number(id) : undefined
  const isValidId = competitionId !== undefined && !Number.isNaN(competitionId)
  const { isEdit, competitionQuery, nextSortOrderQuery, saveMutation, deleteMutation } =
    useCompetitionForm(isValidId ? competitionId : undefined)

  const [currentImageUrl, setCurrentImageUrl] = useState<string | null>(null)

  const form = useForm<CompetitionFormValues>({
    resolver: zodResolver(competitionFormSchema),
    defaultValues: { title: '', description: '', linkUrl: '', sortOrder: 0 },
  })

  useEffect(() => {
    if (competitionQuery.data) {
      form.reset({
        title: competitionQuery.data.title,
        description: competitionQuery.data.description ?? '',
        linkUrl: competitionQuery.data.linkUrl ?? '',
        sortOrder: competitionQuery.data.sortOrder,
      })
      setCurrentImageUrl(competitionQuery.data.imageUrl)
    }
  }, [competitionQuery.data, form])

  useEffect(() => {
    if (!isEdit && nextSortOrderQuery.data !== undefined) {
      form.setValue('sortOrder', nextSortOrderQuery.data)
    }
  }, [isEdit, nextSortOrderQuery.data, form])

  const onSubmit = (values: CompetitionFormValues) => {
    const payload: SaveCompetitionPayload = {
      title: values.title.trim(),
      description: values.description?.trim() ? values.description.trim() : null,
      linkUrl: values.linkUrl?.trim() ? values.linkUrl.trim() : null,
      sortOrder: values.sortOrder,
      imageFile: values.imageFile,
    }
    saveMutation.mutate(payload)
  }

  const handleDelete = () => {
    if (!window.confirm('حذف هذه المسابقة؟')) return
    deleteMutation.mutate()
  }

  const isLoading = isEdit ? competitionQuery.isLoading : nextSortOrderQuery.isLoading

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف المسابقة غير صالح.</Alert>
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && competitionQuery.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل بيانات المسابقة. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل مسابقة' : 'إضافة مسابقة'}
        actions={
          <Link
            to="/competitions"
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
                  <FormLabel>صورة المسابقة</FormLabel>
                  {currentImageUrl && (
                    <div className="mb-3">
                      <span className="mb-1 block text-sm text-slate-500">الصورة الحالية:</span>
                      <img
                        src={resolveImageUrl(currentImageUrl)}
                        alt="صورة المسابقة"
                        className="max-h-[200px] max-w-full rounded-lg border object-contain"
                      />
                      <p className="mt-1 text-sm text-slate-500">
                        اختر صورة جديدة أدناه لاستبدالها
                      </p>
                    </div>
                  )}
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
                    امتدادات مقبولة: JPG, JPEG, PNG, GIF, WebP
                  </p>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="linkUrl"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>رابط المسابقة</FormLabel>
                  <FormControl>
                    <Input {...field} />
                  </FormControl>
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
