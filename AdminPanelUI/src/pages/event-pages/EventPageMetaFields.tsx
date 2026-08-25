import type { Control } from 'react-hook-form'
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { EventPageTextField } from '@/pages/event-pages/EventPageTextField'
import type { EventPageFormValues } from '@/pages/event-pages/eventPageFormSchema'

interface EventPageMetaFieldsProps {
  control: Control<EventPageFormValues>
}

export function EventPageMetaFields({ control }: EventPageMetaFieldsProps) {
  return (
    <div className="grid gap-4 md:grid-cols-2">
      <EventPageTextField
        control={control}
        name="activityName"
        label="اسم النشاط *"
        maxLength={200}
      />
      <EventPageTextField
        control={control}
        name="slug"
        label="رابط الصفحة *"
        maxLength={120}
        placeholder="hifz-sunnah-1"
      />
      <FormField
        control={control}
        name="isPublished"
        render={({ field }) => (
          <FormItem>
            <FormLabel>منشورة</FormLabel>
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
        name="isRegistrationOpen"
        render={({ field }) => (
          <FormItem>
            <FormLabel>التسجيل مفتوح</FormLabel>
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
    </div>
  )
}
