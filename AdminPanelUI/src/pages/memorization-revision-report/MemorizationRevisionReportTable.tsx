import type { MemorizationRevisionPlanRow } from '@/types/memorizationRevisionReport'

interface MemorizationRevisionReportTableProps {
  rows: MemorizationRevisionPlanRow[]
}

export function MemorizationRevisionReportTable({ rows }: MemorizationRevisionReportTableProps) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-right text-sm">
        <thead>
          <tr className="border-b bg-[#2c5aa0] text-white">
            <th className="px-4 py-3 text-center font-semibold">الحالة</th>
            <th className="px-4 py-3 text-center font-semibold">أسم السورة</th>
            <th className="px-4 py-3 text-center font-semibold">الطالب</th>
            <th className="px-4 py-3 text-center font-semibold">من الآية</th>
            <th className="px-4 py-3 text-center font-semibold">إلى الآية</th>
            <th className="px-4 py-3 text-center font-semibold">نوع الخطة</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => (
            <tr key={`${row.planType}-${row.surahNameAr}-${row.fromAyah}-${index}`} className="border-b">
              <td className="px-4 py-3 text-center">{row.status}</td>
              <td className="px-4 py-3 text-center">{row.surahNameAr}</td>
              <td className="px-4 py-3 text-center">{row.studentName}</td>
              <td className="px-4 py-3 text-center">{row.fromAyah}</td>
              <td className="px-4 py-3 text-center">{row.toAyah}</td>
              <td className="px-4 py-3 text-center">{row.planType}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
