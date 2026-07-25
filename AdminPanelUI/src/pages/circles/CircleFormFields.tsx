import { Link } from 'react-router-dom'
import type { UseFormReturn } from 'react-hook-form'
import { Button } from '@/components/ui/button'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import type { CircleFormValues } from '@/pages/circles/circleFormSchema'
import type { CircleTeacherOption } from '@/types/circle'

interface CircleFormFieldsProps {
  form: UseFormReturn<CircleFormValues>
  teachers: CircleTeacherOption[]
  isEdit: boolean
  isSaving: boolean
  onSubmit: (values: CircleFormValues) => void
  onDelete: () => void
}

export function CircleFormFields({
  form,
  teachers,
  isEdit,
  isSaving,
  onSubmit,
  onDelete,
}: CircleFormFieldsProps) {
  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <div className="grid gap-5 md:grid-cols-2">
          <FormField
            control={form.control}
            name="name"
            render={({ field }) => (
              <FormItem>
                <FormLabel>اسم الحلقة *</FormLabel>
                <FormControl>
                  <Input placeholder="أدخل اسم الحلقة" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="teacherId"
            render={({ field }) => (
              <FormItem>
                <FormLabel>المعلم المسؤول *</FormLabel>
                <FormControl>
                  <select
                    className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                    {...field}
                  >
                    <option value="">اختر المعلم</option>
                    {teachers.map((teacher) => (
                      <option key={teacher.id} value={teacher.id}>
                        {teacher.name}
                      </option>
                    ))}
                  </select>
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <FormField
          control={form.control}
          name="forGirls"
          render={({ field }) => (
            <FormItem>
              <label className="flex cursor-pointer items-center gap-3 rounded-lg border bg-slate-50 px-4 py-3">
                <FormControl>
                  <input
                    type="checkbox"
                    checked={field.value}
                    onChange={(event) => field.onChange(event.target.checked)}
                    className="size-5 accent-[var(--color-primary)]"
                  />
                </FormControl>
                <span className="font-semibold">حلقة للنساء</span>
              </label>
            </FormItem>
          )}
        />

        <p className="text-sm text-muted-foreground">
          أيام العمل والحضور تُحدَّد من{' '}
          <Link to="/work-days" className="font-medium text-[var(--color-primary)] underline">
            إعدادات أيام العمل
          </Link>{' '}
          وتطبَّق على جميع الحلقات.
        </p>

        <div className="flex flex-wrap justify-end gap-2">
          <Button type="button" variant="outline" asChild>
            <Link to="/circles">إلغاء</Link>
          </Button>
          {isEdit && (
            <Button
              type="button"
              variant="outline"
              className="border-red-200 text-red-600 hover:bg-red-50"
              onClick={onDelete}
            >
              حذف
            </Button>
          )}
          <Button type="submit" disabled={isSaving}>
            {isSaving ? 'جاري الحفظ...' : 'حفظ الحلقة'}
          </Button>
        </div>
      </form>
    </Form>
  )
}
