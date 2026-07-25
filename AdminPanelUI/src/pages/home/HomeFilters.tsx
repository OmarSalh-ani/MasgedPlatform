import { FileSpreadsheet, Filter } from 'lucide-react'
import { SearchableDropdown } from '@/components/shared/SearchableDropdown'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { HomeStudentNameDropdown } from '@/pages/home/HomeStudentNameDropdown'
import type { HomeFilterFormState } from '@/pages/home/homeUtils'
import { toHomeLookupDropdownOptions } from '@/pages/home/homeUtils'
import type { HomeFilterOptions } from '@/types/home'
import { HOME_FORM_STATUS_OPTIONS } from '@/types/home'

interface HomeFiltersProps {
  form: HomeFilterFormState
  options?: HomeFilterOptions
  isGirlTeacher: boolean
  isExporting: boolean
  onChange: (next: HomeFilterFormState) => void
  onApply: () => void
  onClear: () => void
  onExport: () => void
}

export function HomeFilters({
  form,
  options,
  isGirlTeacher,
  isExporting,
  onChange,
  onApply,
  onClear,
  onExport,
}: HomeFiltersProps) {
  const update = <K extends keyof HomeFilterFormState>(key: K, value: HomeFilterFormState[K]) =>
    onChange({ ...form, [key]: value })

  return (
    <section className="rounded-xl border bg-white p-5 shadow-sm">
      <h2 className="mb-4 flex items-center gap-2 text-lg font-semibold text-[var(--color-primary)]">
        <Filter className="size-5" />
        فلاتر البحث
      </h2>

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        <Field label="اسم الطالب">
          <HomeStudentNameDropdown value={form.studentName} onChange={(value) => update('studentName', value)} />
        </Field>
        <Field label="العمر">
          <div className="flex items-center gap-2">
            <Input
              type="number"
              value={form.ageFrom}
              placeholder="من"
              onChange={(event) => update('ageFrom', event.target.value)}
            />
            <span className="shrink-0 text-sm text-slate-500">إلى</span>
            <Input
              type="number"
              value={form.ageTo}
              placeholder="إلى"
              onChange={(event) => update('ageTo', event.target.value)}
            />
          </div>
        </Field>
        <Field label="الحلقة">
          <SearchableDropdown
            value={form.circleId}
            onChange={(value) => update('circleId', value)}
            options={toHomeLookupDropdownOptions(options?.circles, 'جميع الحلقات')}
            placeholder="جميع الحلقات"
          />
        </Field>
        <Field label="هاتف ولي الأمر">
          <Input value={form.fatherMobile} placeholder="ابحث برقم الهاتف..." onChange={(e) => update('fatherMobile', e.target.value)} />
        </Field>
        {isGirlTeacher && (
          <Field label="نوع التسجيل">
            <SearchableDropdown
              value={form.womanActivityTypeId}
              onChange={(value) => update('womanActivityTypeId', value)}
              options={toHomeLookupDropdownOptions(options?.womanActivityTypes, 'جميع الأنواع')}
              placeholder="جميع الأنواع"
            />
          </Field>
        )}
        <Field label="حالة الاستمارة">
          <SearchableDropdown
            value={form.formStatus}
            onChange={(value) => update('formStatus', value)}
            options={HOME_FORM_STATUS_OPTIONS}
            placeholder="جميع الاستمارات"
          />
        </Field>
      </div>

      <div className="mt-4 flex flex-wrap gap-4">
        <Toggle checked={form.specialOnly} label="الطلاب المميزين فقط" onChange={(v) => update('specialOnly', v)} />
        <Toggle checked={form.eliteOnly} label="طلاب النخبة فقط" onChange={(v) => update('eliteOnly', v)} />
        <Toggle checked={form.boysOnly} label="الذكور فقط" onChange={(v) => update('boysOnly', v)} />
        <Toggle checked={form.girlsOnly} label="الإناث فقط" onChange={(v) => update('girlsOnly', v)} />
      </div>

      <div className="mt-5 flex flex-wrap justify-end gap-2">
        <Button type="button" variant="outline" onClick={onClear}>مسح الفلاتر</Button>
        <Button type="button" onClick={onApply}>تطبيق الفلاتر</Button>
        <Button type="button" variant="outline" disabled={isExporting} onClick={onExport}>
          <FileSpreadsheet className="size-4" />
          {isExporting ? 'جاري التصدير...' : 'تصدير Excel'}
        </Button>
      </div>
    </section>
  )
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="space-y-1">
      <Label>{label}</Label>
      {children}
    </div>
  )
}

function Toggle({ checked, label, onChange }: { checked: boolean; label: string; onChange: (value: boolean) => void }) {
  return (
    <label className="flex items-center gap-2 text-sm">
      <input type="checkbox" checked={checked} onChange={(e) => onChange(e.target.checked)} />
      {label}
    </label>
  )
}
