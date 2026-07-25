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
import { useMosqueForm } from '@/hooks/useMosqueForm'
import {
  mosqueFormSchema,
  type MosqueFormValues,
} from '@/pages/mosques/mosqueFormSchema'
import type { SaveMosquePayload } from '@/types/mosque'

export function MosqueFormPage() {
  const { id } = useParams()
  const mosqueId = id ? Number(id) : undefined
  const isValidId = mosqueId !== undefined && !Number.isNaN(mosqueId)
  const { isEdit, mosqueQuery, nextSortOrderQuery, saveMutation, deleteMutation } =
    useMosqueForm(isValidId ? mosqueId : undefined)

  const [currentImageUrl, setCurrentImageUrl] = useState<string | null>(null)

  const form = useForm<MosqueFormValues>({
    resolver: zodResolver(mosqueFormSchema),
    defaultValues: { name: '', description: '', googleMapsUrl: '', sortOrder: 0 },
  })

  useEffect(() => {
    if (mosqueQuery.data) {
      form.reset({
        name: mosqueQuery.data.name,
        description: mosqueQuery.data.description ?? '',
        googleMapsUrl: mosqueQuery.data.googleMapsUrl ?? '',
        sortOrder: mosqueQuery.data.sortOrder,
      })
      setCurrentImageUrl(mosqueQuery.data.imageUrl)
    }
  }, [mosqueQuery.data, form])

  useEffect(() => {
    if (!isEdit && nextSortOrderQuery.data !== undefined) {
      form.setValue('sortOrder', nextSortOrderQuery.data)
    }
  }, [isEdit, nextSortOrderQuery.data, form])

  const onSubmit = (values: MosqueFormValues) => {
    const payload: SaveMosquePayload = {
      name: values.name.trim(),
      description: values.description?.trim() ? values.description.trim() : null,
      googleMapsUrl: values.googleMapsUrl?.trim() ? values.googleMapsUrl.trim() : null,
      sortOrder: values.sortOrder,
      imageFile: values.imageFile,
    }
    saveMutation.mutate(payload)
  }

  const handleDelete = () => {
    if (!window.confirm('حذف هذا المسجد؟')) return
    deleteMutation.mutate()
  }

  const isLoading = isEdit ? mosqueQuery.isLoading : nextSortOrderQuery.isLoading

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف المسجد غير صالح.</Alert>
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && mosqueQuery.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل بيانات المسجد. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل مسجد' : 'إضافة مسجد'}
        actions={
          <Link
            to="/mosques"
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
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>اسم المسجد *</FormLabel>
                  <FormControl>
                    <Input maxLength={200} placeholder="اسم المسجد" {...field} />
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
                    <Textarea
                      rows={3}
                      placeholder="وصف قصير أو عنوان الفرع"
                      {...field}
                    />
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
                  <FormLabel>صورة المسجد</FormLabel>
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
                        alt="صورة المسجد"
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
              name="googleMapsUrl"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>رابط Google Maps</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="https://www.google.com/maps/place/..."
                      {...field}
                    />
                  </FormControl>
                  <p className="text-sm text-slate-500">
                    رابط الموقع من Google Maps (ليس رابط الـ embed)
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
