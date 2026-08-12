import { useMemo, useState } from 'react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import {
  useCircleMemorizationRevisionExport,
  useCircleMemorizationTeachers,
} from '@/hooks/useCircleMemorizationRevisionReport'
import { CircleMemorizationRevisionReportFilters } from '@/pages/circle-memorization-revision-report/CircleMemorizationRevisionReportFilters'
import type { CircleReportExportFormat } from '@/types/circleMemorizationRevisionReport'

function defaultDateRange() {
  const to = new Date()
  const from = new Date()
  from.setDate(to.getDate() - 7)
  const fmt = (d: Date) => d.toISOString().slice(0, 10)
  return { fromDate: fmt(from), toDate: fmt(to) }
}

export function CircleMemorizationRevisionReportPage() {
  const initial = useMemo(() => defaultDateRange(), [])
  const [teacherId, setTeacherId] = useState('')
  const [fromDate, setFromDate] = useState(initial.fromDate)
  const [toDate, setToDate] = useState(initial.toDate)
  const [format, setFormat] = useState<CircleReportExportFormat>('pdf')

  const teachersQuery = useCircleMemorizationTeachers()
  const exportMutation = useCircleMemorizationRevisionExport()

  const selectedTeacherId = useMemo(() => {
    const parsed = Number(teacherId)
    return Number.isFinite(parsed) && parsed > 0 ? parsed : null
  }, [teacherId])

  const handleExport = () => {
    if (!selectedTeacherId) {
      window.alert('يرجى اختيار معلم أولاً.')
      return
    }
    if (!fromDate || !toDate) {
      window.alert('يرجى تحديد من تاريخ والى تاريخ.')
      return
    }
    if (toDate < fromDate) {
      window.alert('تاريخ النهاية يجب أن يكون بعد أو يساوي تاريخ البداية.')
      return
    }
    exportMutation.mutate({
      teacherId: selectedTeacherId,
      fromDate,
      toDate,
      format,
    })
  }

  return (
    <div>
      <PageHeader
        title="تقرير الحفظ والمراجعة للحلقة"
        description="اختر معلماً وفترة زمنية لتوليد تقرير الحفظ والمراجعة (تم الحفظ / تم المراجعة فقط)"
        gradientClassName="bg-gradient-to-br from-[#2c5aa0] to-[#1e3d6f]"
      />

      {teachersQuery.isLoading ? (
        <Skeleton className="mb-6 h-40 w-full" />
      ) : teachersQuery.isError ? (
        <Alert variant="destructive" className="mb-6">
          تعذر تحميل قائمة المعلمين.
        </Alert>
      ) : (
        <div className="mb-6">
          <CircleMemorizationRevisionReportFilters
            teacherId={teacherId}
            fromDate={fromDate}
            toDate={toDate}
            format={format}
            teachers={teachersQuery.data ?? []}
            isExporting={exportMutation.isPending}
            onTeacherIdChange={setTeacherId}
            onFromDateChange={setFromDate}
            onToDateChange={setToDate}
            onFormatChange={setFormat}
            onExport={handleExport}
          />
        </div>
      )}

      {!selectedTeacherId && (
        <Alert>اختر معلماً وحدد الفترة ثم اضغط توليد التقرير.</Alert>
      )}
    </div>
  )
}
