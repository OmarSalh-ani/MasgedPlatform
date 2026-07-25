import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { calcPlanDays, todayIsoDate } from '@/types/studentPlan'

interface StudentPlanNewPlanPanelProps {
  onCreate: (name: string, fromDate: string, toDate: string) => void
  onCancel: () => void
  isPending: boolean
}

export function StudentPlanNewPlanPanel({ onCreate, onCancel, isPending }: StudentPlanNewPlanPanelProps) {
  const today = todayIsoDate()

  return (
    <div className="mb-6 rounded-xl bg-white p-6 shadow-md">
      <h3 className="mb-4 text-lg font-semibold">إضافة خطة جديدة</h3>
      <form
        className="space-y-3"
        onSubmit={(e) => {
          e.preventDefault()
          const fd = new FormData(e.currentTarget)
          onCreate(
            String(fd.get('name') ?? '').trim(),
            String(fd.get('fromDate') ?? today),
            String(fd.get('toDate') ?? today),
          )
        }}
      >
        <div>
          <label className="mb-1 block text-sm font-semibold">اسم الخطة</label>
          <Input name="name" required maxLength={200} placeholder="مثال: خطة 2026" className="max-w-xs" />
        </div>
        <div className="flex flex-wrap gap-4">
          <div>
            <label className="mb-1 block text-sm font-semibold">من تاريخ</label>
            <Input name="fromDate" type="date" defaultValue={today} className="max-w-[180px]" />
          </div>
          <div>
            <label className="mb-1 block text-sm font-semibold">إلى تاريخ</label>
            <Input name="toDate" type="date" defaultValue={today} className="max-w-[180px]" />
          </div>
        </div>
        <div className="flex gap-2 pt-2">
          <Button type="submit" disabled={isPending}>
            {isPending ? 'جاري الإنشاء...' : 'إنشاء الخطة'}
          </Button>
          <Button type="button" variant="outline" onClick={onCancel}>
            إلغاء
          </Button>
        </div>
      </form>
    </div>
  )
}

interface StudentPlanDateFieldsProps {
  memorizationLevel: string
  planStartDate: string
  planEndDate: string
  levels: string[]
  onLevelChange: (v: string) => void
  onStartChange: (v: string) => void
  onEndChange: (v: string) => void
}

export function StudentPlanDateFields({
  memorizationLevel,
  planStartDate,
  planEndDate,
  levels,
  onLevelChange,
  onStartChange,
  onEndChange,
}: StudentPlanDateFieldsProps) {
  const days = calcPlanDays(planStartDate, planEndDate)

  return (
    <div className="mb-4 flex flex-wrap gap-4">
      <div>
        <label className="mb-1 block text-sm font-semibold">مستوى الحفظ</label>
        <select
          className="min-w-[160px] rounded-lg border px-3 py-2 text-sm"
          value={memorizationLevel}
          onChange={(e) => onLevelChange(e.target.value)}
        >
          {levels.map((l) => (
            <option key={l} value={l}>
              {l}
            </option>
          ))}
        </select>
      </div>
      <div>
        <label className="mb-1 block text-sm font-semibold">تاريخ بداية الخطة</label>
        <Input type="date" value={planStartDate} onChange={(e) => onStartChange(e.target.value)} className="max-w-[160px]" />
      </div>
      <div>
        <label className="mb-1 block text-sm font-semibold">تاريخ نهاية الخطة</label>
        <Input type="date" value={planEndDate} onChange={(e) => onEndChange(e.target.value)} className="max-w-[160px]" />
      </div>
      <div>
        <label className="mb-1 block text-sm font-semibold">مدة الخطة</label>
        <span className="inline-block min-w-[80px] pt-2 text-sm">{days === null ? '-- يوم' : `${days} يوم`}</span>
      </div>
    </div>
  )
}
