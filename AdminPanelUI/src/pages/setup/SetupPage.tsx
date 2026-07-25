import { useEffect, useRef, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { z } from 'zod'
import { SETUP_STATUS_QUERY_KEY } from '@/components/auth/SetupGuard'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import { MASGED_SETTINGS_QUERY_KEY } from '@/contexts/MasgedBrandingContext'
import { DEFAULT_PRIMARY_COLOR } from '@/lib/masgedBrandingDefaults'
import { applyPrimaryColor } from '@/lib/applyPrimaryColor'
import {
  completeFirstTimeSetup,
  getSetupStatus,
} from '@/services/masgedSettingsService'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp']

const optionalUrl = z
  .string()
  .max(500, 'الرابط يجب ألا يتجاوز 500 حرف')
  .optional()
  .or(z.literal(''))

const setupSchema = z
  .object({
    masgedName: z
      .string()
      .min(1, 'اسم الشركة مطلوب')
      .max(200, 'الاسم يجب ألا يتجاوز 200 حرف'),
    primaryColor: z
      .string()
      .regex(/^#[0-9A-Fa-f]{6}$/, 'اللون يجب أن يكون بصيغة #RRGGBB'),
    domain: z
      .string()
      .min(1, 'النطاق مطلوب')
      .max(200, 'النطاق يجب ألا يتجاوز 200 حرف'),
    parentAppStoreUrl: optionalUrl,
    parentGooglePlayUrl: optionalUrl,
    teacherAppStoreUrl: optionalUrl,
    teacherGooglePlayUrl: optionalUrl,
    adminName: z
      .string()
      .min(1, 'اسم مدير النظام مطلوب')
      .max(200, 'اسم المدير يجب ألا يتجاوز 200 حرف'),
    adminEmail: z
      .string()
      .min(1, 'بريد المدير مطلوب')
      .email('صيغة البريد غير صحيحة')
      .max(200, 'البريد يجب ألا يتجاوز 200 حرف'),
    adminPassword: z
      .string()
      .min(6, 'كلمة المرور يجب ألا تقل عن 6 أحرف')
      .max(500, 'كلمة المرور طويلة جداً'),
    adminPasswordConfirm: z.string().min(1, 'تأكيد كلمة المرور مطلوب'),
  })
  .refine((v) => v.adminPassword === v.adminPasswordConfirm, {
    message: 'كلمتا المرور غير متطابقتين',
    path: ['adminPasswordConfirm'],
  })

type SetupFormValues = z.infer<typeof setupSchema>

export function SetupPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [logoFile, setLogoFile] = useState<File | null>(null)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const [fileError, setFileError] = useState<string | null>(null)

  const statusQuery = useQuery({
    queryKey: SETUP_STATUS_QUERY_KEY,
    queryFn: getSetupStatus,
  })

  const form = useForm<SetupFormValues>({
    resolver: zodResolver(setupSchema),
    defaultValues: {
      masgedName: '',
      primaryColor: DEFAULT_PRIMARY_COLOR,
      domain: '',
      parentAppStoreUrl: '',
      parentGooglePlayUrl: '',
      teacherAppStoreUrl: '',
      teacherGooglePlayUrl: '',
      adminName: '',
      adminEmail: '',
      adminPassword: '',
      adminPasswordConfirm: '',
    },
  })

  useEffect(() => {
    if (!statusQuery.data?.domain) return
    form.setValue('domain', statusQuery.data.domain)
  }, [statusQuery.data?.domain, form])

  const primaryColor = form.watch('primaryColor')
  useEffect(() => {
    applyPrimaryColor(primaryColor)
  }, [primaryColor])

  const mutation = useMutation({
    mutationFn: completeFirstTimeSetup,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: SETUP_STATUS_QUERY_KEY })
      await queryClient.invalidateQueries({ queryKey: MASGED_SETTINGS_QUERY_KEY })
      navigate('/login', { replace: true })
    },
  })

  const handleFileChange = (file: File | undefined) => {
    if (!file) return
    const ext = file.name.slice(file.name.lastIndexOf('.')).toLowerCase()
    if (!allowedExtensions.includes(ext)) {
      setFileError('يرجى اختيار صورة (JPG, PNG, GIF, WebP).')
      return
    }
    setFileError(null)
    setLogoFile(file)
    setPreviewUrl(URL.createObjectURL(file))
  }

  const onSubmit = (values: SetupFormValues) => {
    mutation.mutate({
      masgedName: values.masgedName.trim(),
      primaryColor: values.primaryColor.trim(),
      domain: values.domain.trim(),
      logoFile,
      parentAppStoreUrl: values.parentAppStoreUrl?.trim() || null,
      parentGooglePlayUrl: values.parentGooglePlayUrl?.trim() || null,
      teacherAppStoreUrl: values.teacherAppStoreUrl?.trim() || null,
      teacherGooglePlayUrl: values.teacherGooglePlayUrl?.trim() || null,
      adminName: values.adminName.trim(),
      adminEmail: values.adminEmail.trim(),
      adminPassword: values.adminPassword,
    })
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-[var(--color-layout-bg)] p-4">
      <Card className="w-full max-w-lg space-y-6 p-6">
        <div className="space-y-1 text-center">
          <h1 className="text-2xl font-bold text-slate-900">إعداد النظام لأول مرة</h1>
          <p className="text-sm text-slate-600">
            أدخل بيانات العلامة التجارية قبل استخدام لوحة التحكم
          </p>
        </div>

        {mutation.isError && (
          <Alert variant="destructive">
            {getErrorMessage(mutation.error)}
          </Alert>
        )}

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="masgedName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>اسم الشركة / المسجد</FormLabel>
                  <FormControl>
                    <Input placeholder="اسم المؤسسة" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="primaryColor"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>اللون الرئيسي</FormLabel>
                  <FormControl>
                    <div className="flex items-center gap-3">
                      <Input
                        type="color"
                        className="h-10 w-14 cursor-pointer p-1"
                        value={field.value}
                        onChange={(e) => field.onChange(e.target.value)}
                      />
                      <Input {...field} placeholder="#2563eb" className="font-mono" dir="ltr" />
                    </div>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="domain"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>النطاق (Domain)</FormLabel>
                  <FormControl>
                    <Input {...field} placeholder="customer.com" dir="ltr" readOnly />
                  </FormControl>
                  <p className="text-xs text-slate-500">
                    يتم ضبطه من ملف .env عند النشر (admin.domain و api.domain)
                  </p>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="space-y-2">
              <FormLabel>الشعار</FormLabel>
              <div className="flex items-center gap-4">
                {previewUrl ? (
                  <img
                    src={previewUrl}
                    alt="Logo preview"
                    className="h-16 w-16 rounded-lg object-contain border bg-white"
                  />
                ) : (
                  <div className="flex h-16 w-16 items-center justify-center rounded-lg border bg-slate-50 text-xs text-slate-400">
                    لا يوجد
                  </div>
                )}
                <Input
                  ref={fileInputRef}
                  type="file"
                  accept=".jpg,.jpeg,.png,.gif,.webp"
                  onChange={(e) => handleFileChange(e.target.files?.[0])}
                />
              </div>
              {fileError && <p className="text-sm text-red-600">{fileError}</p>}
            </div>

            <div className="space-y-3 border-t pt-4">
              <h3 className="text-sm font-semibold">حساب مدير النظام (Super Admin)</h3>
              <p className="text-xs text-slate-500">
                يُستخدم لتسجيل الدخول إلى لوحة التحكم (البريد = اسم المستخدم)
              </p>
              <FormField
                control={form.control}
                name="adminName"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>الاسم</FormLabel>
                    <FormControl>
                      <Input placeholder="اسم المدير" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="adminEmail"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>البريد الإلكتروني</FormLabel>
                    <FormControl>
                      <Input type="email" dir="ltr" placeholder="admin@customer.com" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="adminPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>كلمة المرور</FormLabel>
                    <FormControl>
                      <Input type="password" dir="ltr" autoComplete="new-password" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="adminPasswordConfirm"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>تأكيد كلمة المرور</FormLabel>
                    <FormControl>
                      <Input type="password" dir="ltr" autoComplete="new-password" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="space-y-3 border-t pt-4">
              <h3 className="text-sm font-semibold">روابط المتاجر (اختياري)</h3>
              <FormField
                control={form.control}
                name="parentAppStoreUrl"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>App Store — أولياء الأمور</FormLabel>
                    <FormControl>
                      <Input dir="ltr" placeholder="https://apps.apple.com/..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="parentGooglePlayUrl"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Google Play — أولياء الأمور</FormLabel>
                    <FormControl>
                      <Input dir="ltr" placeholder="https://play.google.com/..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="teacherAppStoreUrl"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>App Store — المعلمون</FormLabel>
                    <FormControl>
                      <Input dir="ltr" placeholder="https://apps.apple.com/..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="teacherGooglePlayUrl"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Google Play — المعلمون</FormLabel>
                    <FormControl>
                      <Input dir="ltr" placeholder="https://play.google.com/..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <Button type="submit" className="w-full" disabled={mutation.isPending}>
              {mutation.isPending ? 'جاري الحفظ...' : 'حفظ ومتابعة'}
            </Button>
          </form>
        </Form>
      </Card>
    </div>
  )
}

function getErrorMessage(error: unknown): string {
  if (typeof error === 'object' && error !== null && 'response' in error) {
    const response = (error as { response?: { data?: { message?: string } } }).response
    if (response?.data?.message) return response.data.message
  }
  if (error instanceof Error) return error.message
  return 'تعذر إكمال الإعداد'
}
