import { FileSpreadsheet, Info, Printer, Star } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { useSpecialStudentsReport } from '@/hooks/useSpecialStudentsReport'
import { SpecialStudentsReportCards } from '@/pages/special-students-report/SpecialStudentsReportCards'

export function SpecialStudentsReportPage() {
  const { reportQuery, exportMutation } = useSpecialStudentsReport()

  const handleExport = () => {
    exportMutation.mutate(undefined, {
      onSuccess: () => {
        const stats = reportQuery.data?.stats
        if (stats) {
          window.alert(
            `تم تصدير ${stats.totalStudents} طالب مميز من ${stats.totalCircles} حلقة بنجاح!`,
          )
        }
      },
    })
  }

  if (reportQuery.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-40 w-full" />
        <Skeleton className="h-32 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (reportQuery.isError) {
    return (
      <Alert variant="destructive">حدث خطأ أثناء تحميل التقرير. يرجى المحاولة مرة أخرى.</Alert>
    )
  }

  const report = reportQuery.data
  const items = report?.items ?? []
  const stats = report?.stats
  const hasStudents = items.length > 0

  return (
    <div>
      <PageHeader
        title={
          <>
            <Star className="inline size-8 fill-[#ffd700] text-[#ffd700]" />
            {' '}
            تقرير الطلاب المميزين - جميع الحلقات
          </>
        }
        description="قائمة شاملة بجميع الطلاب المميزين من كافة حلقات المسجد"
        gradientClassName="bg-gradient-to-br from-[#2c5aa0] to-[#1e3d6f]"
        className="print:[&_button]:hidden"
      >
        <div className="mt-5 flex flex-wrap items-center justify-center gap-3">
          <Button
            type="button"
            className="bg-emerald-600 px-6 py-3 hover:bg-emerald-700"
            disabled={exportMutation.isPending || !hasStudents}
            onClick={handleExport}
          >
            <FileSpreadsheet className="size-4" />
            {exportMutation.isPending ? 'جاري التصدير...' : 'تصدير Excel'}
          </Button>
          <Button
            type="button"
            variant="outline"
            className="border-white/30 bg-white/10 px-6 py-3 text-white hover:bg-white/20"
            onClick={() => window.print()}
          >
            <Printer className="size-4" />
            طباعة التقرير
          </Button>
        </div>
      </PageHeader>

      {hasStudents && stats && (
        <div className="mb-6 grid gap-4 sm:grid-cols-3 print:hidden">
          <StatsCard label="إجمالي الطلاب المميزين" value={String(stats.totalStudents)} />
          <StatsCard label="عدد الحلقات" value={String(stats.totalCircles)} />
          <StatsCard
            label="متوسط الطلاب لكل حلقة"
            value={stats.averagePerCircle.toFixed(1)}
          />
        </div>
      )}

      {hasStudents ? (
        <SpecialStudentsReportCards items={items} />
      ) : (
        <div className="rounded-2xl py-16 text-center text-slate-500">
          <Info className="mx-auto mb-4 size-16 text-slate-300" />
          <h4 className="text-xl font-semibold">لا يوجد طلاب مميزين حالياً</h4>
          <p className="mt-2">لم يتم العثور على أي طلاب مميزين في جميع حلقات المسجد.</p>
        </div>
      )}
    </div>
  )
}

function StatsCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-2xl bg-gradient-to-br from-emerald-500 to-teal-500 p-5 text-center text-white shadow-md">
      <p className="text-4xl font-bold">{value}</p>
      <p className="mt-1 text-lg opacity-90">{label}</p>
    </div>
  )
}
