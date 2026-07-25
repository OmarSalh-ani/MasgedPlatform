import { Filter } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'

interface ParentPanelLogStatisticsFiltersProps {
  fromDate: string
  toDate: string
  isLoading: boolean
  onFromDateChange: (value: string) => void
  onToDateChange: (value: string) => void
  onApply: () => void
}

export function ParentPanelLogStatisticsFilters({
  fromDate,
  toDate,
  isLoading,
  onFromDateChange,
  onToDateChange,
  onApply,
}: ParentPanelLogStatisticsFiltersProps) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <h3 className="mb-4 flex items-center gap-2 text-lg font-bold text-slate-800">
        <Filter className="size-5 text-[var(--color-primary)]" strokeWidth={1.5} absoluteStrokeWidth />
        تصفية البيانات
      </h3>

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
        <div className="flex items-end">
          <Button type="button" className="w-full" disabled={isLoading} onClick={onApply}>
            {isLoading ? 'جاري التحميل...' : 'تطبيق التصفية'}
          </Button>
        </div>
      </div>
    </div>
  )
}
