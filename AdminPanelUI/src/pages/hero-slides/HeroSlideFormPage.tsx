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
import { useHeroSlideForm } from '@/hooks/useHeroSlideForm'
import type { SaveHeroSlidePayload } from '@/types/heroSlide'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp']

const heroSlideFormSchema = z.object({
  sortOrder: z.number(),
  imageFiles: z
    .array(z.instanceof(File))
    .refine(
      (files) =>
        files.every((file) => {
          const ext = file.name.slice(file.name.lastIndexOf('.')).toLowerCase()
          return allowedExtensions.includes(ext)
        }),
      { message: 'يرجى اختيار ملفات صورة فقط (JPG, PNG, GIF, WebP).' },
    ),
})

type HeroSlideFormValues = z.infer<typeof heroSlideFormSchema>

export function HeroSlideFormPage() {
  const { id } = useParams()
  const heroSlideId = id ? Number(id) : undefined
  const isValidId = heroSlideId !== undefined && !Number.isNaN(heroSlideId)
  const { isEdit, heroSlideQuery, nextSortOrderQuery, saveMutation, deleteMutation } =
    useHeroSlideForm(isValidId ? heroSlideId : undefined)

  const [currentImageUrl, setCurrentImageUrl] = useState<string | null>(null)
  const [submitError, setSubmitError] = useState<string | null>(null)

  const form = useForm<HeroSlideFormValues>({
    resolver: zodResolver(heroSlideFormSchema),
    defaultValues: { sortOrder: 0, imageFiles: [] },
  })

  useEffect(() => {
    if (heroSlideQuery.data) {
      form.reset({
        sortOrder: heroSlideQuery.data.sortOrder,
        imageFiles: [],
      })
      setCurrentImageUrl(heroSlideQuery.data.imageUrl)
    }
  }, [heroSlideQuery.data, form])

  useEffect(() => {
    if (!isEdit && nextSortOrderQuery.data !== undefined) {
      form.setValue('sortOrder', nextSortOrderQuery.data)
    }
  }, [isEdit, nextSortOrderQuery.data, form])

  const onSubmit = (values: HeroSlideFormValues) => {
    if (values.imageFiles.length === 0 && !currentImageUrl) {
      setSubmitError('يرجى اختيار صورة أو أكثر للرفع.')
      return
    }

    setSubmitError(null)
    const payload: SaveHeroSlidePayload = {
      sortOrder: values.sortOrder,
      imageFiles: values.imageFiles,
    }
    saveMutation.mutate(payload)
  }

  const handleDelete = () => {
    if (!window.confirm('حذف هذه الصورة؟')) return
    deleteMutation.mutate()
  }

  const isLoading = isEdit ? heroSlideQuery.isLoading : nextSortOrderQuery.isLoading
  const previewSrc = resolveImageUrl(currentImageUrl)

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف الصورة غير صالح.</Alert>
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && heroSlideQuery.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل بيانات الصورة. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل صورة الهيرو' : 'إضافة صورة هيرو'}
        actions={
          <Link
            to="/hero-slides"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <ArrowRight className="size-4" />
            العودة للقائمة
          </Link>
        }
      />

      {(saveMutation.isError || deleteMutation.isError || submitError) && (
        <Alert variant="destructive" className="mb-4">
          {submitError ?? 'تعذر إتمام العملية. يرجى المحاولة مرة أخرى.'}
        </Alert>
      )}

      <Card className="p-6">
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
            <FormField
              control={form.control}
              name="imageFiles"
              render={({ field: { onChange } }) => (
                <FormItem>
                  <FormLabel>صورة الهيرو *</FormLabel>
                  {previewSrc && (
                    <div className="mb-3">
                      <span className="mb-1 block text-sm text-slate-500">الصورة الحالية:</span>
                      <img
                        src={previewSrc}
                        alt="صورة الهيرو"
                        className="max-h-[200px] max-w-full rounded-lg border border-slate-200 object-contain"
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
                      multiple
                      onChange={(e) => onChange(Array.from(e.target.files ?? []))}
                    />
                  </FormControl>
                  <p className="text-sm text-slate-500">
                    امتدادات مقبولة: JPG, JPEG, PNG, GIF, WebP. يمكنك اختيار عدة صور مرة واحدة.
                  </p>
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
