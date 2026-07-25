import {
  formatCurrency,
  REQUIRED_ATTENDANCE_DAYS,
  type TeacherSalaryReportItem,
} from '@/types/teacherSalary'

interface TeacherSalaryReportTableProps {
  items: TeacherSalaryReportItem[]
}

export function TeacherSalaryReportTable({ items }: TeacherSalaryReportTableProps) {
  if (items.length === 0) {
    return <p className="py-8 text-center text-slate-500">لا توجد بيانات للتقرير</p>
  }

  return (
    <div className="overflow-x-auto rounded-xl bg-white shadow-md">
      <table className="w-full min-w-[720px] border-collapse">
        <thead className="bg-[var(--color-primary)] text-white">
          <tr>
            <th className="p-3 text-right">اسم المعلم</th>
            <th className="p-3 text-right">أيام الحضور / المطلوب</th>
            <th className="p-3 text-right">إجمالي الساعات</th>
            <th className="p-3 text-right">الراتب الأساسي</th>
            <th className="p-3 text-right">الخصومات</th>
            <th className="p-3 text-right">الراتب النهائي</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => {
            const complete = item.daysAttended >= REQUIRED_ATTENDANCE_DAYS
            return (
              <tr key={item.id} className="border-b hover:bg-slate-50">
                <td className="p-3">{item.teacherName}</td>
                <td className="p-3">
                  {item.daysAttended} / {REQUIRED_ATTENDANCE_DAYS}{' '}
                  <span
                    className={`rounded-full px-2 py-0.5 text-xs ${
                      complete ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                    }`}
                  >
                    {complete ? 'مكتمل' : 'ناقص'}
                  </span>
                </td>
                <td className="p-3">{item.totalHours.toFixed(2)} ساعة</td>
                <td className="p-3">{formatCurrency(item.baseSalary)}</td>
                <td className="p-3">
                  {item.deduction > 0 ? (
                    <span className="rounded-full bg-amber-100 px-2 py-0.5 text-amber-800">
                      {formatCurrency(item.deduction)}
                    </span>
                  ) : (
                    '0.00 د.ك'
                  )}
                </td>
                <td className="p-3 font-bold">{formatCurrency(item.calculatedSalary)}</td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
