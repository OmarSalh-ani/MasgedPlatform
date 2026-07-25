import type { SearchableDropdownOption } from '@/components/shared/SearchableDropdown'
import type { TestsReportCircleOption, TestsReportFilters } from '@/types/testsReport'

export function getDefaultTestsReportDates(): Pick<TestsReportFilters, 'fromDate' | 'toDate'> {
  const to = new Date()
  const from = new Date()
  from.setMonth(from.getMonth() - 1)
  return {
    fromDate: formatDateInput(from),
    toDate: formatDateInput(to),
  }
}

export function formatDateInput(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

export function toTestsReportCircleDropdownOptions(
  circles: TestsReportCircleOption[] | undefined,
): SearchableDropdownOption[] {
  return [
    { value: '', label: 'جميع الحلقات' },
    ...(circles ?? []).map((circle) => ({
      value: String(circle.id),
      label: circle.name,
    })),
  ]
}

export function validateTestsReportDates(fromDate: string, toDate: string): string | null {
  if (!fromDate || !toDate) return 'يرجى اختيار تاريخ البداية والنهاية'
  const from = new Date(fromDate)
  const to = new Date(toDate)
  if (Number.isNaN(from.getTime()) || Number.isNaN(to.getTime()))
    return 'يرجى إدخال تاريخ صحيح'
  if (from > to) return 'تاريخ البداية يجب أن يكون قبل تاريخ النهاية'
  return null
}
