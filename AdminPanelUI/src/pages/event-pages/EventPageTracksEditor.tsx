import { Plus, Trash2 } from 'lucide-react'
import type { Control } from 'react-hook-form'
import { useFieldArray } from 'react-hook-form'
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
import type { EventPageFormValues } from '@/pages/event-pages/eventPageFormSchema'

interface EventPageTracksEditorProps {
  control: Control<EventPageFormValues>
}

export function EventPageTracksEditor({ control }: EventPageTracksEditorProps) {
  const { fields, append, remove } = useFieldArray({ control, name: 'tracks' })

  return (
    <section className="space-y-3">
      <div className="flex items-center justify-between">
        <h3 className="text-base font-semibold text-[var(--color-primary)]">مسارات الدورة</h3>
        <Button
          type="button"
          variant="outline"
          onClick={() => append({ title: '', description: '', sortOrder: fields.length + 1 })}
        >
          <Plus className="size-4" />
          إضافة مسار
        </Button>
      </div>

      {fields.length === 0 && (
        <p className="text-sm text-slate-500">لا توجد مسارات. أضف مسارات لتظهر كبطاقات في الصفحة.</p>
      )}

      {fields.map((item, index) => (
        <div key={item.id} className="space-y-3 rounded-xl border border-slate-200 p-4">
          <div className="flex items-center justify-between">
            <span className="text-sm font-medium text-slate-600">مسار {index + 1}</span>
            <Button type="button" variant="outline" className="text-red-600" onClick={() => remove(index)}>
              <Trash2 className="size-4" />
              حذف
            </Button>
          </div>
          <FormField
            control={control}
            name={`tracks.${index}.title`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>العنوان *</FormLabel>
                <FormControl>
                  <Input maxLength={300} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={control}
            name={`tracks.${index}.description`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>الوصف</FormLabel>
                <FormControl>
                  <Textarea rows={2} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={control}
            name={`tracks.${index}.sortOrder`}
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
      ))}
    </section>
  )
}
