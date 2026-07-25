import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Loader2, Lock, LogIn, User } from 'lucide-react'
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
import { Skeleton } from '@/components/ui/skeleton'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { useLogin } from '@/hooks/useLogin'

const loginSchema = z.object({
  username: z.string().trim().min(1, 'يرجى إدخال اسم المستخدم'),
  password: z.string().trim().min(1, 'يرجى إدخال كلمة المرور'),
})

type LoginFormValues = z.infer<typeof loginSchema>

export function LoginForm() {
  const { masgedName, logoUrl, isLoading } = useMasgedBranding()
  const mutation = useLogin()
  const [serverError, setServerError] = useState<string | null>(null)

  const form = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { username: '', password: '' },
  })

  const onSubmit = (values: LoginFormValues) => {
    setServerError(null)
    mutation.mutate(values, {
      onSuccess: (response) => {
        if (response.success) return
        setServerError(response.message || 'خطأ في أسم المستخدم أو كلمة المرور')
      },
      onError: () => {
        setServerError('تعذر الاتصال بالخادم. يرجى المحاولة مرة أخرى.')
      },
    })
  }

  const emptyFields =
    !form.watch('username')?.trim() || !form.watch('password')?.trim()

  const handleSubmit = form.handleSubmit((values) => {
    if (!values.username.trim() || !values.password.trim()) {
      setServerError('يرجى تعبئة جميع الحقول!')
      return
    }
    onSubmit(values)
  })

  return (
    <>
      <header className="mb-8 flex flex-col items-center text-center">
        {isLoading ? (
          <>
            <Skeleton className="mb-4 h-24 w-48 max-w-full rounded-lg" />
            <Skeleton className="h-7 w-56 max-w-full" />
          </>
        ) : (
          <>
            <img
              src={logoUrl}
              alt={masgedName}
              className="mb-4 w-full max-w-[200px] lg:hidden"
            />
            <h1 className="text-xl font-bold text-slate-800">{masgedName}</h1>
          </>
        )}
        <p className="mt-2 text-sm text-slate-500">سجّل دخولك للوصول إلى لوحة التحكم</p>
      </header>

      {serverError && (
        <Alert variant="destructive" className="mb-5 border-red-200 bg-red-50">
          {serverError}
        </Alert>
      )}

      <Form {...form}>
        <form onSubmit={handleSubmit} className="space-y-5">
          <FormField
            control={form.control}
            name="username"
            render={({ field }) => (
              <FormItem>
                <FormLabel className="flex items-center gap-2 text-sm font-semibold text-slate-700">
                  <User className="size-4 text-[#7c8738]" />
                  اسم المستخدم
                </FormLabel>
                <FormControl>
                  <Input
                    {...field}
                    type="text"
                    placeholder="أدخل اسم المستخدم"
                    autoComplete="username"
                    className="login-input h-12 rounded-lg border-slate-200 bg-white text-base"
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="password"
            render={({ field }) => (
              <FormItem>
                <FormLabel className="flex items-center gap-2 text-sm font-semibold text-slate-700">
                  <Lock className="size-4 text-[#7c8738]" />
                  كلمة المرور
                </FormLabel>
                <FormControl>
                  <Input
                    {...field}
                    type="password"
                    placeholder="أدخل كلمة المرور"
                    autoComplete="current-password"
                    className="login-input h-12 rounded-lg border-slate-200 bg-white text-base"
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <Button
            type="submit"
            disabled={mutation.isPending || emptyFields}
            className="login-submit mt-2 h-12 w-full rounded-lg border-0 text-base font-semibold text-white"
          >
            {mutation.isPending ? (
              <>
                <Loader2 className="ms-2 size-5 animate-spin" />
                جاري تسجيل الدخول...
              </>
            ) : (
              <>
                <LogIn className="ms-2 size-5" />
                تسجيل الدخول
              </>
            )}
          </Button>
        </form>
      </Form>

      {mutation.isPending && (
        <div
          className="login-spinner-overlay fixed inset-0 z-50 flex items-center justify-center bg-white/80"
          aria-live="polite"
          aria-busy="true"
        >
          <div className="flex flex-col items-center gap-3">
            <div className="size-11 animate-spin rounded-full border-4 border-[#7c8738]/30 border-t-[#7c8738]" />
            <p className="text-sm font-medium text-slate-600">جاري التحقق...</p>
          </div>
        </div>
      )}
    </>
  )
}
