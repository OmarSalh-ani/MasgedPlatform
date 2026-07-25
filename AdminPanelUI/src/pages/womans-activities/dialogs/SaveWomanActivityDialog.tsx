import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { X } from 'lucide-react'
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
import {
  saveWomanActivitySchema,
  type SaveWomanActivityFormValues,
} from '@/pages/womans-activities/saveWomanActivitySchema'
import type { WomanActivityListItem } from '@/types/womansActivity'

interface SaveWomanActivityDialogProps {
  open: boolean
  activity: WomanActivityListItem | null
  feminineTheme: boolean
  isPending: boolean
  onOpenChange: (open: boolean) => void
  onSubmit: (values: SaveWomanActivityFormValues) => void
}

export function SaveWomanActivityDialog({
  open,
  activity,
  feminineTheme,
  isPending,
  onOpenChange,
  onSubmit,
}: SaveWomanActivityDialogProps) {
  const form = useForm<SaveWomanActivityFormValues>({
    resolver: zodResolver(saveWomanActivitySchema),
    defaultValues: { name: '', isVisible: true },
  })

  useEffect(() => {
    if (!open) return
    form.reset({
      name: activity?.name ?? '',
      isVisible: activity?.isVisible ?? true,
    })
  }, [open, activity, form])

  if (!open) return null

  const primaryBtnClass = feminineTheme
    ? 'bg-gradient-to-br from-pink-600 to-pink-800 hover:opacity-90'
    : 'bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] hover:opacity-90'

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-lg">
        <div className="mb-5 flex items-center justify-between border-b pb-4">
          <h2
            className={`text-lg font-bold ${feminineTheme ? 'text-pink-600' : 'text-[var(--color-primary)]'}`}
          >
            {activity ? 'تعديل النشاط' : 'إضافة نشاط جديد'}
          </h2>
          <button
            type="button"
            className="text-slate-500 hover:text-slate-700"
            onClick={() => onOpenChange(false)}
          >
            <X className="size-5" />
          </button>
        </div>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>اسم النشاط</FormLabel>
                  <FormControl>
                    <Input placeholder="أدخل اسم النشاط" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="isVisible"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>مرئي</FormLabel>
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
            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                إلغاء
              </Button>
              <Button type="submit" className={primaryBtnClass} disabled={isPending}>
                {isPending ? 'جاري الحفظ...' : 'حفظ'}
              </Button>
            </div>
          </form>
        </Form>
      </div>
    </div>
  )
}
