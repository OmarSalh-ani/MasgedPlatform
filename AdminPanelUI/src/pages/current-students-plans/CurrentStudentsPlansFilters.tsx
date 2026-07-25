import { Filter } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { CurrentStudentsPlansStudentDropdown } from '@/pages/current-students-plans/CurrentStudentsPlansStudentDropdown'

interface CurrentStudentsPlansFiltersProps {
  studentId: string
  isLoading: boolean
  onStudentIdChange: (value: string) => void
  onApply: () => void
}

export function CurrentStudentsPlansFilters({
  studentId,
  isLoading,
  onStudentIdChange,
  onApply,
}: CurrentStudentsPlansFiltersProps) {
  return (
    <div className="mb-6 rounded-xl border border-slate-200 bg-slate-50 p-5">
      <h3 className="mb-4 flex items-center gap-2 text-lg font-semibold text-[var(--color-primary)]">
        <Filter className="size-5" />
        تصفية البيانات
      </h3>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div className="space-y-1">
          <Label htmlFor="currentPlansStudentFilter">الطالب</Label>
          <CurrentStudentsPlansStudentDropdown
            value={studentId}
            onChange={onStudentIdChange}
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
