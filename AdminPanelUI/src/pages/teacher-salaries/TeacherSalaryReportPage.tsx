import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ArrowRight, FileSpreadsheet } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Skeleton } from '@/components/ui/skeleton'
import { isAdmin } from '@/lib/authStorage'
import { useTeacherSalaryReport } from '@/hooks/useTeacherSalaryReport'
import { TeacherSalaryReportTable } from '@/pages/teacher-salaries/TeacherSalaryReportTable'
import { formatCurrency } from '@/types/teacherSalary'

function buildYearOptions() {
  const currentYear = new Date().getFullYear()
  return Array.from({ length: 4 }, (_, i) => currentYear - 2 + i)
}

export function TeacherSalaryReportPage() {
  const [month, setMonth] = useState(new Date().getMonth() + 1)
  const [year, setYear] = useState(new Date().getFullYear())
  const [applied, setApplied] = useState({ month, year })
  const { reportQuery, exportMutation } = useTeacherSalaryReport(
    applied.month,
    applied.year,
    true,
  )

  useEffect(() => {
    setApplied({ month, year })
  }, [])

  if (!isAdmin()) {
    return <Alert variant="destructive">غير مصرح بالوصول إلى هذه الصفحة.</Alert>
  }

  const handleGenerate = () => setApplied({ month, year })

  if (reportQuery.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (reportQuery.isError) {
    return <Alert variant="destructive">تعذر تحميل التقرير.</Alert>
  }

  const report = reportQuery.data
  const summary = report?.summary
  const items = report?.items ?? []

  return (
    <div>
      <PageHeader
        title="تقرير رواتب المعلمين الشهري"
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

      <div className="mb-6 flex flex-wrap items-end gap-4 rounded-xl bg-white p-5 shadow-md">
        <div className="space-y-1">
          <Label>الشهر</Label>
          <select
            className="rounded-lg border border-slate-200 px-3 py-2"
            value={month}
            onChange={(e) => setMonth(Number(e.target.value))}
          >
            {Array.from({ length: 12 }, (_, i) => i + 1).map((m) => (
              <option key={m} value={m}>
                {m}
              </option>
            ))}
          </select>
        </div>
        <div className="space-y-1">
          <Label>السنة</Label>
          <select
            className="rounded-lg border border-slate-200 px-3 py-2"
            value={year}
            onChange={(e) => setYear(Number(e.target.value))}
          >
            {buildYearOptions().map((y) => (
              <option key={y} value={y}>
                {y}
              </option>
            ))}
          </select>
        </div>
        <Button type="button" onClick={handleGenerate}>
          إنشاء التقرير
        </Button>
        <Button
          type="button"
          variant="outline"
          disabled={exportMutation.isPending}
          onClick={() => exportMutation.mutate()}
        >
          <FileSpreadsheet className="size-4" />
          {exportMutation.isPending ? 'جاري التصدير...' : 'تصدير إلى Excel'}
        </Button>
      </div>

      {exportMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر تصدير التقرير.
        </Alert>
      )}

      {summary && (
        <div className="mb-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-5">
          <SummaryCard label="إجمالي المعلمين" value={String(summary.totalTeachers)} />
          <SummaryCard label="إجمالي الرواتب" value={formatCurrency(summary.totalSalary)} />
          <SummaryCard label="متوسط الراتب" value={formatCurrency(summary.averageSalary)} />
          <SummaryCard label="حضور كامل" value={String(summary.fullAttendance)} />
          <SummaryCard label="مع خصومات" value={String(summary.withDeductions)} />
        </div>
      )}

      <TeacherSalaryReportTable items={items} />
    </div>
  )
}

function SummaryCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-xl border-t-4 border-[var(--color-primary)] bg-white p-4 text-center shadow-md">
      <p className="text-sm text-slate-500">{label}</p>
      <p className="mt-2 text-2xl font-bold text-[var(--color-primary)]">{value}</p>
    </div>
  )
}
