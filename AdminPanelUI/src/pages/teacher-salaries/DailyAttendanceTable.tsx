import type { DailyAttendanceDetail } from '@/types/teacherSalary'

interface DailyAttendanceTableProps {
  details: DailyAttendanceDetail[]
}

export function DailyAttendanceTable({ details }: DailyAttendanceTableProps) {
  if (details.length === 0) return null

  return (
    <div className="mt-4 overflow-x-auto rounded-lg bg-slate-50 p-4">
      <h5 className="mb-3 font-semibold text-[var(--color-primary)]">تفاصيل الحضور اليومي:</h5>
      <table className="w-full min-w-[520px] border-collapse text-sm">
        <thead>
          <tr className="border-b bg-white">
            <th className="p-2 text-right">التاريخ</th>
            <th className="p-2 text-right">وقت الحضور</th>
            <th className="p-2 text-right">وقت الانصراف</th>
            <th className="p-2 text-right">الساعات</th>
            <th className="p-2 text-right">الحالة</th>
          </tr>
        </thead>
        <tbody>
          {details.map((detail) => (
            <tr key={detail.date + detail.attendanceTime} className="border-b">
              <td className="p-2">{detail.dateFormatted}</td>
              <td className="p-2">{detail.attendanceTime}</td>
              <td className="p-2">{detail.departureTime}</td>
              <td className="p-2">{detail.hours.toFixed(2)}</td>
              <td className="p-2">
                {detail.isValid ? (
                  <span className="text-green-700">✓ صالح</span>
                ) : (
                  <span className="text-red-600">✗ أقل من 1.5 ساعة</span>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
