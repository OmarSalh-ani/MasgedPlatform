import { FileSpreadsheet, FileText } from 'lucide-react'
import { SearchableDropdown } from '@/components/shared/SearchableDropdown'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type {
  CircleMemorizationTeacherOption,
  CircleReportExportFormat,
} from '@/types/circleMemorizationRevisionReport'
import { toCircleMemorizationTeacherDropdownOptions } from '@/types/circleMemorizationRevisionReport'

interface CircleMemorizationRevisionReportFiltersProps {
  teacherId: string
  fromDate: string
  toDate: string
  format: CircleReportExportFormat
  teachers: CircleMemorizationTeacherOption[]
  isExporting: boolean
  onTeacherIdChange: (value: string) => void
  onFromDateChange: (value: string) => void
  onToDateChange: (value: string) => void
  onFormatChange: (value: CircleReportExportFormat) => void
  onExport: () => void
}

export function CircleMemorizationRevisionReportFilters({
  teacherId,
  fromDate,
  toDate,
  format,
  teachers,
  isExporting,
  onTeacherIdChange,
  onFromDateChange,
  onToDateChange,
  onFormatChange,
  onExport,
}: CircleMemorizationRevisionReportFiltersProps) {
  return (
    <div className="rounded-xl bg-white p-6 shadow-md">
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4 lg:items-end">
        <div className="space-y-1">
          <Label htmlFor="teacherPick">المعلم</Label>
          <SearchableDropdown
            id="teacherPick"
            value={teacherId}
            onChange={onTeacherIdChange}
            options={toCircleMemorizationTeacherDropdownOptions(teachers)}
            placeholder="— اختر المعلم —"
            searchPlaceholder="ابحث باسم المعلم..."
          />
        </div>
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
          <Label htmlFor="toDate">الى تاريخ</Label>
          <Input
            id="toDate"
            type="date"
            value={toDate}
            onChange={(e) => onToDateChange(e.target.value)}
          />
        </div>
        <div className="space-y-1">
          <Label>صيغة التقرير</Label>
          <div className="flex gap-2">
            <Button
              type="button"
              variant={format === 'pdf' ? 'default' : 'outline'}
              className="flex-1"
              onClick={() => onFormatChange('pdf')}
            >
              <FileText className="size-4" />
              PDF
            </Button>
            <Button
              type="button"
              variant={format === 'excel' ? 'default' : 'outline'}
              className="flex-1"
              onClick={() => onFormatChange('excel')}
            >
              <FileSpreadsheet className="size-4" />
              Excel
            </Button>
          </div>
        </div>
      </div>
      <div className="mt-4 flex justify-end">
        <Button
          type="button"
          className="bg-green-600 hover:bg-green-700"
          disabled={!teacherId || !fromDate || !toDate || isExporting}
          onClick={onExport}
        >
          {isExporting ? 'جاري التوليد...' : 'توليد التقرير'}
        </Button>
      </div>
    </div>
  )
}
