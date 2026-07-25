import { Filter } from 'lucide-react'
import { SearchableDropdown } from '@/components/shared/SearchableDropdown'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { TeachersAttendanceFilterOptions } from '@/types/teachersAttendance'
import { toTeachersAttendanceDropdownOptions } from '@/types/teachersAttendance'

interface TeachersAttendanceFiltersProps {
  fromDate: string
  toDate: string
  teacherId: string
  filterOptions?: TeachersAttendanceFilterOptions
  isLoading: boolean
  onFromDateChange: (value: string) => void
  onToDateChange: (value: string) => void
  onTeacherIdChange: (value: string) => void
  onApply: () => void
}

export function TeachersAttendanceFilters({
  fromDate,
  toDate,
  teacherId,
  filterOptions,
  isLoading,
  onFromDateChange,
  onToDateChange,
  onTeacherIdChange,
  onApply,
}: TeachersAttendanceFiltersProps) {
  return (
    <div className="mb-6 rounded-xl border border-slate-200 bg-slate-50 p-5">
      <h3 className="mb-4 flex items-center gap-2 text-lg font-semibold text-[#7C8738]">
        <Filter className="size-5 text-[#CBAC2D]" />
        تصفية البيانات
      </h3>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
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
          <Label htmlFor="teacherFilter">المعلم</Label>
          <SearchableDropdown
            id="teacherFilter"
            value={teacherId}
            onChange={onTeacherIdChange}
            options={toTeachersAttendanceDropdownOptions(filterOptions?.teachers)}
            placeholder="جميع المعلمين"
            searchPlaceholder="ابحث عن معلم..."
          />
        </div>
        <div className="flex items-end">
          <Button type="button" className="w-full" disabled={isLoading} onClick={onApply}>
            {isLoading ? 'جاري التحميل...' : 'تطبيق التصفية'}
          </Button>
        </div>
      </div>
    </div>
  )
}
