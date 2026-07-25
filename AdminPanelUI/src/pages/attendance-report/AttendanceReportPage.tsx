import { useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { LogOut, MessageCircle } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { canModify } from '@/lib/authStorage'
import {
  useAttendanceReport,
  useAttendanceReportFilterOptions,
} from '@/hooks/useAttendanceReport'
import { AttendanceReportFilters } from '@/pages/attendance-report/AttendanceReportFilters'
import { AttendanceReportTable } from '@/pages/attendance-report/AttendanceReportTable'
import { AttendanceWhatsappDialog } from '@/pages/attendance-report/dialogs/AttendanceWhatsappDialog'
import {
  buildAppliedFilters,
  getInitialAttendanceFilter,
  getInitialDatesByType,
  getQuickDateRange,
  parseSelectedRows,
  uniqueStudentIdsFromRows,
  validateExportDates,
  validateReportDates,
} from '@/pages/attendance-report/attendanceReportUtils'
import { getDefaultDateRange, getRowKey, type AttendanceFilter } from '@/types/attendanceReport'

export function AttendanceReportPage() {
  const [searchParams] = useSearchParams()
  const initialDates = getInitialDatesByType(searchParams.get('type'))
  const [fromDate, setFromDate] = useState(initialDates.fromDate)
  const [toDate, setToDate] = useState(initialDates.toDate)
  const [circleId, setCircleId] = useState('')
  const [teacherId, setTeacherId] = useState('')
  const [attendanceFilter, setAttendanceFilter] = useState<AttendanceFilter>(
    getInitialAttendanceFilter(searchParams.get('type')),
  )
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(50)
  const [appliedFilters, setAppliedFilters] = useState<ReturnType<typeof buildAppliedFilters> | null>(
    null,
  )
  const [selectedRows, setSelectedRows] = useState<Set<string>>(new Set())
  const [validationError, setValidationError] = useState<string | null>(null)
  const [whatsappOpen, setWhatsappOpen] = useState(false)

  const filterOptionsQuery = useAttendanceReportFilterOptions()
  const { reportQuery, exportMutation, whatsappMutation, departureMutation } =
    useAttendanceReport(appliedFilters)

  useEffect(() => {
    const error = validateReportDates(fromDate, toDate)
    if (error) return
    setAppliedFilters(
      buildAppliedFilters({
        fromDate,
        toDate,
        circleId,
        teacherId,
        attendanceFilter,
        pageNumber: 1,
        pageSize,
      }),
    )
  }, [])

  const report = reportQuery.data
  const items = report?.items ?? []
  const selectedCount = selectedRows.size
  const selectedRowData = useMemo(() => parseSelectedRows(selectedRows), [selectedRows])

  const applyFilters = (page = pageNumber, nextPageSize = pageSize) => {
    const error = validateReportDates(fromDate, toDate)
    if (error) {
      setValidationError(error)
      return
    }
    setValidationError(null)
    setSelectedRows(new Set())
    setAppliedFilters(
      buildAppliedFilters({
        fromDate,
        toDate,
        circleId,
        teacherId,
        attendanceFilter,
        pageNumber: page,
        pageSize: nextPageSize,
      }),
    )
  }

  const handleExport = () => {
    const error = validateExportDates(fromDate, toDate)
    if (error) {
      setValidationError(error)
      return
    }
    setValidationError(null)
    setAppliedFilters(
      buildAppliedFilters({
        fromDate,
        toDate,
        circleId,
        teacherId,
        attendanceFilter,
        pageNumber,
        pageSize,
      }),
    )
    exportMutation.mutate()
  }

  const handleClear = () => {
    const defaults = getDefaultDateRange()
    setFromDate(defaults.fromDate)
    setToDate(defaults.toDate)
    setCircleId('')
    setTeacherId('')
    setAttendanceFilter('all')
    setPageNumber(1)
    setPageSize(50)
    setSelectedRows(new Set())
    setValidationError(null)
    setAppliedFilters(null)
  }

  const toggleRow = (row: { studentId: number; date: string }) => {
    const key = getRowKey(row)
    setSelectedRows((prev) => {
      const next = new Set(prev)
      if (next.has(key)) next.delete(key)
      else next.add(key)
      return next
    })
  }

  const toggleAll = (rows: typeof items, checked: boolean) => {
    setSelectedRows((prev) => {
      const next = new Set(prev)
      rows.forEach((row) => {
        const key = getRowKey({ studentId: row.studentId, date: row.date })
        if (checked) next.add(key)
        else next.delete(key)
      })
      return next
    })
  }

  const handleDeparture = () => {
    const rows = selectedRowData
    if (rows.length === 0) {
      window.alert('يرجى تحديد السجلات المراد تسجيل انصرافها أولاً')
      return
    }
    if (!window.confirm(`هل أنت متأكد من تسجيل انصراف ${rows.length} طالب؟`)) return
    departureMutation.mutate(rows, {
      onSuccess: (result) => {
        window.alert(result.message)
        setSelectedRows(new Set())
      },
    })
  }

  const handleWhatsappSubmit = (payload: { message: string; image?: File | null }) => {
    const studentIds = uniqueStudentIdsFromRows(selectedRowData)
    if (studentIds.length === 0 || !payload.message.trim()) {
      window.alert('يرجى تحديد السجلات وكتابة الرسالة')
      return
    }
    whatsappMutation.mutate(
      { studentIds, message: payload.message, image: payload.image },
      {
        onSuccess: (message) => {
          window.alert(message)
          setWhatsappOpen(false)
          setSelectedRows(new Set())
        },
      },
    )
  }

  return (
    <div>
      <PageHeader
        title="تقرير الحضور والانصراف والغياب"
        description="تقرير شامل لحضور وانصراف وغياب الطلاب خلال فترة زمنية محددة"
        gradientClassName="bg-gradient-to-br from-[#7C8738] to-[#5f6830]"
      />

      <AttendanceReportFilters
        fromDate={fromDate}
        toDate={toDate}
        circleId={circleId}
        teacherId={teacherId}
        attendanceFilter={attendanceFilter}
        filterOptions={filterOptionsQuery.data}
        isExporting={exportMutation.isPending}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        onCircleIdChange={setCircleId}
        onTeacherIdChange={setTeacherId}
        onAttendanceFilterChange={setAttendanceFilter}
        onGenerate={() => applyFilters(1)}
        onExport={handleExport}
        onClear={handleClear}
        onQuickRange={(range) => {
          const next = getQuickDateRange(range)
          setFromDate(next.fromDate)
          setToDate(next.toDate)
        }}
      />

      {validationError && (
        <Alert variant="destructive" className="mb-4">
          {validationError}
        </Alert>
      )}

      {reportQuery.isLoading && (
        <div className="space-y-4">
          <Skeleton className="h-24 w-full" />
          <Skeleton className="h-64 w-full" />
        </div>
      )}

      {reportQuery.isError && (
        <Alert variant="destructive">تعذر تحميل التقرير. يرجى المحاولة مرة أخرى.</Alert>
      )}

      {report && report.items.length === 0 && !reportQuery.isLoading && appliedFilters && (
        <Alert>لا توجد بيانات للفترة المحددة. يرجى تجربة فترة زمنية أخرى.</Alert>
      )}

      {report && report.summary && report.items.length > 0 && (
        <>
          <div className="mb-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <SummaryCard label="إجمالي الأيام" value={String(report.summary.totalDays)} />
            <SummaryCard label="عدد الطلاب" value={String(report.summary.totalStudents)} />
            <SummaryCard label="إجمالي الحضور" value={String(report.summary.totalAttendance)} tone="success" />
            <SummaryCard label="إجمالي الانصراف" value={String(report.summary.totalDeparture)} tone="danger" />
          </div>

          <div className="mb-4 flex flex-wrap gap-2">
            <Button
              type="button"
              disabled={selectedCount === 0}
              onClick={() => setWhatsappOpen(true)}
            >
              <MessageCircle className="size-4" />
              إرسال واتساب ({selectedCount})
            </Button>
            <Button type="button" disabled={selectedCount === 0 || departureMutation.isPending} onClick={handleDeparture}>
              <LogOut className="size-4" />
              {departureMutation.isPending ? 'جاري الحفظ...' : `انصراف (${selectedCount})`}
            </Button>
            <Button type="button" variant="outline" onClick={() => toggleAll(items, true)}>
              تحديد الكل
            </Button>
            <Button type="button" variant="outline" onClick={() => setSelectedRows(new Set())}>
              إلغاء التحديد
            </Button>
          </div>

          <AttendanceReportTable
            items={items}
            selectedRows={selectedRows}
            pageNumber={report.pageNumber}
            pageSize={report.pageSize}
            totalCount={report.totalCount}
            totalPages={report.totalPages}
            onToggleRow={toggleRow}
            onToggleAll={toggleAll}
            onPageChange={(page) => {
              setPageNumber(page)
              applyFilters(page)
            }}
            onPageSizeChange={(size) => {
              setPageSize(size)
              setPageNumber(1)
              applyFilters(1, size)
            }}
          />
        </>
      )}

      <AttendanceWhatsappDialog
        open={whatsappOpen}
        selectedCount={selectedCount}
        isPending={whatsappMutation.isPending}
        canModify={canModify()}
        onOpenChange={setWhatsappOpen}
        onSubmit={handleWhatsappSubmit}
      />
    </div>
  )
}

function SummaryCard({
  label,
  value,
  tone = 'default',
}: {
  label: string
  value: string
  tone?: 'default' | 'success' | 'danger'
}) {
  const toneClass =
    tone === 'success' ? 'text-green-600' : tone === 'danger' ? 'text-red-600' : 'text-[#7C8738]'
  return (
    <div className="rounded-xl border bg-white p-4 text-center shadow-sm">
      <p className="text-sm text-slate-500">{label}</p>
      <p className={`mt-2 text-2xl font-bold ${toneClass}`}>{value}</p>
    </div>
  )
}
