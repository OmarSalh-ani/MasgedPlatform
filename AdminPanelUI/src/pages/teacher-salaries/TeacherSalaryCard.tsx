import { Link } from 'react-router-dom'
import { Pencil, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
  formatCurrency,
  formatTeacherSalaryDate,
  getMonthName,
  REQUIRED_ATTENDANCE_DAYS,
  type TeacherSalaryListItem,
} from '@/types/teacherSalary'

interface TeacherSalaryCardProps {
  item: TeacherSalaryListItem
  selected: boolean
  canModify: boolean
  onSelectChange: (checked: boolean) => void
  onDelete: () => void
}

export function TeacherSalaryCard({
  item,
  selected,
  canModify,
  onSelectChange,
  onDelete,
}: TeacherSalaryCardProps) {
  const lowDays = item.daysAttended < REQUIRED_ATTENDANCE_DAYS

  return (
    <article
      className={`relative rounded-xl border-r-4 bg-white p-5 shadow-md transition hover:-translate-y-1 hover:shadow-lg ${
        selected ? 'border-r-emerald-500 bg-emerald-50/40' : 'border-r-[var(--color-primary)]'
      }`}
    >
      <input
        type="checkbox"
        className="absolute top-4 left-4 size-5 accent-[var(--color-primary)]"
        checked={selected}
        onChange={(e) => onSelectChange(e.target.checked)}
      />

      <div className="mb-4 border-b pb-4 pe-8">
        <h3 className="text-lg font-bold text-[var(--color-primary)]">{item.teacherName}</h3>
      </div>

      <div className="grid grid-cols-2 gap-4 text-sm">
        <Info label="الشهر / السنة" value={`${getMonthName(item.month)} / ${item.year}`} />
        <Info
          label="أيام الحضور"
          value={
            <>
              {item.daysAttended} / {REQUIRED_ATTENDANCE_DAYS}
              {lowDays && (
                <span className="mr-2 rounded-full bg-red-100 px-2 py-0.5 text-xs text-red-800">
                  ناقص
                </span>
              )}
            </>
          }
        />
        <Info label="إجمالي الساعات" value={`${item.totalHours} ساعة`} />
        <Info label="الراتب الأساسي" value={formatCurrency(item.baseSalary)} highlight />
        <Info label="الراتب المحسوب" value={formatCurrency(item.calculatedSalary)} highlight />
        <Info label="تاريخ الإنشاء" value={formatTeacherSalaryDate(item.createdAt)} />
      </div>

      {item.notes && (
        <div className="mt-4 rounded-lg bg-slate-50 p-3 text-sm">
          <strong>ملاحظات:</strong> {item.notes}
        </div>
      )}

      <div className="mt-4 flex flex-wrap gap-2 border-t pt-4">
        <Link to={`/teacher-salaries/${item.id}/edit`}>
          <Button type="button" variant="outline" className="px-3 py-1">
            <Pencil className="size-4" />
            تعديل
          </Button>
        </Link>
        {canModify && (
          <Button
            type="button"
            className="bg-red-600 px-3 py-1 text-white hover:bg-red-700"
            onClick={onDelete}
          >
            <Trash2 className="size-4" />
            حذف
          </Button>
        )}
      </div>
    </article>
  )
}

function Info({
  label,
  value,
  highlight,
}: {
  label: string
  value: React.ReactNode
  highlight?: boolean
}) {
  return (
    <div>
      <p className="text-slate-500">{label}</p>
      <p className={`font-semibold ${highlight ? 'text-[var(--color-primary)]' : 'text-slate-800'}`}>
        {value}
      </p>
    </div>
  )
}
