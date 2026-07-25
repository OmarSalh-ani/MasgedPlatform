import { useState } from 'react'
import { ClipboardList } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useTestsReport, useTestsReportFilterOptions } from '@/hooks/useTestsReport'
import { TestsReportFilters } from '@/pages/tests/TestsReportFilters'
import { TestsReportTable } from '@/pages/tests/TestsReportTable'
import {
  getDefaultTestsReportDates,
  validateTestsReportDates,
} from '@/pages/tests/testsReportConfig'
import { TESTS_REPORT_PAGE_SIZE, type TestsReportFilters as Filters } from '@/types/testsReport'

export function TestsReportPage() {
  const defaults = getDefaultTestsReportDates()
  const [fromDate, setFromDate] = useState(defaults.fromDate)
  const [toDate, setToDate] = useState(defaults.toDate)
  const [circleId, setCircleId] = useState('')
  const [pageNumber, setPageNumber] = useState(1)
  const [appliedFilters, setAppliedFilters] = useState<Filters | null>(null)
  const [validationError, setValidationError] = useState<string | null>(null)
  const [hasGenerated, setHasGenerated] = useState(false)

  const filterOptionsQuery = useTestsReportFilterOptions()
  const { reportQuery, exportMutation } = useTestsReport(appliedFilters)

  const buildFilters = (page: number): Filters => ({
    fromDate,
    toDate,
    circleId,
    pageNumber: page,
    pageSize: TESTS_REPORT_PAGE_SIZE,
  })

  const generateReport = (page = 1) => {
    const error = validateTestsReportDates(fromDate, toDate)
    if (error) {
      setValidationError(error)
      return
    }
    setValidationError(null)
    setPageNumber(page)
    setHasGenerated(true)
    setAppliedFilters(buildFilters(page))
  }

  const handleExport = () => {
    const error = validateTestsReportDates(fromDate, toDate)
    if (error) {
      setValidationError(error)
      return
    }
    if (!reportQuery.data?.items.length) {
      window.alert('لا توجد بيانات للتصدير')
      return
    }
    setValidationError(null)
    setAppliedFilters(buildFilters(pageNumber))
    exportMutation.mutate()
  }

  const report = reportQuery.data

  return (
    <div className="mx-auto max-w-7xl space-y-8">
      <PageHeader
        icon={ClipboardList}
        title="تقرير الأختبارات"
        description="تقرير شامل لجميع الاختبارات حسب المعايير المحددة"
        className="mb-0"
      />

      <TestsReportFilters
        fromDate={fromDate}
        toDate={toDate}
        circleId={circleId}
        filterOptions={filterOptionsQuery.data}
        isGenerating={reportQuery.isFetching && hasGenerated}
        isExporting={exportMutation.isPending}
        canExport={Boolean(report?.items.length)}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        onCircleIdChange={setCircleId}
        onGenerate={() => generateReport(1)}
        onExport={handleExport}
      />

      {validationError && <Alert variant="destructive">{validationError}</Alert>}

      {reportQuery.isLoading && hasGenerated && <TestsReportPageSkeleton />}

      {reportQuery.isError && hasGenerated && (
        <Alert variant="destructive">حدث خطأ أثناء توليد التقرير. يرجى المحاولة مرة أخرى.</Alert>
      )}

      {hasGenerated && report && report.items.length === 0 && !reportQuery.isLoading && (
        <div className="rounded-2xl border border-slate-200 bg-white px-4 py-16 text-center shadow-sm">
          <p className="text-lg font-semibold text-slate-800">لا توجد نتائج</p>
          <p className="mt-2 text-sm text-slate-500">
            لم يتم العثور على أي اختبارات تطابق المعايير المحددة.
          </p>
        </div>
      )}

      {report && report.items.length > 0 && (
        <div className="space-y-4">
          <div className="flex items-start gap-3">
            <div className="flex size-10 shrink-0 items-center justify-center rounded-xl bg-[var(--color-primary-muted)] ring-1 ring-blue-100">
              <ClipboardList className="size-5 text-[var(--color-primary)]" strokeWidth={1.5} absoluteStrokeWidth />
            </div>
            <div>
              <h2 className="text-lg font-bold text-slate-800">نتائج التقرير</h2>
              <p className="mt-0.5 text-sm text-slate-500">
                تم العثور على {report.totalCount.toLocaleString('ar-EG')} اختبار
              </p>
            </div>
          </div>

          <TestsReportTable
            items={report.items}
            totalCount={report.totalCount}
            pageNumber={report.pageNumber}
            pageSize={report.pageSize}
            totalPages={report.totalPages}
            onPageChange={(page) => generateReport(page)}
          />
        </div>
      )}
    </div>
  )
}

function TestsReportPageSkeleton() {
  return (
    <div className="space-y-4">
      <Skeleton className="h-12 w-56" />
      <Skeleton className="h-64 rounded-2xl" />
    </div>
  )
}
