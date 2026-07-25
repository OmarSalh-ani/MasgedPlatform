import { FileSpreadsheet, Search, X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { AttendanceFilter, AttendanceReportFilterOptions } from '@/types/attendanceReport'

interface AttendanceReportFiltersProps {
  fromDate: string
  toDate: string
  circleId: string
  teacherId: string
  attendanceFilter: AttendanceFilter
  filterOptions?: AttendanceReportFilterOptions
  isExporting: boolean
  onFromDateChange: (value: string) => void
  onToDateChange: (value: string) => void
  onCircleIdChange: (value: string) => void
  onTeacherIdChange: (value: string) => void
  onAttendanceFilterChange: (value: AttendanceFilter) => void
  onGenerate: () => void
  onExport: () => void
  onClear: () => void
  onQuickRange: (range: 'today' | 'yesterday' | 'week' | 'month' | 'lastMonth') => void
}

export function AttendanceReportFilters({
  fromDate,
  toDate,
  circleId,
  teacherId,
  attendanceFilter,
  filterOptions,
  isExporting,
  onFromDateChange,
  onToDateChange,
  onCircleIdChange,
  onTeacherIdChange,
  onAttendanceFilterChange,
  onGenerate,
  onExport,
  onClear,
  onQuickRange,
}: AttendanceReportFiltersProps) {
  return (
    <div className="mb-6 rounded-xl border border-slate-200 bg-slate-50 p-5">
      <h3 className="mb-4 text-lg font-semibold text-[#7C8738]">تصفية التقرير</h3>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <div className="space-y-1">
          <Label htmlFor="fromDate">من تاريخ</Label>
          <Input
            id="fromDate"
            type="date"
            value={fromDate}
            onChange={(e) => onFromDateChange(e.target.value)}
          />
        </div>
        <div className="space-y-1">
          <Label htmlFor="toDate">إلى تاريخ</Label>
          <Input
            id="toDate"
            type="date"
            value={toDate}
            onChange={(e) => onToDateChange(e.target.value)}
          />
        </div>
        <div className="space-y-1">
          <Label htmlFor="circleFilter">الحلقة</Label>
          <select
            id="circleFilter"
            className="w-full rounded-lg border border-slate-200 px-3 py-2"
            value={circleId}
            onChange={(e) => onCircleIdChange(e.target.value)}
          >
            <option value="">جميع الحلقات</option>
            {filterOptions?.circles.map((circle) => (
              <option key={circle.id} value={circle.id}>
                {circle.name}
              </option>
            ))}
          </select>
        </div>
        <div className="space-y-1">
          <Label htmlFor="teacherFilter">المعلم</Label>
          <select
            id="teacherFilter"
            className="w-full rounded-lg border border-slate-200 px-3 py-2"
            value={teacherId}
            onChange={(e) => onTeacherIdChange(e.target.value)}
          >
            <option value="">جميع المعلمين</option>
            {filterOptions?.teachers.map((teacher) => (
              <option key={teacher.id} value={teacher.id}>
                {teacher.name}
              </option>
            ))}
          </select>
        </div>
        <div className="space-y-1">
          <Label htmlFor="attendanceFilter">حالة الحضور</Label>
          <select
            id="attendanceFilter"
            className="w-full rounded-lg border border-slate-200 px-3 py-2"
            value={attendanceFilter}
            onChange={(e) => onAttendanceFilterChange(e.target.value as AttendanceFilter)}
          >
            <option value="all">الكل</option>
            <option value="present">الحضور فقط</option>
            <option value="departed">المنصرفين فقط</option>
            <option value="absent">الغياب فقط</option>
          </select>
        </div>
      </div>

      <div className="mt-4 flex flex-wrap justify-center gap-3">
        <Button type="button" onClick={onGenerate}>
          <Search className="size-4" />
          عرض التقرير
        </Button>
        <Button type="button" variant="outline" disabled={isExporting} onClick={onExport}>
          <FileSpreadsheet className="size-4" />
          {isExporting ? 'جاري التصدير...' : 'تصدير Excel'}
        </Button>
        <Button type="button" variant="outline" onClick={onClear}>
          <X className="size-4" />
          مسح الفلاتر
        </Button>
      </div>

      <div className="mt-4 text-center">
        <p className="mb-2 text-sm text-slate-500">فلاتر سريعة</p>
        <div className="flex flex-wrap justify-center gap-2">
          {(
            [
              ['today', 'اليوم'],
              ['yesterday', 'أمس'],
              ['week', 'هذا الأسبوع'],
              ['month', 'هذا الشهر'],
              ['lastMonth', 'الشهر الماضي'],
            ] as const
          ).map(([range, label]) => (
            <Button key={range} type="button" variant="outline" className="h-8 px-3 text-sm" onClick={() => onQuickRange(range)}>
              {label}
            </Button>
          ))}
        </div>
      </div>
    </div>
  )
}
