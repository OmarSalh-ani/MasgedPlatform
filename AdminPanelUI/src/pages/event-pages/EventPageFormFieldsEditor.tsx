import { Plus } from 'lucide-react'
import type { Control } from 'react-hook-form'
import { useFieldArray } from 'react-hook-form'
import { Button } from '@/components/ui/button'
import { EventPageFormFieldRow } from '@/pages/event-pages/EventPageFormFieldRow'
import type { EventPageFormValues } from '@/pages/event-pages/eventPageFormSchema'

interface EventPageFormFieldsEditorProps {
  control: Control<EventPageFormValues>
}

export function EventPageFormFieldsEditor({ control }: EventPageFormFieldsEditorProps) {
  const { fields, append, remove } = useFieldArray({ control, name: 'formFields' })

  return (
    <section className="space-y-3">
      <div className="flex items-center justify-between">
        <h3 className="text-base font-semibold text-[var(--color-primary)]">حقول نموذج التسجيل</h3>
        <Button
          type="button"
          variant="outline"
          onClick={() =>
            append({
              label: '',
              fieldType: 'Text',
              isRequired: true,
              sortOrder: fields.length + 1,
              optionsText: '',
            })
          }
        >
          <Plus className="size-4" />
          إضافة حقل
        </Button>
      </div>
      {fields.length === 0 && (
        <p className="text-sm text-slate-500">
          أضف حقولاً ديناميكية مثل الاسم ورقم الهاتف والمسار.
        </p>
      )}
      {fields.map((item, index) => (
        <EventPageFormFieldRow
          key={item.id}
          control={control}
          index={index}
          onRemove={() => remove(index)}
        />
      ))}
    </section>
  )
}
