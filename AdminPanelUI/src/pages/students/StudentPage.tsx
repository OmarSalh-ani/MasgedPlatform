import axios from 'axios'
import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowRight, GraduationCap, User } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { Form } from '@/components/ui/form'
import { Skeleton } from '@/components/ui/skeleton'
import { useStudent } from '@/hooks/useStudent'
import { StudentAcademicFields } from '@/pages/students/StudentAcademicFields'
import { StudentPersonalFields } from '@/pages/students/StudentPersonalFields'
import {
  parseStudentAge,
  studentFormDefaultValues,
  studentFormSchema,
  type StudentFormValues,
} from '@/pages/students/studentFormSchema'
import type { ApiResponse } from '@/types/api'
import type { SaveStudentPayload } from '@/types/student'

function getErrorMessage(error: unknown, fallback: string): string {
  if (axios.isAxiosError(error)) {
    const body = error.response?.data as ApiResponse<unknown> | undefined
    if (body?.errors?.length) return body.errors.join('\n')
    if (body?.message && body.message !== 'Validation failed') return body.message
  }
  if (error instanceof Error) return error.message
  return fallback
}

function formatRegistrationDate(value: string | null | undefined, fallback: string): string {
  if (!value) return fallback
  return value.slice(0, 10)
}

export function StudentPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const studentId = id && id !== 'new' ? Number(id) : undefined
  const isValidId = studentId !== undefined && !Number.isNaN(studentId)
  const isEdit = isValidId

  const { formDataQuery, studentQuery, saveMutation } = useStudent(isEdit ? studentId : undefined)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  const form = useForm<StudentFormValues>({
    resolver: zodResolver(studentFormSchema),
    defaultValues: studentFormDefaultValues,
  })

  const canModify = formDataQuery.data?.canModify ?? false
  const readOnly = !canModify

  useEffect(() => {
    if (!studentQuery.data) return
    const student = studentQuery.data
    form.reset({
      fullName: student.fullName,
      fatherPhone: student.fatherPhone,
      alternativePhone: student.alternativePhone ?? '',
      parentPanelPassword: student.parentPanelPassword ?? '',
      age: student.age > 0 ? String(student.age) : '',
      studentGender: student.studentGender === 'أنثى' ? 'أنثى' : 'ذكر',
      quranCircleId: student.quranCircleId ? String(student.quranCircleId) : '',
      planLevelId: student.planLevelId ? String(student.planLevelId) : '',
      isSpecial: student.isSpecial,
    })
  }, [studentQuery.data, form])

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف الطالب غير صالح.</Alert>
  }

  if (formDataQuery.isLoading || (isEdit && studentQuery.isLoading)) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (formDataQuery.isError) {
    return <Alert variant="destructive">تعذر تحميل بيانات النموذج.</Alert>
  }

  if (isEdit && studentQuery.isError) {
    return (
      <Alert variant="destructive">
        {getErrorMessage(studentQuery.error, 'تعذر تحميل بيانات الطالب.')}
      </Alert>
    )
  }

  const formData = formDataQuery.data!
  const student = studentQuery.data
  const pageTitle = isEdit
    ? `تعديل بيانات الطالب${student ? ` - ${student.studentName}` : ''}`
    : 'إضافة طالب جديد'
  const registrationDate = isEdit
    ? formatRegistrationDate(student?.createdAt, formData.defaultRegistrationDate)
    : formData.defaultRegistrationDate

  const buildPayload = (values: StudentFormValues): SaveStudentPayload => ({
    fullName: values.fullName.trim(),
    fatherPhone: values.fatherPhone.trim(),
    alternativePhone: values.alternativePhone?.trim() || null,
    parentPanelPassword: values.parentPanelPassword?.trim() || null,
    age: parseStudentAge(values.age),
    studentGender: values.studentGender,
    quranCircleId: values.quranCircleId ? Number(values.quranCircleId) : null,
    planLevelId: values.planLevelId ? Number(values.planLevelId) : null,
    isSpecial: values.isSpecial,
  })

  const onSubmit = (values: StudentFormValues) => {
    setSuccessMessage(null)
    saveMutation.mutate(buildPayload(values), {
      onSuccess: () => {
        setSuccessMessage('تم حفظ بيانات الطالب بنجاح')
        window.setTimeout(() => navigate('/home'), 2000)
      },
    })
  }

  return (
    <div>
      <PageHeader
        title={pageTitle}
        description="تحديث معلومات الطالب الشخصية والأكاديمية"
        className="mb-0"
        actions={
          <Link
            to="/home"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <ArrowRight className="size-4" />
            العودة للقائمة
          </Link>
        }
      />

      {successMessage ? <Alert className="mb-4">{successMessage}</Alert> : null}
      {saveMutation.isError ? (
        <Alert variant="destructive" className="mb-4">
          {getErrorMessage(saveMutation.error, 'تعذر حفظ بيانات الطالب.')}
        </Alert>
      ) : null}

      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <Card className="p-6">
            <h2 className="mb-5 flex items-center gap-2 text-lg font-semibold text-[var(--color-primary)]">
              <User className="size-5" />
              البيانات الشخصية
            </h2>
            <div className="space-y-4">
              <StudentPersonalFields control={form.control} readOnly={readOnly} />
            </div>
          </Card>

          <Card className="p-6">
            <h2 className="mb-5 flex items-center gap-2 text-lg font-semibold text-[var(--color-primary)]">
              <GraduationCap className="size-5" />
              البيانات الأضافية
            </h2>
            <div className="space-y-4">
              <StudentAcademicFields
                control={form.control}
                readOnly={readOnly}
                circles={formData.circles}
                planLevels={formData.planLevels}
                registrationDate={registrationDate}
              />
            </div>
          </Card>

          <div className="flex flex-wrap justify-end gap-3">
            <Link
              to="/home"
              className="inline-flex h-10 items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium"
            >
              إلغاء
            </Link>
            {canModify ? (
              <Button type="submit" disabled={saveMutation.isPending}>
                {saveMutation.isPending ? 'جاري الحفظ...' : 'حفظ التغييرات'}
              </Button>
            ) : null}
          </div>
        </form>
      </Form>

      <Link
        to="/home"
        className="mt-6 inline-flex items-center gap-2 font-semibold text-[var(--color-primary)]"
      >
        <ArrowRight className="size-4" />
        العودة للقائمة
      </Link>
    </div>
  )
}
