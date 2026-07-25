import { useEffect, useState } from 'react'
import { CalendarCheck } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import {
  useTeachersAttendance,
  useTeachersAttendanceFilterOptions,
} from '@/hooks/useTeachersAttendance'
import { TeachersAttendanceFilters } from '@/pages/teachers-attendance/TeachersAttendanceFilters'
import { TeachersAttendanceTable } from '@/pages/teachers-attendance/TeachersAttendanceTable'
import {
  getDefaultTeachersAttendanceFilters,
  type TeachersAttendanceFilters as Filters,
} from '@/types/teachersAttendance'

export function TeachersAttendancePage() {
  const defaults = getDefaultTeachersAttendanceFilters()
  const [fromDate, setFromDate] = useState(defaults.fromDate)
  const [toDate, setToDate] = useState(defaults.toDate)
  const [teacherId, setTeacherId] = useState(defaults.teacherId)
  const [appliedFilters, setAppliedFilters] = useState<Filters | null>(defaults)

  const filterOptionsQuery = useTeachersAttendanceFilterOptions()
  const { listQuery, exportMutation } = useTeachersAttendance(appliedFilters)

  useEffect(() => {
    setAppliedFilters(defaults)
  }, [])

  const applyFilters = () => {
    setAppliedFilters({ fromDate, toDate, teacherId })
  }

  const report = listQuery.data

  return (
    <div>
      <PageHeader
        title="حضور المعلمين"
        description="تتبع ومراقبة حضور المعلمين"
        gradientClassName="bg-gradient-to-br from-[#7C8738] to-[#1a5f8a]"
      />

      <TeachersAttendanceFilters
        fromDate={fromDate}
        toDate={toDate}
        teacherId={teacherId}
        filterOptions={filterOptionsQuery.data}
        isLoading={listQuery.isFetching}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        onTeacherIdChange={setTeacherId}
        onApply={applyFilters}
      />

      {listQuery.isLoading && (
        <div className="space-y-4">
          <Skeleton className="h-24 w-full" />
          <Skeleton className="h-64 w-full" />
        </div>
      )}

      {listQuery.isError && (
        <Alert variant="destructive">حدث خطأ أثناء تحميل البيانات. يرجى المحاولة مرة أخرى.</Alert>
      )}

      {report && (
        <section>
          <h2 className="mb-4 flex items-center justify-center gap-2 text-lg font-semibold text-[#7C8738]">
            <CalendarCheck className="size-5" />
            سجل الحضور
          </h2>
          <TeachersAttendanceTable
            items={report.items}
            onExport={() => exportMutation.mutate()}
            isExporting={exportMutation.isPending}
          />
        </section>
      )}
    </div>
  )
}
