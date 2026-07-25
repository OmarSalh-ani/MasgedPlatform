import { List } from 'lucide-react'
import { StatisticsSection } from '@/pages/statistics/StatisticsSection'
import type { ParentPanelLogEntry } from '@/types/parentPanelLogStatistics'

interface ParentPanelLogStatisticsTableProps {
  entries: ParentPanelLogEntry[]
}

export function ParentPanelLogStatisticsTable({ entries }: ParentPanelLogStatisticsTableProps) {
  return (
    <StatisticsSection
      title="سجل الدخول"
      description="تفاصيل دخول أولياء الأمور مرتبة حسب التاريخ والوقت"
      icon={List}
      columns={4}
    >
      <div className="col-span-2 overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm xl:col-span-4">
        {entries.length === 0 ? (
          <p className="px-4 py-12 text-center text-sm text-slate-500">لا توجد بيانات</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full text-sm">
              <thead>
                <tr className="border-b border-slate-200 bg-slate-50">
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">رقم الهاتف</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">اسم الطالب</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">رقم الطالب</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">تاريخ الدخول</th>
                  <th className="px-4 py-3 text-right font-semibold text-slate-700">وقت الدخول</th>
                </tr>
              </thead>
              <tbody>
                {entries.map((entry) => (
                  <tr
                    key={`${entry.parentMobile}-${entry.studentId}-${entry.accessDate}-${entry.accessTime}`}
                    className="border-b border-slate-100 transition-colors last:border-0 hover:bg-slate-50/80"
                  >
                    <td className="px-4 py-3 text-right tabular-nums text-slate-800">
                      {entry.parentMobile}
                    </td>
                    <td className="px-4 py-3 text-right text-slate-800">{entry.studentName}</td>
                    <td className="px-4 py-3 text-right tabular-nums text-slate-800">
                      {entry.studentId.toLocaleString('ar-EG')}
                    </td>
                    <td className="px-4 py-3 text-right tabular-nums text-slate-600">
                      {entry.accessDate}
                    </td>
                    <td className="px-4 py-3 text-right tabular-nums text-slate-600">
                      {entry.accessTime}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </StatisticsSection>
  )
}
