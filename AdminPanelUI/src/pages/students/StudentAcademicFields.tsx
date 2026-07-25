import type { Control } from 'react-hook-form'
import { SearchableDropdown } from '@/components/shared/SearchableDropdown'
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import type { StudentFormValues } from '@/pages/students/studentFormSchema'
import type { StudentLookupOption } from '@/types/student'

function toDropdownOptions(items: StudentLookupOption[]) {
  return items.map((item) => ({ value: String(item.id), label: item.name }))
}

interface Props {
  control: Control<StudentFormValues>
  readOnly: boolean
  circles: StudentLookupOption[]
  planLevels: StudentLookupOption[]
  registrationDate: string
}

export function StudentAcademicFields({
  control,
  readOnly,
  circles,
  planLevels,
  registrationDate,
}: Props) {
  return (
    <>
      <div className="grid gap-4 md:grid-cols-2">
        <FormField
          control={control}
          name="quranCircleId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>الحلقة *</FormLabel>
              <FormControl>
                <SearchableDropdown
                  disabled={readOnly}
                  options={toDropdownOptions(circles)}
                  placeholder="اختر الحلقة"
                  value={field.value ?? ''}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  name={field.name}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={control}
          name="planLevelId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>مستوى الطالب</FormLabel>
              <FormControl>
                <SearchableDropdown
                  disabled={readOnly}
                  options={toDropdownOptions(planLevels)}
                  placeholder="اختر المستوى"
                  value={field.value ?? ''}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  name={field.name}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>

      <FormField
        control={control}
        name="parentPanelPassword"
        render={({ field }) => (
          <FormItem>
            <FormLabel>كلمة مرور لوحة ولي الأمر</FormLabel>
            <FormControl>
              <Input placeholder="أدخل كلمة المرور" disabled={readOnly} {...field} />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />

      <FormItem>
        <FormLabel>تاريخ التسجيل</FormLabel>
        <FormControl>
          <Input type="date" value={registrationDate} disabled readOnly />
        </FormControl>
      </FormItem>

      <FormField
        control={control}
        name="isSpecial"
        render={({ field }) => (
          <FormItem>
            <FormLabel>هل هو طالب مميز؟</FormLabel>
            <FormControl>
              <label className="flex items-center gap-2">
                <input
                  type="checkbox"
                  checked={field.value}
                  disabled={readOnly}
                  onChange={(event) => field.onChange(event.target.checked)}
                />
                <span>نعم</span>
              </label>
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />
    </>
  )
}
