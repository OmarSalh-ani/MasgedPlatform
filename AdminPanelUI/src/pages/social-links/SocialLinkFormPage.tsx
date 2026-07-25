import { useEffect } from 'react'
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
import { useSocialLinkForm } from '@/hooks/useSocialLinkForm'
import { SocialIconPicker } from '@/pages/social-links/SocialIconPicker'
import {
  socialLinkFormSchema,
  type SocialLinkFormValues,
} from '@/pages/social-links/socialLinkFormSchema'
import type { SaveSocialLinkPayload } from '@/types/socialLink'

export function SocialLinkFormPage() {
  const { id } = useParams()
  const socialLinkId = id ? Number(id) : undefined
  const isValidId = socialLinkId !== undefined && !Number.isNaN(socialLinkId)
  const { isEdit, socialLinkQuery, nextSortOrderQuery, saveMutation, deleteMutation } =
    useSocialLinkForm(isValidId ? socialLinkId : undefined)

  const form = useForm<SocialLinkFormValues>({
    resolver: zodResolver(socialLinkFormSchema),
    defaultValues: { platformName: '', url: '', iconClass: '', sortOrder: 0 },
  })

  useEffect(() => {
    if (socialLinkQuery.data) {
      form.reset({
        platformName: socialLinkQuery.data.platformName,
        url: socialLinkQuery.data.url,
        iconClass: socialLinkQuery.data.iconClass ?? '',
        sortOrder: socialLinkQuery.data.sortOrder,
      })
    }
  }, [socialLinkQuery.data, form])

  useEffect(() => {
    if (!isEdit && nextSortOrderQuery.data !== undefined) {
      form.setValue('sortOrder', nextSortOrderQuery.data)
    }
  }, [isEdit, nextSortOrderQuery.data, form])

  const onSubmit = (values: SocialLinkFormValues) => {
    const payload: SaveSocialLinkPayload = {
      platformName: values.platformName.trim(),
      url: values.url.trim(),
      iconClass: values.iconClass?.trim() ? values.iconClass.trim() : null,
      sortOrder: values.sortOrder,
    }
    saveMutation.mutate(payload)
  }

  const handleDelete = () => {
    if (!window.confirm('حذف؟')) return
    deleteMutation.mutate()
  }

  const isLoading = isEdit ? socialLinkQuery.isLoading : nextSortOrderQuery.isLoading

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف الرابط غير صالح.</Alert>
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && socialLinkQuery.isError) {
    return (
      <Alert variant="destructive">تعذر تحميل الرابط. يرجى المحاولة مرة أخرى.</Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل رابط' : 'إضافة رابط'}
        actions={
          <Link
            to="/social-links"
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
              name="platformName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>اسم المنصة (فيسبوك، تويتر، واتساب، ...) *</FormLabel>
                  <FormControl>
                    <Input maxLength={100} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="url"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>الرابط *</FormLabel>
                  <FormControl>
                    <Input maxLength={500} placeholder="https://..." {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="iconClass"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>اختر أيقونة المنصة</FormLabel>
                  <FormControl>
                    <SocialIconPicker value={field.value ?? ''} onChange={field.onChange} />
                  </FormControl>
                  <p className="text-sm text-slate-500">
                    أو اكتب اسم الأيقونة يدوياً (Font Awesome)
                  </p>
                  <FormControl>
                    <Input
                      maxLength={100}
                      placeholder="اختر من الأعلى أو اكتب مثلاً: fab fa-facebook-f"
                      {...field}
                    />
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
