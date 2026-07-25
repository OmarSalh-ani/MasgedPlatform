import { useEffect, useState } from 'react'
import { Link, Navigate, useNavigate, useParams } from 'react-router-dom'
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
import { getAdminSession, canModify } from '@/lib/authStorage'
import { useTeacherCircles } from '@/hooks/useTeacherCircles'
import { useTeacherForm } from '@/hooks/useTeacherForm'
import { TeacherImageUploadField } from '@/pages/teachers/TeacherImageUploadField'
import { TeacherManualLocations } from '@/pages/teachers/TeacherManualLocations'
import {
  getTeacherFormSchema,
  parseBaseSalary,
  type TeacherFormValues,
} from '@/pages/teachers/teacherFormSchema'
import type { SaveTeacherPayload, TeacherMapLocation } from '@/types/teacher'

function normalizeManualLocations(
  locations: TeacherMapLocation[],
): TeacherFormValues['manualLocations'] {
  return locations.map(({ url, lat, lng }) => ({
    url,
    lat: lat ?? undefined,
    lng: lng ?? undefined,
  }))
}

const defaultValues: TeacherFormValues = {
  name: '',
  mobile: '',
  email: '',
  password: '',
  baseSalary: '',
  circleId: '',
  isGirlTeacher: false,
  usersManage: false,
  isViewOnly: false,
  manualLocations: [],
  selectedMosqueIds: [],
}

export function TeacherFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const teacherId = id && id !== 'new' ? Number(id) : undefined
  const isValidId = teacherId !== undefined && !Number.isNaN(teacherId)
  const isEdit = isValidId
  const { teacherQuery, mosquesQuery, saveMutation, deleteMutation } = useTeacherForm(
    isEdit ? teacherId : undefined,
  )

  const session = getAdminSession()
  const [removeImage, setRemoveImage] = useState(false)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  const form = useForm<TeacherFormValues>({
    resolver: zodResolver(getTeacherFormSchema(isEdit)),
    defaultValues: {
      ...defaultValues,
      isGirlTeacher: session?.isGirlTeacher ?? false,
    },
  })

  const isGirlTeacher = form.watch('isGirlTeacher')
  const circlesQuery = useTeacherCircles(isGirlTeacher)

  useEffect(() => {
    form.setValue('circleId', '')
  }, [isGirlTeacher, form])

  useEffect(() => {
    if (!teacherQuery.data) return
    const teacher = teacherQuery.data
    form.reset({
      name: teacher.name,
      mobile: teacher.mobile ?? '',
      email: teacher.email,
      password: '',
      baseSalary: teacher.baseSalary?.toFixed(2) ?? '',
      circleId: '',
      isGirlTeacher: teacher.isGirlTeacher,
      usersManage: teacher.usersManage,
      isViewOnly: teacher.isViewOnly,
      manualLocations: normalizeManualLocations(teacher.manualLocations),
      selectedMosqueIds: teacher.selectedMosqueIds,
    })
    setRemoveImage(false)
  }, [teacherQuery.data, form])

  if (!canModify()) {
    return <Navigate to="/teachers" replace />
  }

  const buildPayload = (values: TeacherFormValues): SaveTeacherPayload => ({
    name: values.name.trim(),
    mobile: values.mobile?.trim() || null,
    email: values.email.trim(),
    password: values.password?.trim() || undefined,
    baseSalary: parseBaseSalary(values.baseSalary),
    circleId: values.circleId ? Number(values.circleId) : null,
    isGirlTeacher: values.isGirlTeacher,
    usersManage: values.usersManage,
    isViewOnly: values.isViewOnly,
    removeImage,
    imageFile: values.imageFile,
    selectedMosqueIds: values.selectedMosqueIds,
    manualLocations: values.manualLocations,
  })

  const onSubmit = (values: TeacherFormValues) => {
    setSuccessMessage(null)
    saveMutation.mutate(buildPayload(values), {
      onSuccess: () => {
        setSuccessMessage(
          isEdit ? 'تم تحديث بيانات المعلم بنجاح' : 'تم إضافة المعلم بنجاح',
        )
        if (!isEdit) {
          form.reset({
            ...defaultValues,
            isGirlTeacher: session?.isGirlTeacher ?? false,
          })
          setRemoveImage(false)
        }
      },
    })
  }

  const handleDelete = () => {
    if (!window.confirm('حذف هذا المعلم؟')) return
    deleteMutation.mutate(undefined, {
      onSuccess: () => navigate('/teachers'),
    })
  }

  const toggleMosque = (mosqueId: number, checked: boolean) => {
    const current = form.getValues('selectedMosqueIds')
    form.setValue(
      'selectedMosqueIds',
      checked ? [...current, mosqueId] : current.filter((id) => id !== mosqueId),
    )
  }

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف المعلم غير صالح.</Alert>
  }

  if (isEdit && teacherQuery.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-96 w-full" />
      </div>
    )
  }

  if (isEdit && teacherQuery.isError) {
    return <Alert variant="destructive">المعلم غير موجود</Alert>
  }

  return (
    <div>
      <PageHeader
        title={isEdit ? 'تعديل بيانات المعلم' : 'إضافة معلم جديد'}
        actions={
          <Link
            to="/teachers"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <ArrowRight className="size-4" />
            العودة للقائمة
          </Link>
        }
      />

      {successMessage && <Alert className="mb-4">{successMessage}</Alert>}
      {(saveMutation.isError || deleteMutation.isError) && (
        <Alert variant="destructive" className="mb-4">
          تعذر إتمام العملية. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <Card className="p-6">
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
            <section className="space-y-4">
              <h2 className="border-b pb-2 text-lg font-semibold text-[var(--color-primary)]">
                المعلومات الأساسية
              </h2>
              <div className="grid gap-4 sm:grid-cols-2">
                <FormField
                  control={form.control}
                  name="name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>أسم المعلم</FormLabel>
                      <FormControl>
                        <Input {...field} placeholder="أدخل أسم المعلم" />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="mobile"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>رقم الموبايل</FormLabel>
                      <FormControl>
                        <Input {...field} placeholder="أدخل رقم الموبايل" />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="email"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>البريد الألكتروني</FormLabel>
                      <FormControl>
                        <Input {...field} type="email" placeholder="أدخل البريد الألكتروني" />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="password"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>كلمة المرور</FormLabel>
                      <FormControl>
                        <Input
                          {...field}
                          type="password"
                          placeholder={
                            isEdit
                              ? 'اتركه فارغاً للاحتفاظ بالكلمة الحالية'
                              : 'أدخل كلمة المرور'
                          }
                        />
                      </FormControl>
                      {isEdit && (
                        <p className="text-sm text-slate-500">
                          اتركه فارغاً للاحتفاظ بالكلمة الحالية
                        </p>
                      )}
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="baseSalary"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>الراتب الأساسي (د.ك)</FormLabel>
                      <FormControl>
                        <Input {...field} type="number" step="0.01" placeholder="أدخل الراتب الأساسي" />
                      </FormControl>
                      <p className="text-sm text-slate-500">الراتب الأساسي للمعلم (اختياري)</p>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="circleId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>أسم الحلقة</FormLabel>
                      <FormControl>
                        <select
                          {...field}
                          className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                        >
                          <option value="">—</option>
                          {(circlesQuery.data ?? []).map((circle) => (
                            <option key={circle.id} value={String(circle.id)}>
                              {circle.name}
                            </option>
                          ))}
                        </select>
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
            </section>

            <section className="space-y-3">
              <h2 className="border-b pb-2 text-lg font-semibold text-[var(--color-primary)]">
                نوع المعلم والصلاحيات
              </h2>
              <div className="flex flex-wrap gap-3">
                {(
                  [
                    ['isGirlTeacher', 'معلم نساء'],
                    ['usersManage', 'أدمن عام'],
                    ['isViewOnly', 'عرض فقط'],
                  ] as const
                ).map(([name, label]) => (
                  <FormField
                    key={name}
                    control={form.control}
                    name={name}
                    render={({ field }) => (
                      <FormItem className="flex items-center gap-2 rounded-lg border px-4 py-3">
                        <FormControl>
                          <input
                            type="checkbox"
                            checked={field.value}
                            onChange={(e) => field.onChange(e.target.checked)}
                            className="size-4"
                          />
                        </FormControl>
                        <FormLabel className="!mt-0 cursor-pointer">{label}</FormLabel>
                      </FormItem>
                    )}
                  />
                ))}
              </div>
            </section>

            <section className="space-y-4">
              <h2 className="border-b pb-2 text-lg font-semibold text-[var(--color-primary)]">
                موقع المسجد
              </h2>
              <p className="text-sm text-slate-500">
                اختر مسجد أو أكثر من القائمة وسيتم إضافة مواقعهم تلقائياً
              </p>
              <div className="flex flex-wrap gap-2">
                {(mosquesQuery.data ?? []).map((mosque) => (
                  <label
                    key={mosque.id}
                    className="flex cursor-pointer items-center gap-2 rounded-lg border px-3 py-2"
                  >
                    <input
                      type="checkbox"
                      checked={form.watch('selectedMosqueIds').includes(mosque.id)}
                      onChange={(e) => toggleMosque(mosque.id, e.target.checked)}
                    />
                    <span>{mosque.name}</span>
                  </label>
                ))}
              </div>
              <hr />
              <p className="font-semibold">أو أضف رابط جوجل ماب يدوياً</p>
              <TeacherManualLocations
                locations={form.watch('manualLocations')}
                onChange={(locations) =>
                  form.setValue('manualLocations', normalizeManualLocations(locations))
                }
              />
            </section>

            <section className="space-y-3">
              <h2 className="border-b pb-2 text-lg font-semibold text-[var(--color-primary)]">
                صورة المعلم
              </h2>
              <TeacherImageUploadField
                currentImageUrl={removeImage ? null : teacherQuery.data?.imageUrl ?? null}
                onFileChange={(file) => {
                  form.setValue('imageFile', file)
                  if (file) setRemoveImage(false)
                }}
                onRemoveImage={() => setRemoveImage(true)}
              />
            </section>

            <div className="flex flex-wrap justify-center gap-3 border-t pt-6">
              <Button type="submit" disabled={saveMutation.isPending}>
                {saveMutation.isPending ? 'جاري الحفظ...' : 'حفظ'}
              </Button>
              <Button type="button" variant="outline" asChild>
                <Link to="/teachers">إلغاء</Link>
              </Button>
              {isEdit && (
                <Button
                  type="button"
                  variant="destructive"
                  disabled={deleteMutation.isPending}
                  onClick={handleDelete}
                >
                  حذف
                </Button>
              )}
            </div>
          </form>
        </Form>
      </Card>
    </div>
  )
}
