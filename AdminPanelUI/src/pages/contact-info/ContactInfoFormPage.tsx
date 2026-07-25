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
import { useContactInfoForm } from '@/hooks/useContactInfoForm'
import {
  contactInfoFormSchema,
  type ContactInfoFormValues,
} from '@/pages/contact-info/contactInfoFormSchema'
import type { SaveContactInfoPayload } from '@/types/contactInfo'

export function ContactInfoFormPage() {
  const { id } = useParams()
  const contactInfoId = id ? Number(id) : undefined
  const isValidId = contactInfoId !== undefined && !Number.isNaN(contactInfoId)
  const { isEdit, contactInfoQuery, nextSortOrderQuery, saveMutation, deleteMutation } =
    useContactInfoForm(isValidId ? contactInfoId : undefined)

  const form = useForm<ContactInfoFormValues>({
    resolver: zodResolver(contactInfoFormSchema),
    defaultValues: { contactType: '', label: '', value: '', sortOrder: 0 },
  })

  useEffect(() => {
    if (contactInfoQuery.data) {
      form.reset({
        contactType: contactInfoQuery.data.contactType,
        label: contactInfoQuery.data.label ?? '',
        value: contactInfoQuery.data.value,
        sortOrder: contactInfoQuery.data.sortOrder,
      })
    }
  }, [contactInfoQuery.data, form])

  useEffect(() => {
    if (!isEdit && nextSortOrderQuery.data !== undefined) {
      form.setValue('sortOrder', nextSortOrderQuery.data)
    }
  }, [isEdit, nextSortOrderQuery.data, form])

  const onSubmit = (values: ContactInfoFormValues) => {
    const payload: SaveContactInfoPayload = {
      contactType: values.contactType.trim(),
      label: values.label?.trim() ? values.label.trim() : null,
      value: values.value.trim(),
      sortOrder: values.sortOrder,
    }
    saveMutation.mutate(payload)
  }

  const handleDelete = () => {
    if (!window.confirm('حذف؟')) return
    deleteMutation.mutate()
  }

  const isLoading = isEdit ? contactInfoQuery.isLoading : nextSortOrderQuery.isLoading

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف بيانات التواصل غير صالح.</Alert>
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && contactInfoQuery.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل بيانات التواصل. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل بيانات التواصل' : 'إضافة بيانات تواصل'}
        actions={
          <Link
            to="/contact-info"
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
              name="contactType"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>نوع (مثال: phone, whatsapp, email, address) *</FormLabel>
                  <FormControl>
                    <Input maxLength={50} placeholder="phone" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="label"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>التسمية (مثال: الهاتف، الواتساب)</FormLabel>
                  <FormControl>
                    <Input maxLength={100} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="value"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>القيمة (رقم، رابط، أو نص) *</FormLabel>
                  <FormControl>
                    <Input maxLength={500} {...field} />
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
