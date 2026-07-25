import * as React from 'react'
import {
  Controller,
  FormProvider,
  useFormContext,
  type ControllerProps,
  type FieldPath,
  type FieldValues,
} from 'react-hook-form'
import { Label } from '@/components/ui/label'
import { cn } from '@/lib/utils'

export const Form = FormProvider

type FormFieldContextValue = { name: string }
const FormFieldContext = React.createContext<FormFieldContextValue>({} as FormFieldContextValue)

export function FormField<
  TFieldValues extends FieldValues = FieldValues,
  TName extends FieldPath<TFieldValues> = FieldPath<TFieldValues>,
>({ ...props }: ControllerProps<TFieldValues, TName>) {
  return (
    <FormFieldContext.Provider value={{ name: props.name }}>
      <Controller {...props} />
    </FormFieldContext.Provider>
  )
}

export function FormItem({ className, ...props }: React.HTMLAttributes<HTMLDivElement>) {
  return <div className={cn('mb-5', className)} {...props} />
}

export function FormLabel(props: React.ComponentProps<typeof Label>) {
  return <Label {...props} />
}

export function FormControl({ ...props }: React.HTMLAttributes<HTMLDivElement>) {
  const { error } = useFormField()
  return <div aria-invalid={!!error} {...props} />
}

export function FormMessage({ className, ...props }: React.HTMLAttributes<HTMLParagraphElement>) {
  const { error } = useFormField()
  if (!error) return null
  return (
    <p className={cn('mt-1 text-sm text-red-600', className)} {...props}>
      {String(error.message)}
    </p>
  )
}

function useFormField() {
  const fieldContext = React.useContext(FormFieldContext)
  const { getFieldState, formState } = useFormContext()
  return getFieldState(fieldContext.name, formState)
}
