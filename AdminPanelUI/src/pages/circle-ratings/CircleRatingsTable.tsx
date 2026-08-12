import { FileText } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { formatVisitDate, type CircleVisitRatingListItem } from '@/types/circleVisitRating'

interface CircleRatingsTableProps {
  items: CircleVisitRatingListItem[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  exportingId: number | null
  onPageChange: (page: number) => void
  onExportPdf: (id: number) => void
}

export function CircleRatingsTable({
  items,
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  exportingId,
  onPageChange,
  onExportPdf,
}: CircleRatingsTableProps) {
  const from = totalCount === 0 ? 0 : (pageNumber - 1) * pageSize + 1
  const to = Math.min(pageNumber * pageSize, totalCount)

  return (
    <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
      {items.length === 0 ? (
        <p className="px-4 py-12 text-center text-sm text-slate-500">لا توجد تقييمات مسجلة</p>
      ) : (
        <>
          <div className="overflow-x-auto">
            <table className="min-w-full text-sm">
              <thead>
                <tr className="border-b border-slate-200 bg-slate-50">
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">#</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">المعلم</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">الحلقة</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">تاريخ الزيارة</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">الوقت</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">رقم الزيارة</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">بواسطة</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">تقرير PDF</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item, index) => (
                  <tr
                    key={item.id}
                    className="border-b border-slate-100 transition-colors last:border-0 hover:bg-slate-50/80"
                  >
                    <td className="px-4 py-3 text-right tabular-nums text-slate-600">
                      {from + index}
                    </td>
                    <td className="px-4 py-3 text-right text-slate-800">{item.teacherName}</td>
                    <td className="px-4 py-3 text-right text-slate-800">{item.circleName}</td>
                    <td className="px-4 py-3 text-right tabular-nums text-slate-600">
                      {formatVisitDate(item.visitDate)}
                    </td>
                    <td className="px-4 py-3 text-right tabular-nums text-slate-600">
                      {item.visitTime}
                    </td>
                    <td className="px-4 py-3 text-right tabular-nums text-slate-600">
                      {item.visitNumberInMonth}
                    </td>
                    <td className="px-4 py-3 text-right text-slate-800">{item.createdByName}</td>
                    <td className="px-4 py-3 text-right">
                      <Button
                        type="button"
                        variant="outline"
                        className="gap-1.5 px-3 py-1.5 text-xs"
                        disabled={exportingId === item.id}
                        onClick={() => onExportPdf(item.id)}
                      >
                        <FileText className="size-3.5" strokeWidth={1.5} absoluteStrokeWidth />
                        {exportingId === item.id ? 'جاري...' : 'PDF'}
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="flex flex-wrap items-center justify-between gap-3 border-t border-slate-100 px-4 py-3 text-sm text-slate-600">
            <span>
              عرض {from}–{to} من {totalCount}
            </span>
            <div className="flex gap-2">
              <Button
                type="button"
                variant="outline"
                disabled={pageNumber <= 1}
                onClick={() => onPageChange(pageNumber - 1)}
              >
                السابق
              </Button>
              <Button
                type="button"
                variant="outline"
                disabled={pageNumber >= totalPages}
                onClick={() => onPageChange(pageNumber + 1)}
              >
                التالي
              </Button>
            </div>
          </div>
        </>
      )}
    </div>
  )
}
