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
import { useTipForm } from '@/hooks/useTipForm'
import type { SaveTipPayload } from '@/types/tip'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp']

const tipFormSchema = z.object({
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

type TipFormValues = z.infer<typeof tipFormSchema>

export function TipFormPage() {
  const { id } = useParams()
  const tipId = id ? Number(id) : undefined
  const isValidId = tipId !== undefined && !Number.isNaN(tipId)
  const { isEdit, tipQuery, nextSortOrderQuery, saveMutation, deleteMutation } =
    useTipForm(isValidId ? tipId : undefined)

  const [currentImageUrl, setCurrentImageUrl] = useState<string | null>(null)

  const form = useForm<TipFormValues>({
    resolver: zodResolver(tipFormSchema),
    defaultValues: { title: '', description: '', linkUrl: '', sortOrder: 0 },
  })

  useEffect(() => {
    if (tipQuery.data) {
      form.reset({
        title: tipQuery.data.title,
        description: tipQuery.data.description ?? '',
        linkUrl: tipQuery.data.linkUrl ?? '',
        sortOrder: tipQuery.data.sortOrder,
      })
      setCurrentImageUrl(tipQuery.data.imageUrl)
    }
  }, [tipQuery.data, form])

  useEffect(() => {
    if (!isEdit && nextSortOrderQuery.data !== undefined) {
      form.setValue('sortOrder', nextSortOrderQuery.data)
    }
  }, [isEdit, nextSortOrderQuery.data, form])

  const onSubmit = (values: TipFormValues) => {
    const payload: SaveTipPayload = {
      title: values.title.trim(),
      description: values.description?.trim() ? values.description.trim() : null,
      linkUrl: values.linkUrl?.trim() ? values.linkUrl.trim() : null,
      sortOrder: values.sortOrder,
      imageFile: values.imageFile,
    }
    saveMutation.mutate(payload)
  }

  const handleDelete = () => {
    if (!window.confirm('حذف هذه النصيحة؟')) return
    deleteMutation.mutate()
  }

  const isLoading = isEdit ? tipQuery.isLoading : nextSortOrderQuery.isLoading

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف النصيحة غير صالح.</Alert>
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && tipQuery.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل بيانات النصيحة. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل نصيحة' : 'إضافة نصيحة'}
        actions={
          <Link
            to="/tips"
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
                  <FormLabel>صورة النصيحة</FormLabel>
                  {currentImageUrl && (
                    <div className="mb-3">
                      <span className="mb-1 block text-sm text-slate-500">الصورة الحالية:</span>
                      <img
                        src={resolveImageUrl(currentImageUrl)}
                        alt="صورة النصيحة"
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
                  <FormLabel>رابط النصيحة</FormLabel>
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
