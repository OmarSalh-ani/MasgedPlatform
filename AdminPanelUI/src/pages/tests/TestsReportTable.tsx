import { Button } from '@/components/ui/button'
import type { TestsReportRow } from '@/types/testsReport'

interface TestsReportTableProps {
  items: TestsReportRow[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  onPageChange: (page: number) => void
}

export function TestsReportTable({
  items,
  totalCount,
  pageNumber,
  pageSize,
  totalPages,
  onPageChange,
}: TestsReportTableProps) {
  const startRecord = totalCount === 0 ? 0 : (pageNumber - 1) * pageSize + 1
  const endRecord = Math.min(pageNumber * pageSize, totalCount)

  return (
    <div>
      <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm">
            <thead>
              <tr className="border-b border-slate-200 bg-slate-50">
                <th className="px-4 py-3 text-right font-semibold text-slate-700">رقم الطالب</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">اسم الطالب</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">هاتف الوالد</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">اسم المعلم</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">اسم الحلقة</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">نوع البرنامج</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">من</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">إلى</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">تاريخ الاختبار</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">النتيجة النهائية</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">ملاحظات</th>
                <th className="px-4 py-3 text-right font-semibold text-slate-700">نوع الاختبار</th>
              </tr>
            </thead>
            <tbody>
              {items.map((row) => (
                <tr
                  key={`${row.studentId}-${row.testDate}-${row.testFrom}`}
                  className="border-b border-slate-100 transition-colors last:border-0 hover:bg-slate-50/80"
                >
                  <td className="px-4 py-3 text-right tabular-nums text-slate-800">{row.studentId}</td>
                  <td className="px-4 py-3 text-right text-slate-800">{row.studentName}</td>
                  <td className="px-4 py-3 text-right text-slate-600">{row.parentPhone}</td>
                  <td className="px-4 py-3 text-right text-slate-800">{row.teacherName}</td>
                  <td className="px-4 py-3 text-right text-slate-800">{row.circleName}</td>
                  <td className="px-4 py-3 text-right text-slate-600">{row.programType}</td>
                  <td className="px-4 py-3 text-right text-slate-600">{row.testFrom}</td>
                  <td className="px-4 py-3 text-right text-slate-600">{row.testTo}</td>
                  <td className="px-4 py-3 text-right tabular-nums text-slate-600">{row.testDate}</td>
                  <td className="px-4 py-3 text-right tabular-nums text-slate-800">{row.finalResults}</td>
                  <td className="px-4 py-3 text-right text-slate-600">{row.notes}</td>
                  <td className="px-4 py-3 text-right text-slate-600">{row.testType}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {totalPages > 1 && (
        <div className="mt-4 flex flex-wrap items-center justify-between gap-3">
          <p className="text-sm text-slate-600">
            عرض {startRecord}-{endRecord} من {totalCount} اختبار
          </p>
          <div className="flex items-center gap-2">
            <Button
              type="button"
              variant="outline"
              className="h-8 px-3 text-sm"
              disabled={pageNumber <= 1}
              onClick={() => onPageChange(pageNumber - 1)}
            >
              السابق
            </Button>
            <span className="px-2 text-sm">
              {pageNumber} / {totalPages}
            </span>
            <Button
              type="button"
              variant="outline"
              className="h-8 px-3 text-sm"
              disabled={pageNumber >= totalPages}
              onClick={() => onPageChange(pageNumber + 1)}
            >
              التالي
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
