import { CheckCircle, FileSpreadsheet } from 'lucide-react'
import { SearchableDropdown } from '@/components/shared/SearchableDropdown'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import type { MemorizationRevisionStudentPick } from '@/types/memorizationRevisionReport'
import { toMemorizationRevisionStudentDropdownOptions } from '@/types/memorizationRevisionReport'

interface MemorizationRevisionReportFiltersProps {
  studentId: string
  students: MemorizationRevisionStudentPick[]
  isExportingFull: boolean
  isExportingCompleted: boolean
  onStudentIdChange: (value: string) => void
  onExportFull: () => void
  onExportCompleted: () => void
}

export function MemorizationRevisionReportFilters({
  studentId,
  students,
  isExportingFull,
  isExportingCompleted,
  onStudentIdChange,
  onExportFull,
  onExportCompleted,
}: MemorizationRevisionReportFiltersProps) {
  return (
    <div className="rounded-xl bg-white p-6 shadow-md">
      <div className="grid gap-4 md:grid-cols-2 md:items-end">
        <div className="space-y-1">
          <Label htmlFor="studentPick">الطالب</Label>
          <SearchableDropdown
            id="studentPick"
            value={studentId}
            onChange={onStudentIdChange}
            options={toMemorizationRevisionStudentDropdownOptions(students)}
            placeholder="— اختر الطالب —"
            searchPlaceholder="ابحث باسم الطالب..."
          />
        </div>
        <div className="flex flex-wrap gap-2">
          <Button
            type="button"
            className="bg-green-600 hover:bg-green-700"
            disabled={!studentId || isExportingFull}
            onClick={onExportFull}
          >
            <FileSpreadsheet className="size-4" />
            {isExportingFull ? 'جاري التصدير...' : 'تصدير Excel'}
          </Button>
          <Button
            type="button"
            variant="outline"
            className="border-green-600 text-green-700 hover:bg-green-50"
            disabled={!studentId || isExportingCompleted}
            onClick={onExportCompleted}
          >
            <CheckCircle className="size-4" />
            {isExportingCompleted ? 'جاري التصدير...' : 'تصدير بيانات السور التي تمت فقط'}
          </Button>
        </div>
      </div>
    </div>
  )
}
