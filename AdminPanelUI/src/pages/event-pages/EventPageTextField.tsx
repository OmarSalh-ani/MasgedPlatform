import type { Control, FieldPath } from 'react-hook-form'
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

interface EventPageTextFieldProps {
  control: Control<EventPageFormValues>
  name: FieldPath<EventPageFormValues>
  label: string
  maxLength?: number
  rows?: number
  placeholder?: string
}

export function EventPageTextField({
  control,
  name,
  label,
  maxLength,
  rows,
  placeholder,
}: EventPageTextFieldProps) {
  return (
    <FormField
      control={control}
      name={name}
      render={({ field }) => (
        <FormItem>
          <FormLabel>{label}</FormLabel>
          <FormControl>
            {rows ? (
              <Textarea
                rows={rows}
                maxLength={maxLength}
                placeholder={placeholder}
                value={String(field.value ?? '')}
                onChange={field.onChange}
                onBlur={field.onBlur}
                name={field.name}
                ref={field.ref}
              />
            ) : (
              <Input
                maxLength={maxLength}
                placeholder={placeholder}
                value={String(field.value ?? '')}
                onChange={field.onChange}
                onBlur={field.onBlur}
                name={field.name}
                ref={field.ref}
              />
            )}
          </FormControl>
          <FormMessage />
        </FormItem>
      )}
    />
  )
}
