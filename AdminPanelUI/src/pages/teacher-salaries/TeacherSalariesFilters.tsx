import { Link } from 'react-router-dom'
import { BarChart3, Calculator, Plus } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import type { TeacherSalaryFilterOptions } from '@/types/teacherSalary'

interface TeacherSalariesFiltersProps {
  options: TeacherSalaryFilterOptions
  month: number
  year: number
  teacherId: number
  onMonthChange: (value: number) => void
  onYearChange: (value: number) => void
  onTeacherChange: (value: number) => void
  onFilter: () => void
  onAutoCalculate: () => void
  canModify: boolean
  isAutoCalculating: boolean
}

export function TeacherSalariesFilters({
  options,
  month,
  year,
  teacherId,
  onMonthChange,
  onYearChange,
  onTeacherChange,
  onFilter,
  onAutoCalculate,
  canModify,
  isAutoCalculating,
}: TeacherSalariesFiltersProps) {
  return (
    <div className="mb-6 space-y-4 rounded-xl bg-white p-5 shadow-md">
      <div className="flex flex-wrap items-end gap-4">
        <div className="min-w-[140px] space-y-1">
          <Label>الشهر</Label>
          <select
            className="w-full rounded-lg border border-slate-200 px-3 py-2"
            value={month}
            onChange={(e) => onMonthChange(Number(e.target.value))}
          >
            {options.months.map((item) => (
              <option key={item.value} value={item.value}>
                {item.label}
              </option>
            ))}
          </select>
        </div>
        <div className="min-w-[140px] space-y-1">
          <Label>السنة</Label>
          <select
            className="w-full rounded-lg border border-slate-200 px-3 py-2"
            value={year}
            onChange={(e) => onYearChange(Number(e.target.value))}
          >
            {options.years.map((item) => (
              <option key={item.value} value={item.value}>
                {item.label}
              </option>
            ))}
          </select>
        </div>
        <div className="min-w-[180px] flex-1 space-y-1">
          <Label>المعلم</Label>
          <select
            className="w-full rounded-lg border border-slate-200 px-3 py-2"
            value={teacherId}
            onChange={(e) => onTeacherChange(Number(e.target.value))}
          >
            {options.teachers.map((item) => (
              <option key={item.value} value={item.value}>
                {item.label}
              </option>
            ))}
          </select>
        </div>
        <Button type="button" onClick={onFilter}>
          تصفية
        </Button>
      </div>

      <div className="flex flex-wrap items-center gap-3">
        <Link
          to="/teacher-salaries/new"
          className="inline-flex items-center gap-2 rounded-full bg-[var(--color-primary)] px-4 py-2 text-sm font-semibold text-white hover:opacity-90"
        >
          <Plus className="size-4" />
          إضافة راتب جديد
        </Link>
        <Link
          to="/teacher-salaries/report"
          className="inline-flex items-center gap-2 rounded-full bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700"
        >
          <BarChart3 className="size-4" />
          تقرير شهري
        </Link>
        {canModify && (
          <Button
            type="button"
            variant="outline"
            className="rounded-full border-amber-400 text-amber-800 hover:bg-amber-50"
            disabled={isAutoCalculating}
            onClick={onAutoCalculate}
          >
            <Calculator className="size-4" />
            حساب تلقائي للشهر
          </Button>
        )}
      </div>
    </div>
  )
}
