import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
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
import { useNewsForm } from '@/hooks/useNewsForm'
import type { SaveNewsPayload } from '@/types/news'
import {
  newsFormSchema,
  todayDateInputValue,
  toNewsDateInputValue,
  type NewsFormValues,
} from '@/pages/news/newsFormSchema'

export function NewsFormPage() {
  const { id } = useParams()
  const newsId = id ? Number(id) : undefined
  const isValidId = newsId !== undefined && !Number.isNaN(newsId)
  const { isEdit, newsQuery, nextSortOrderQuery, saveMutation, deleteMutation } =
    useNewsForm(isValidId ? newsId : undefined)

  const [currentImageUrl, setCurrentImageUrl] = useState<string | null>(null)

  const form = useForm<NewsFormValues>({
    resolver: zodResolver(newsFormSchema),
    defaultValues: {
      title: '',
      description: '',
      newsDate: todayDateInputValue(),
      sortOrder: 0,
    },
  })

  useEffect(() => {
    if (newsQuery.data) {
      form.reset({
        title: newsQuery.data.title,
        description: newsQuery.data.description ?? '',
        newsDate: toNewsDateInputValue(newsQuery.data.newsDate),
        sortOrder: newsQuery.data.sortOrder,
      })
      setCurrentImageUrl(newsQuery.data.imageUrl)
    }
  }, [newsQuery.data, form])

  useEffect(() => {
    if (!isEdit && nextSortOrderQuery.data !== undefined) {
      form.setValue('sortOrder', nextSortOrderQuery.data)
    }
  }, [isEdit, nextSortOrderQuery.data, form])

  const onSubmit = (values: NewsFormValues) => {
    const payload: SaveNewsPayload = {
      title: values.title.trim(),
      description: values.description?.trim() ? values.description.trim() : null,
      newsDate: values.newsDate,
      sortOrder: values.sortOrder,
      imageFile: values.imageFile,
    }
    saveMutation.mutate(payload)
  }

  const handleDelete = () => {
    if (!window.confirm('حذف هذا الخبر؟')) return
    deleteMutation.mutate()
  }

  const isLoading = isEdit ? newsQuery.isLoading : nextSortOrderQuery.isLoading

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف الخبر غير صالح.</Alert>
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && newsQuery.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل بيانات الخبر. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل خبر' : 'إضافة خبر'}
        actions={
          <Link
            to="/news"
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
                    <Input maxLength={300} {...field} />
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
                    <Textarea rows={4} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="newsDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>تاريخ الخبر</FormLabel>
                  <FormControl>
                    <Input type="date" {...field} />
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
                  <FormLabel>صورة الخبر</FormLabel>
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
                    الامتدادات المسموحة: jpg, jpeg, png, gif, webp
                  </p>
                  {currentImageUrl && (
                    <div className="mt-2">
                      <span className="mb-1 block text-sm">الصورة الحالية:</span>
                      <img
                        src={resolveImageUrl(currentImageUrl)}
                        alt="صورة الخبر"
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
