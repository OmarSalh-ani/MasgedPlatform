import { useEffect } from 'react'
import { Link, Navigate, useLocation, useParams } from 'react-router-dom'
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
import { Textarea } from '@/components/ui/textarea'
import { useExpensiveForm } from '@/hooks/useExpensiveForm'
import { canModify } from '@/lib/authStorage'
import { ExpensiveAttachments } from '@/pages/expensives/ExpensiveAttachments'
import {
  expensiveFormSchema,
  type ExpensiveFormValues,
} from '@/pages/expensives/expensiveFormSchema'
import type { ExpensiveFormMode, SaveExpensivePayload } from '@/types/expensives'

function resolveMode(id: string | undefined, pathname: string): ExpensiveFormMode {
  if (!id) return 'create'
  if (pathname.endsWith('/edit')) return 'edit'
  return 'view'
}

function pageTitle(mode: ExpensiveFormMode): string {
  if (mode === 'edit') return 'تعديل المصروف'
  if (mode === 'view') return 'عرض المصروف'
  return 'إضافة مصروف جديد'
}

export function ExpensiveFormPage() {
  const { id } = useParams()
  const { pathname } = useLocation()
  const expensiveId = id ? Number(id) : undefined
  const isValidId = expensiveId !== undefined && !Number.isNaN(expensiveId)
  const mode = resolveMode(id, pathname)
  const userCanModify = canModify()
  const readOnly = mode === 'view' || !userCanModify

  const {
    expensiveQuery,
    saveMutation,
    deleteAttachmentMutation,
    downloadAttachment,
    goToList,
  } = useExpensiveForm(isValidId ? expensiveId : undefined, mode)

  const form = useForm<ExpensiveFormValues>({
    resolver: zodResolver(expensiveFormSchema),
    defaultValues: { reason: '', totalAmount: 0, supplier: '', notes: '', files: undefined },
  })

  useEffect(() => {
    if (!expensiveQuery.data) return
    form.reset({
      reason: expensiveQuery.data.reason,
      totalAmount: expensiveQuery.data.totalAmount,
      supplier: expensiveQuery.data.supplier,
      notes: expensiveQuery.data.notes ?? '',
    })
  }, [expensiveQuery.data, form])

  if (mode === 'edit' && !userCanModify) {
    return <Navigate to="/expensives" replace />
  }

  if (mode !== 'create' && !isValidId) {
    return <Alert variant="destructive">معرّف المصروف غير صالح.</Alert>
  }

  if (mode !== 'create' && expensiveQuery.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (mode !== 'create' && expensiveQuery.isError) {
    return <Alert variant="destructive">تعذر تحميل بيانات المصروف.</Alert>
  }

  const onSubmit = (values: ExpensiveFormValues) => {
    const files = values.files ? Array.from(values.files) : undefined
    const payload: SaveExpensivePayload = {
      reason: values.reason.trim(),
      supplier: values.supplier.trim(),
      totalAmount: values.totalAmount,
      notes: values.notes?.trim() ? values.notes.trim() : null,
      files,
    }
    saveMutation.mutate(payload)
  }

  const attachments = expensiveQuery.data?.attachments ?? []

  return (
    <div>
      <PageHeader
        title={pageTitle(mode)}
        description="إدخال تفاصيل المصروف والمرفقات"
        actions={
          <Link
            to="/expensives"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <ArrowRight className="size-4" />
            العودة للقائمة
          </Link>
        }
      />

      {saveMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حفظ المصروف. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <Card className="mb-6 p-6">
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="reason"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>سبب الصرف *</FormLabel>
                    <FormControl>
                      <Input placeholder="أدخل سبب الصرف" disabled={readOnly} {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="totalAmount"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>القيمة *</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        step="0.01"
                        min={0}
                        disabled={readOnly}
                        {...field}
                        onChange={(e) => field.onChange(e.target.valueAsNumber || 0)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="supplier"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>اسم المورد *</FormLabel>
                    <FormControl>
                      <Input placeholder="أدخل اسم المورد" disabled={readOnly} {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="notes"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>الملاحظات</FormLabel>
                    <FormControl>
                      <Textarea rows={3} disabled={readOnly} {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {!readOnly && (
              <FormField
                control={form.control}
                name="files"
                render={({ field: { onChange, ...field } }) => (
                  <FormItem>
                    <FormLabel>المرفقات</FormLabel>
                    <FormControl>
                      <Input
                        type="file"
                        multiple
                        onChange={(e) => onChange(e.target.files ?? undefined)}
                        {...field}
                        value={undefined}
                      />
                    </FormControl>
                    <p className="text-sm text-slate-500">يمكنك اختيار ملفات متعددة</p>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            <div className="flex flex-wrap justify-end gap-2">
              <Button type="button" variant="outline" onClick={goToList}>
                إلغاء
              </Button>
              {!readOnly && (
                <Button type="submit" disabled={saveMutation.isPending}>
                  {saveMutation.isPending ? 'جاري الحفظ...' : 'حفظ المصروف'}
                </Button>
              )}
            </div>
          </form>
        </Form>
      </Card>

      {attachments.length > 0 && (
        <Card className="p-6">
          <h2 className="mb-4 text-lg font-semibold text-[#7C8738]">المرفقات المرفوعة</h2>
          <ExpensiveAttachments
            attachments={attachments}
            readOnly={readOnly}
            isDeleting={deleteAttachmentMutation.isPending}
            onDownload={downloadAttachment}
            onDelete={(fileName) => deleteAttachmentMutation.mutate(fileName)}
          />
        </Card>
      )}
    </div>
  )
}
