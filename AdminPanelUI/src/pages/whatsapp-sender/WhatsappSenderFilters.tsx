import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { HomeFilterOptions } from '@/types/home'
import type { WhatsappSenderFilterForm } from '@/types/whatsappSender'

interface WhatsappSenderFiltersProps {
  form: WhatsappSenderFilterForm
  options?: HomeFilterOptions
  onChange: (form: WhatsappSenderFilterForm) => void
  onApply: () => void
  onClear: () => void
}

export function WhatsappSenderFilters({
  form,
  options,
  onChange,
  onApply,
  onClear,
}: WhatsappSenderFiltersProps) {
  const set = <K extends keyof WhatsappSenderFilterForm>(key: K, value: WhatsappSenderFilterForm[K]) =>
    onChange({ ...form, [key]: value })

  return (
    <section className="rounded-xl border bg-white p-5 shadow-sm">
      <h2 className="mb-4 text-lg font-semibold text-[var(--color-primary)]">فلاتر البحث</h2>
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        <Field label="اسم الطالب">
          <Input value={form.studentName} placeholder="ابحث باسم الطالب..." onChange={(e) => set('studentName', e.target.value)} />
        </Field>
        <Field label="العمر من">
          <Input type="number" value={form.ageFrom} placeholder="من" onChange={(e) => set('ageFrom', e.target.value)} />
        </Field>
        <Field label="العمر إلى">
          <Input type="number" value={form.ageTo} placeholder="إلى" onChange={(e) => set('ageTo', e.target.value)} />
        </Field>
        <Field label="الحلقة">
          <select className="w-full rounded-md border px-3 py-2" value={form.circleId} onChange={(e) => set('circleId', e.target.value)}>
            <option value="">جميع الحلقات</option>
            {(options?.circles ?? []).map((circle) => (
              <option key={circle.id} value={circle.id}>{circle.name}</option>
            ))}
          </select>
        </Field>
        <Field label="هاتف ولي الأمر">
          <Input value={form.fatherMobile} placeholder="ابحث برقم الهاتف..." onChange={(e) => set('fatherMobile', e.target.value)} />
        </Field>
        <Field label="حالة الاستمارة">
          <select className="w-full rounded-md border px-3 py-2" value={form.formStatus} onChange={(e) => set('formStatus', e.target.value)}>
            <option value="">جميع الاستمارات</option>
            <option value="نعم">الاستمارات المكتملة</option>
            <option value="لا">الاستمارات غير المكتملة</option>
          </select>
        </Field>
      </div>

      <div className="mt-4 flex flex-wrap gap-4 text-sm">
        <Toggle checked={form.specialOnly} label="الطلاب المميزين فقط" onChange={(v) => set('specialOnly', v)} />
        <Toggle checked={form.boysOnly} label="الذكور فقط" onChange={(v) => set('boysOnly', v)} />
        <Toggle checked={form.girlsOnly} label="الإناث فقط" onChange={(v) => set('girlsOnly', v)} />
      </div>

      <div className="mt-4 flex flex-wrap justify-end gap-2">
        <Button type="button" variant="outline" onClick={onClear}>مسح الفلاتر</Button>
        <Button type="button" onClick={onApply}>تطبيق الفلاتر</Button>
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

function Toggle({
  checked,
  label,
  onChange,
}: {
  checked: boolean
  label: string
  onChange: (value: boolean) => void
}) {
  return (
    <label className="flex items-center gap-2">
      <input type="checkbox" checked={checked} onChange={(e) => onChange(e.target.checked)} />
      <span>{label}</span>
    </label>
  )
}
