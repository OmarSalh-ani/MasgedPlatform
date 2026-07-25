import { useMemo, useState } from 'react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import {
  useMemorizationRevisionReport,
  useMemorizationRevisionStudents,
} from '@/hooks/useMemorizationRevisionReport'
import { MemorizationRevisionReportFilters } from '@/pages/memorization-revision-report/MemorizationRevisionReportFilters'
import { MemorizationRevisionReportTable } from '@/pages/memorization-revision-report/MemorizationRevisionReportTable'

export function MemorizationRevisionReportPage() {
  const [studentId, setStudentId] = useState('')
  const selectedStudentId = useMemo(() => {
    const parsed = Number(studentId)
    return Number.isFinite(parsed) && parsed > 0 ? parsed : null
  }, [studentId])

  const studentsQuery = useMemorizationRevisionStudents()
  const { reportQuery, exportFullMutation, exportCompletedMutation } =
    useMemorizationRevisionReport(selectedStudentId)

  const handleExportFull = () => {
    if (!selectedStudentId) {
      window.alert('يرجى اختيار طالب أولاً.')
      return
    }
    exportFullMutation.mutate()
  }

  const handleExportCompleted = () => {
    if (!selectedStudentId) {
      window.alert('يرجى اختيار طالب أولاً.')
      return
    }
    exportCompletedMutation.mutate()
  }

  const students = studentsQuery.data ?? []
  const report = reportQuery.data
  const rows = report?.rows ?? []

  return (
    <div>
      <PageHeader
        title="تقرير الحفظ والمراجعة"
        description="اختر طالباً لعرض بيانات خطط الحفظ والمراجعة وتصديرها إلى Excel"
        gradientClassName="bg-gradient-to-br from-[#2c5aa0] to-[#1e3d6f]"
      />

      {studentsQuery.isLoading ? (
        <Skeleton className="mb-6 h-24 w-full" />
      ) : studentsQuery.isError ? (
        <Alert variant="destructive" className="mb-6">
          تعذر تحميل قائمة الطلاب.
        </Alert>
      ) : (
        <div className="mb-6">
          <MemorizationRevisionReportFilters
            studentId={studentId}
            students={students}
            isExportingFull={exportFullMutation.isPending}
            isExportingCompleted={exportCompletedMutation.isPending}
            onStudentIdChange={setStudentId}
            onExportFull={handleExportFull}
            onExportCompleted={handleExportCompleted}
          />
        </div>
      )}

      {!selectedStudentId && (
        <Alert>اختر طالباً من القائمة أعلاه لعرض التقرير.</Alert>
      )}

      {selectedStudentId && reportQuery.isLoading && (
        <Skeleton className="h-64 w-full" />
      )}

      {selectedStudentId && reportQuery.isError && (
        <Alert variant="destructive">تعذر تحميل التقرير. يرجى المحاولة مرة أخرى.</Alert>
      )}

      {selectedStudentId && report && rows.length === 0 && !reportQuery.isLoading && (
        <Alert>لا توجد بيانات لهذا الطالب.</Alert>
      )}

      {selectedStudentId && rows.length > 0 && (
        <div className="rounded-xl bg-white p-6 shadow-md">
          <MemorizationRevisionReportTable rows={rows} />
        </div>
      )}
    </div>
  )
}
