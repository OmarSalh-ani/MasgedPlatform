import type { Control } from 'react-hook-form'
import { Button } from '@/components/ui/button'
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { EVENT_PAGE_FIELD_TYPES } from '@/types/eventPage'
import type { EventPageFormValues } from '@/pages/event-pages/eventPageFormSchema'

interface EventPageFormFieldRowProps {
  control: Control<EventPageFormValues>
  index: number
  onRemove: () => void
}

export function EventPageFormFieldRow({ control, index, onRemove }: EventPageFormFieldRowProps) {
  return (
    <div className="space-y-3 rounded-xl border border-slate-200 p-4">
      <div className="flex items-center justify-between">
        <span className="text-sm font-medium text-slate-600">حقل {index + 1}</span>
        <Button type="button" variant="outline" className="text-red-600" onClick={onRemove}>
          حذف
        </Button>
      </div>
      <div className="grid gap-3 md:grid-cols-2">
        <FormField
          control={control}
          name={`formFields.${index}.label`}
          render={({ field }) => (
            <FormItem>
              <FormLabel>عنوان الحقل *</FormLabel>
              <FormControl>
                <Input maxLength={300} {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={control}
          name={`formFields.${index}.fieldType`}
          render={({ field }) => (
            <FormItem>
              <FormLabel>النوع</FormLabel>
              <FormControl>
                <select
                  className="w-full rounded-lg border border-slate-200 px-3 py-2"
                  value={field.value}
                  onChange={field.onChange}
                >
                  {EVENT_PAGE_FIELD_TYPES.map((type) => (
                    <option key={type.value} value={type.value}>
                      {type.label}
                    </option>
                  ))}
                </select>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={control}
          name={`formFields.${index}.isRequired`}
          render={({ field }) => (
            <FormItem>
              <FormLabel>مطلوب</FormLabel>
              <FormControl>
                <select
                  className="w-full rounded-lg border border-slate-200 px-3 py-2"
                  value={field.value ? 'true' : 'false'}
                  onChange={(event) => field.onChange(event.target.value === 'true')}
                >
                  <option value="true">نعم</option>
                  <option value="false">لا</option>
                </select>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={control}
          name={`formFields.${index}.sortOrder`}
          render={({ field }) => (
            <FormItem>
              <FormLabel>ترتيب العرض</FormLabel>
              <FormControl>
                <Input
                  type="number"
                  value={field.value}
                  onChange={(event) => field.onChange(event.target.valueAsNumber || 0)}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
      <FormField
        control={control}
        name={`formFields.${index}.optionsText`}
        render={({ field }) => (
          <FormItem>
            <FormLabel>الخيارات (سطر لكل خيار — للاختيار الواحد/المتعدد)</FormLabel>
            <FormControl>
              <Textarea rows={3} placeholder={'الخيار الأول\nالخيار الثاني'} {...field} />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />
    </div>
  )
}
