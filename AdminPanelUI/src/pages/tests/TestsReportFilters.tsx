import { Filter } from 'lucide-react'
import { SearchableDropdown } from '@/components/shared/SearchableDropdown'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { toTestsReportCircleDropdownOptions } from '@/pages/tests/testsReportConfig'
import type { TestsReportFilterOptions } from '@/types/testsReport'

interface TestsReportFiltersProps {
  fromDate: string
  toDate: string
  circleId: string
  filterOptions?: TestsReportFilterOptions
  isGenerating: boolean
  isExporting: boolean
  canExport: boolean
  onFromDateChange: (value: string) => void
  onToDateChange: (value: string) => void
  onCircleIdChange: (value: string) => void
  onGenerate: () => void
  onExport: () => void
}

export function TestsReportFilters({
  fromDate,
  toDate,
  circleId,
  filterOptions,
  isGenerating,
  isExporting,
  canExport,
  onFromDateChange,
  onToDateChange,
  onCircleIdChange,
  onGenerate,
  onExport,
}: TestsReportFiltersProps) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <h3 className="mb-4 flex items-center gap-2 text-lg font-bold text-slate-800">
        <Filter className="size-5 text-[var(--color-primary)]" strokeWidth={1.5} absoluteStrokeWidth />
        معايير التقرير
      </h3>

      <div className="grid gap-4 sm:grid-cols-2">
        <div className="space-y-1">
          <Label htmlFor="circleFilter">الحلقة</Label>
          <SearchableDropdown
            id="circleFilter"
            value={circleId}
            onChange={onCircleIdChange}
            options={toTestsReportCircleDropdownOptions(filterOptions?.circles)}
            placeholder="جميع الحلقات"
            searchPlaceholder="ابحث عن حلقة..."
          />
        </div>
      </div>

      <div className="mt-4 grid gap-4 sm:grid-cols-2">
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
      </div>

      <div className="mt-6 flex flex-wrap justify-center gap-3">
        <Button type="button" disabled={isGenerating} onClick={onGenerate}>
          {isGenerating ? 'جاري التوليد...' : 'توليد التقرير'}
        </Button>
        <Button
          type="button"
          className="bg-green-600 text-white hover:bg-green-700"
          disabled={!canExport || isExporting}
          onClick={onExport}
        >
          {isExporting ? 'جاري التصدير...' : 'تصدير Excel'}
        </Button>
      </div>
    </div>
  )
}
