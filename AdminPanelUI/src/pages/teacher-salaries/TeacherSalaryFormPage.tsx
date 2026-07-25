import { useEffect, useMemo, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { ArrowRight } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
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
import { isAdmin } from '@/lib/authStorage'
import { useTeacherSalaryForm } from '@/hooks/useTeacherSalaryForm'
import { DailyAttendanceTable } from '@/pages/teacher-salaries/DailyAttendanceTable'
import {
  teacherSalaryFormSchema,
  type TeacherSalaryFormValues,
} from '@/pages/teacher-salaries/teacherSalaryFormSchema'
import type { DailyAttendanceDetail, SaveTeacherSalaryPayload } from '@/types/teacherSalary'

function buildYearOptions() {
  const currentYear = new Date().getFullYear()
  return Array.from({ length: 3 }, (_, i) => currentYear - 1 + i)
}

export function TeacherSalaryFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const salaryId = id ? Number(id) : undefined
  const isValidId = salaryId !== undefined && !Number.isNaN(salaryId)
  const isEdit = isValidId
  const { salaryQuery, teachersQuery, calculateMutation, createMutation, updateMutation } =
    useTeacherSalaryForm(isEdit ? salaryId : undefined)

  const [dailyDetails, setDailyDetails] = useState<DailyAttendanceDetail[]>([])
  const [showResults, setShowResults] = useState(false)
  const yearOptions = useMemo(() => buildYearOptions(), [])

  const form = useForm<TeacherSalaryFormValues>({
    resolver: zodResolver(teacherSalaryFormSchema),
    defaultValues: {
      teacherId: 0,
      month: new Date().getMonth() + 1,
      year: new Date().getFullYear(),
      baseSalary: 0,
      daysAttended: 0,
      totalHours: 0,
      calculatedSalary: 0,
      dayOffDate: '',
      notes: '',
    },
  })

  useEffect(() => {
    if (!salaryQuery.data) return
    const salary = salaryQuery.data
    form.reset({
      teacherId: salary.teacherId,
      month: salary.month,
      year: salary.year,
      baseSalary: salary.baseSalary ?? 0,
      daysAttended: salary.daysAttended,
      totalHours: salary.totalHours,
      calculatedSalary: salary.calculatedSalary,
      dayOffDate: salary.dayOffDate?.slice(0, 10) ?? '',
      notes: salary.notes ?? '',
    })
    setShowResults(true)
  }, [salaryQuery.data, form])

  if (!isAdmin()) {
    return <Alert variant="destructive">غير مصرح بالوصول إلى هذه الصفحة.</Alert>
  }

  if (isEdit && !isValidId) {
    return <Alert variant="destructive">معرّف الراتب غير صالح.</Alert>
  }

  const teachers = teachersQuery.data ?? []
  const isLoading = isEdit ? salaryQuery.isLoading : teachersQuery.isLoading
  const isSaving = isEdit ? updateMutation.isPending : createMutation.isPending
  const hasError = isEdit ? updateMutation.isError : createMutation.isError

  const handleTeacherChange = (teacherId: number) => {
    form.setValue('teacherId', teacherId)
    const teacher = teachers.find((t) => t.id === teacherId)
    form.setValue('baseSalary', teacher?.baseSalary ?? 120)
  }

  const handleCalculate = () => {
    const values = form.getValues()
    if (values.teacherId <= 0) {
      window.alert('يرجى اختيار المعلم')
      return
    }
    if (values.baseSalary <= 0) {
      window.alert('يرجى إدخال الراتب الأساسي بشكل صحيح')
      return
    }

    calculateMutation.mutate(
      {
        teacherId: values.teacherId,
        month: values.month,
        year: values.year,
        baseSalary: values.baseSalary,
        dayOffDate: values.dayOffDate || null,
      },
      {
        onSuccess: (result) => {
          form.setValue('daysAttended', result.daysAttended)
          form.setValue('totalHours', result.totalHours)
          form.setValue('calculatedSalary', result.calculatedSalary)
          setDailyDetails(result.dailyDetails)
          setShowResults(true)
        },
      },
    )
  }

  const onSubmit = (values: TeacherSalaryFormValues) => {
    const payload: SaveTeacherSalaryPayload = {
      teacherId: values.teacherId,
      month: values.month,
      year: values.year,
      baseSalary: values.baseSalary,
      daysAttended: values.daysAttended,
      totalHours: values.totalHours,
      calculatedSalary: values.calculatedSalary,
      notes: values.notes?.trim() || undefined,
      dayOffDate: values.dayOffDate || null,
    }

    if (isEdit) {
      updateMutation.mutate(payload)
    } else {
      createMutation.mutate(payload)
    }
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-80 w-full" />
      </div>
    )
  }

  if (isEdit && salaryQuery.isError) {
    return <Alert variant="destructive">تعذر تحميل بيانات الراتب.</Alert>
  }

  return (
    <div>
      <PageHeader
        title="إضافة / تعديل راتب المعلم"
        actions={
          <Link
            to="/teacher-salaries"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <ArrowRight className="size-4" />
            العودة إلى الرواتب
          </Link>
        }
      />

      <Form {...form}>
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className="space-y-5 rounded-xl bg-white p-6 shadow-md"
        >
          {hasError && (
            <Alert variant="destructive">تعذر حفظ الراتب. يرجى المحاولة مرة أخرى.</Alert>
          )}

          <FormField
            control={form.control}
            name="teacherId"
            render={({ field }) => (
              <FormItem>
                <FormLabel>المعلم *</FormLabel>
                <FormControl>
                  {isEdit ? (
                    <Input value={salaryQuery.data?.teacherName ?? ''} disabled readOnly />
                  ) : (
                    <select
                      className="w-full rounded-lg border border-slate-200 px-3 py-2"
                      value={field.value}
                      onChange={(e) => handleTeacherChange(Number(e.target.value))}
                    >
                      <option value={0}>-- اختر المعلم --</option>
                      {teachers.map((teacher) => (
                        <option key={teacher.id} value={teacher.id}>
                          {teacher.name}
                        </option>
                      ))}
                    </select>
                  )}
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <div className="grid gap-4 md:grid-cols-2">
            <FormField
              control={form.control}
              name="month"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>الشهر *</FormLabel>
                  <FormControl>
                    <select
                      className="w-full rounded-lg border border-slate-200 px-3 py-2 disabled:bg-slate-100"
                      value={field.value}
                      disabled={isEdit}
                      onChange={(e) => field.onChange(Number(e.target.value))}
                    >
                      {Array.from({ length: 12 }, (_, i) => i + 1).map((m) => (
                        <option key={m} value={m}>
                          {m}
                        </option>
                      ))}
                    </select>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="year"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>السنة *</FormLabel>
                  <FormControl>
                    <select
                      className="w-full rounded-lg border border-slate-200 px-3 py-2 disabled:bg-slate-100"
                      value={field.value}
                      disabled={isEdit}
                      onChange={(e) => field.onChange(Number(e.target.value))}
                    >
                      {yearOptions.map((y) => (
                        <option key={y} value={y}>
                          {y}
                        </option>
                      ))}
                    </select>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <FormField
            control={form.control}
            name="baseSalary"
            render={({ field }) => (
              <FormItem>
                <FormLabel>الراتب الأساسي (د.ك) *</FormLabel>
                <FormControl>
                  <Input
                    type="number"
                    step="0.01"
                    value={field.value}
                    onChange={(e) => field.onChange(Number(e.target.value))}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="dayOffDate"
            render={({ field }) => (
              <FormItem>
                <FormLabel>يوم الإجازة (اختياري)</FormLabel>
                <FormControl>
                  <Input type="date" {...field} />
                </FormControl>
                <p className="text-xs text-slate-500">اختر اليوم الذي سيتم اعتباره يوم إجازة</p>
                <FormMessage />
              </FormItem>
            )}
          />

          <div className="text-center">
            <Button
              type="button"
              className="rounded-full bg-emerald-600 hover:bg-emerald-700"
              disabled={calculateMutation.isPending}
              onClick={handleCalculate}
            >
              {calculateMutation.isPending ? 'جاري الحساب...' : 'حساب تلقائي من الحضور'}
            </Button>
          </div>

          {showResults && (
            <div className="space-y-4 rounded-lg border border-slate-200 bg-slate-50 p-4">
              <h5 className="font-semibold text-[var(--color-primary)]">نتائج الحساب:</h5>
              <div className="grid gap-4 md:grid-cols-2">
                <FormField
                  control={form.control}
                  name="daysAttended"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>أيام الحضور</FormLabel>
                      <FormControl>
                        <Input readOnly value={field.value} onChange={field.onChange} />
                      </FormControl>
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="totalHours"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>إجمالي الساعات</FormLabel>
                      <FormControl>
                        <Input readOnly value={field.value} onChange={field.onChange} />
                      </FormControl>
                    </FormItem>
                  )}
                />
              </div>
              <FormField
                control={form.control}
                name="calculatedSalary"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>الراتب المحسوب (د.ك)</FormLabel>
                    <FormControl>
                      <Input
                        readOnly
                        className="text-lg font-bold text-[var(--color-primary)]"
                        value={field.value}
                        onChange={field.onChange}
                      />
                    </FormControl>
                  </FormItem>
                )}
              />
              <DailyAttendanceTable details={dailyDetails} />
            </div>
          )}

          <FormField
            control={form.control}
            name="notes"
            render={({ field }) => (
              <FormItem>
                <FormLabel>ملاحظات</FormLabel>
                <FormControl>
                  <Textarea rows={4} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <div className="flex justify-end gap-2 border-t pt-4">
            <Button type="button" variant="outline" onClick={() => navigate('/teacher-salaries')}>
              إلغاء
            </Button>
            <Button type="submit" disabled={isSaving}>
              {isSaving ? 'جاري الحفظ...' : 'حفظ'}
            </Button>
          </div>
        </form>
      </Form>
    </div>
  )
}
