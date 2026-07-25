import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { KeyRound, Loader2 } from 'lucide-react'
import { Alert } from '@/components/ui/alert'
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
import { useChangePassword } from '@/hooks/useChangePassword'

const changePasswordSchema = z
  .object({
    currentPassword: z.string().trim().min(1, 'يرجى إدخال كلمة المرور الحالية'),
    newPassword: z.string().trim().min(1, 'يرجى إدخال كلمة المرور الجديدة'),
    confirmPassword: z.string().trim().min(1, 'يرجى تأكيد كلمة المرور'),
  })
  .refine((values) => values.newPassword === values.confirmPassword, {
    message: 'كلمة المرور غير متطابقة',
    path: ['confirmPassword'],
  })

type ChangePasswordFormValues = z.infer<typeof changePasswordSchema>

interface ChangePasswordDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function ChangePasswordDialog({ open, onOpenChange }: ChangePasswordDialogProps) {
  const mutation = useChangePassword()

  const form = useForm<ChangePasswordFormValues>({
    resolver: zodResolver(changePasswordSchema),
    defaultValues: {
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    },
  })

  if (!open) return null

  const handleClose = () => {
    if (mutation.isPending) return
    form.reset()
    mutation.reset()
    onOpenChange(false)
  }

  const onSubmit = (values: ChangePasswordFormValues) => {
    mutation.mutate(values, {
      onSuccess: (response) => {
        if (response.success) return
        form.setError('root', { message: response.message || 'تعذر تغيير كلمة المرور' })
      },
      onError: () => {
        form.setError('root', { message: 'تعذر الاتصال بالخادم. يرجى المحاولة مرة أخرى.' })
      },
    })
  }

  return (
    <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/40 p-4">
      <div className="w-full max-w-md rounded-2xl border border-blue-100 bg-white p-6 shadow-xl">
        <div className="mb-5 flex items-center gap-3">
          <div className="flex size-10 items-center justify-center rounded-xl bg-blue-50 text-[var(--color-primary)]">
            <KeyRound className="size-5" />
          </div>
          <div>
            <h2 className="text-lg font-bold text-slate-800">تغيير كلمة المرور</h2>
            <p className="text-sm text-slate-500">أدخل كلمة المرور الحالية والجديدة</p>
          </div>
        </div>

        {form.formState.errors.root?.message && (
          <Alert variant="destructive" className="mb-4 border-red-200 bg-red-50">
            {form.formState.errors.root.message}
          </Alert>
        )}

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="currentPassword"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>كلمة المرور الحالية</FormLabel>
                  <FormControl>
                    <Input {...field} type="password" autoComplete="current-password" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="newPassword"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>كلمة المرور الجديدة</FormLabel>
                  <FormControl>
                    <Input {...field} type="password" autoComplete="new-password" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="confirmPassword"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>تأكيد كلمة المرور</FormLabel>
                  <FormControl>
                    <Input {...field} type="password" autoComplete="new-password" />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="outline" disabled={mutation.isPending} onClick={handleClose}>
                إلغاء
              </Button>
              <Button type="submit" disabled={mutation.isPending}>
                {mutation.isPending ? (
                  <>
                    <Loader2 className="ms-2 size-4 animate-spin" />
                    جاري الحفظ...
                  </>
                ) : (
                  'حفظ'
                )}
              </Button>
            </div>
          </form>
        </Form>
      </div>
    </div>
  )
}
