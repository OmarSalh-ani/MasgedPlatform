import type { Control } from 'react-hook-form'
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import { resolveImageUrl } from '@/lib/resolveImageUrl'
import type { EventPageFormValues } from '@/pages/event-pages/eventPageFormSchema'

interface EventPageImageFieldProps {
  control: Control<EventPageFormValues>
  currentImageUrl: string | null
}

export function EventPageImageField({ control, currentImageUrl }: EventPageImageFieldProps) {
  return (
    <FormField
      control={control}
      name="imageFile"
      render={({ field: { onChange, ...field } }) => (
        <FormItem>
          <FormLabel>صورة الصفحة</FormLabel>
          <FormControl>
            <Input
              type="file"
              accept="image/*"
              onChange={(event) => onChange(event.target.files?.[0])}
              {...field}
              value={undefined}
            />
          </FormControl>
          <p className="text-sm text-slate-500">الامتدادات المسموحة: jpg, jpeg, png, gif</p>
          {currentImageUrl && (
            <div className="mt-2">
              <span className="mb-1 block text-sm">الصورة الحالية:</span>
              <img
                src={resolveImageUrl(currentImageUrl)}
                alt="صورة الصفحة"
                className="max-h-[160px] max-w-[220px] rounded-lg border-2 border-slate-200 object-contain"
              />
            </div>
          )}
          <FormMessage />
        </FormItem>
      )}
    />
  )
}
