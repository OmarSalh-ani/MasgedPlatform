import { Button } from '@/components/ui/button'
import {
  formatReportDate,
  getRowKey,
  getStatusRowClass,
  type AttendanceReportRow,
  type SelectedAttendanceRow,
} from '@/types/attendanceReport'

interface AttendanceReportTableProps {
  items: AttendanceReportRow[]
  selectedRows: Set<string>
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  onToggleRow: (row: SelectedAttendanceRow) => void
  onToggleAll: (rows: AttendanceReportRow[], checked: boolean) => void
  onPageChange: (page: number) => void
  onPageSizeChange: (size: number) => void
}

export function AttendanceReportTable({
  items,
  selectedRows,
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  onToggleRow,
  onToggleAll,
  onPageChange,
  onPageSizeChange,
}: AttendanceReportTableProps) {
  const pageKeys = items.map((item) => getRowKey({ studentId: item.studentId, date: item.date }))
  const allSelected = pageKeys.length > 0 && pageKeys.every((key) => selectedRows.has(key))
  const startRecord = totalCount === 0 ? 0 : (pageNumber - 1) * pageSize + 1
  const endRecord = Math.min(pageNumber * pageSize, totalCount)

  return (
    <div>
      <div className="mb-3 flex flex-wrap gap-4 text-sm">
        <Legend colorClass="bg-red-200" label="غائب" />
        <Legend colorClass="bg-amber-200" label="حاضر ولم ينصرف" />
        <Legend colorClass="bg-green-200" label="حاضر وانصرف" />
        <Legend colorClass="bg-slate-200" label="اجازة" />
      </div>

      <div className="overflow-x-auto rounded-xl border bg-white shadow-sm">
        <table className="min-w-full text-sm">
          <thead className="bg-slate-50 text-[#7C8738]">
            <tr>
              <th className="px-3 py-3 text-right">
                <input
                  type="checkbox"
                  checked={allSelected}
                  onChange={(e) => onToggleAll(items, e.target.checked)}
                />
              </th>
              <th className="px-3 py-3 text-right">اسم الطالب</th>
              <th className="px-3 py-3 text-right">الحلقة</th>
              <th className="px-3 py-3 text-right">اسم المعلم</th>
              <th className="px-3 py-3 text-right">التاريخ</th>
              <th className="px-3 py-3 text-right">اليوم</th>
              <th className="px-3 py-3 text-right">الحضور</th>
              <th className="px-3 py-3 text-right">الانصراف</th>
              <th className="px-3 py-3 text-right">وقت الانصراف</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => {
              const rowKey = getRowKey({ studentId: item.studentId, date: item.date })
              return (
                <tr key={rowKey} className={`border-t ${getStatusRowClass(item.color)}`}>
                  <td className="px-3 py-2">
                    <input
                      type="checkbox"
                      checked={selectedRows.has(rowKey)}
                      onChange={() =>
                        onToggleRow({ studentId: item.studentId, date: item.date })
                      }
                    />
                  </td>
                  <td className="px-3 py-2">{item.studentName}</td>
                  <td className="px-3 py-2">{item.circleName}</td>
                  <td className="px-3 py-2">{item.teacherName || '-'}</td>
                  <td className="px-3 py-2">{formatReportDate(item.date)}</td>
                  <td className="px-3 py-2">{item.dayOfWeek}</td>
                  <td className="px-3 py-2">{item.status}</td>
                  <td className="px-3 py-2">{item.isDeparted ? 'انصرف' : 'لم ينصرف'}</td>
                  <td className="px-3 py-2">{item.departureTime || '-'}</td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <div className="mt-4 flex flex-wrap items-center justify-between gap-3">
          <p className="text-sm text-slate-600">
            عرض {startRecord}-{endRecord} من {totalCount} سجل
          </p>
          <div className="flex items-center gap-2">
            <Button
              type="button"
              variant="outline"
              className="h-8 px-3 text-sm"
              disabled={pageNumber <= 1}
              onClick={() => onPageChange(1)}
            >
              الأول
            </Button>
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
            <Button
              type="button"
              variant="outline"
              className="h-8 px-3 text-sm"
              disabled={pageNumber >= totalPages}
              onClick={() => onPageChange(totalPages)}
            >
              الأخير
            </Button>
          </div>
          <div className="flex items-center gap-2 text-sm">
            <label htmlFor="pageSize">عدد السجلات</label>
            <select
              id="pageSize"
              className="rounded border px-2 py-1"
              value={pageSize}
              onChange={(e) => onPageSizeChange(Number(e.target.value))}
            >
              {[25, 50, 100, 200].map((size) => (
                <option key={size} value={size}>
                  {size}
                </option>
              ))}
            </select>
          </div>
        </div>
      )}
    </div>
  )
}

function Legend({ colorClass, label }: { colorClass: string; label: string }) {
  return (
    <span className="inline-flex items-center gap-2">
      <span className={`inline-block size-3 rounded ${colorClass}`} />
      {label}
    </span>
  )
}
