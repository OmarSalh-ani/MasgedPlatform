import { useEffect, useState } from 'react'
import { Link, useLocation, useNavigate, useParams } from 'react-router-dom'
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
import { getAdminSession } from '@/lib/authStorage'
import { useFilesManagerForm } from '@/hooks/useFilesManagerForm'
import {
  filesManagerFormSchema,
  type FilesManagerFormValues,
} from '@/pages/files-manager/filesManagerFormSchema'
import type { SaveFilesManagerPayload } from '@/types/filesManager'

type FormMode = 'add' | 'view' | 'edit'

function resolveMode(id: string | undefined, pathname: string): FormMode {
  if (!id || id === 'new') return 'add'
  if (pathname.endsWith('/edit')) return 'edit'
  return 'view'
}

function resolveTitle(mode: FormMode): string {
  if (mode === 'add') return 'رفع ملف جديد'
  if (mode === 'edit') return 'تعديل الملف'
  return 'عرض الملف'
}

export function FileFormPage() {
  const { id } = useParams()
  const { pathname } = useLocation()
  const navigate = useNavigate()
  const mode = resolveMode(id, pathname)
  const filesManagerId = id && id !== 'new' ? Number(id) : undefined
  const isValidId = filesManagerId !== undefined && !Number.isNaN(filesManagerId)
  const canModify = !getAdminSession()?.isViewOnly
  const isReadOnly = mode === 'view' || !canModify

  const { isEdit, filesManagerQuery, saveMutation, deleteMutation } = useFilesManagerForm(
    isValidId ? filesManagerId : undefined,
  )

  const [submitError, setSubmitError] = useState<string | null>(null)

  const form = useForm<FilesManagerFormValues>({
    resolver: zodResolver(filesManagerFormSchema),
    defaultValues: { name: '', file: undefined },
  })

  useEffect(() => {
    if (!canModify && (mode === 'edit' || mode === 'add')) {
      navigate('/files-manager', { replace: true })
    }
  }, [mode, canModify, navigate])

  useEffect(() => {
    if (filesManagerQuery.data) {
      form.reset({ name: filesManagerQuery.data.name, file: undefined })
    }
  }, [filesManagerQuery.data, form])

  const onSubmit = (values: FilesManagerFormValues) => {
    if (!canModify || isReadOnly) {
      setSubmitError('ليس لديك صلاحية لحفظ أو تعديل الملفات')
      return
    }

    if (!values.file) {
      setSubmitError('يرجى اختيار ملف للرفع')
      return
    }

    setSubmitError(null)
    const payload: SaveFilesManagerPayload = {
      name: values.name.trim(),
      file: values.file,
    }
    saveMutation.mutate(payload)
  }

  const handleDelete = () => {
    if (!canModify) {
      setSubmitError('ليس لديك صلاحية لحذف الملفات')
      return
    }
    if (!window.confirm('هل أنت متأكد من حذف هذا الملف؟')) return
    deleteMutation.mutate()
  }

  const isLoading = isEdit && filesManagerQuery.isLoading
  const currentFileUrl = filesManagerQuery.data?.fileUrl

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف الملف غير صالح.</Alert>
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && filesManagerQuery.isError) {
    return (
      <Alert variant="destructive">تعذر تحميل بيانات الملف. يرجى المحاولة مرة أخرى.</Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title={resolveTitle(mode)}
        description={mode === 'add' ? 'أدخل بيانات الملف الجديد' : 'معلومات الملف'}
        actions={
          <Link
            to="/files-manager"
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
        <h2 className="mb-5 text-lg font-semibold text-[var(--color-primary)]">معلومات الملف</h2>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>اسم الملف *</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="أدخل اسم الملف"
                      disabled={isReadOnly}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="file"
              render={({ field: { onChange } }) => (
                <FormItem>
                  <FormLabel>الملف *</FormLabel>
                  {currentFileUrl && (
                    <div className="mb-3 rounded-lg border border-slate-200 bg-slate-50 p-3 text-sm">
                      <span className="mb-1 block text-slate-500">الملف الحالي:</span>
                      <a
                        href={currentFileUrl}
                        target="_blank"
                        rel="noreferrer"
                        className="break-all font-medium text-[var(--color-primary)] underline"
                      >
                        {currentFileUrl}
                      </a>
                    </div>
                  )}
                  <FormControl>
                    <Input
                      type="file"
                      disabled={isReadOnly}
                      onChange={(e) => onChange(e.target.files?.[0])}
                    />
                  </FormControl>
                  <p className="text-sm text-slate-500">يمكنك اختيار أي نوع من الملفات</p>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="flex flex-wrap justify-end gap-2">
              <Link
                to="/files-manager"
                className="inline-flex items-center justify-center rounded-full border border-slate-200 bg-white px-4 py-2 text-sm font-semibold hover:bg-slate-50"
              >
                إلغاء
              </Link>
              {isEdit && canModify && (
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
              {!isReadOnly && (
                <Button type="submit" disabled={saveMutation.isPending}>
                  {saveMutation.isPending ? 'جاري الحفظ...' : 'حفظ الملف'}
                </Button>
              )}
            </div>
          </form>
        </Form>
      </Card>
    </div>
  )
}
