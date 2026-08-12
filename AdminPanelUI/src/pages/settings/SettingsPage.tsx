import { useEffect, useRef, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { PageHeader } from '@/components/shared/PageHeader'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
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
import { Skeleton } from '@/components/ui/skeleton'
import { useMasgedSettings } from '@/hooks/useMasgedSettings'
import { DEFAULT_PRIMARY_COLOR } from '@/lib/masgedBrandingDefaults'
import { resolveLogoUrl } from '@/lib/resolveLogoUrl'

const allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp']

const optionalUrl = z
  .string()
  .max(500, 'الرابط يجب ألا يتجاوز 500 حرف')
  .optional()
  .or(z.literal(''))

const settingsSchema = z.object({
  masgedName: z
    .string()
    .min(1, 'اسم المسجد مطلوب')
    .max(200, 'اسم المسجد يجب ألا يتجاوز 200 حرف'),
  primaryColor: z
    .string()
    .regex(/^#[0-9A-Fa-f]{6}$/, 'اللون يجب أن يكون بصيغة #RRGGBB'),
  parentAppStoreUrl: optionalUrl,
  parentGooglePlayUrl: optionalUrl,
  teacherAppStoreUrl: optionalUrl,
  teacherGooglePlayUrl: optionalUrl,
})

type SettingsFormValues = z.infer<typeof settingsSchema>

export function SettingsPage() {
  const { masgedName: currentName } = useMasgedBranding()
  const { query, mutation } = useMasgedSettings()
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [logoFile, setLogoFile] = useState<File | null>(null)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const [removeLogo, setRemoveLogo] = useState(false)
  const [fileError, setFileError] = useState<string | null>(null)

  const form = useForm<SettingsFormValues>({
    resolver: zodResolver(settingsSchema),
    defaultValues: {
      masgedName: currentName,
      primaryColor: DEFAULT_PRIMARY_COLOR,
      parentAppStoreUrl: '',
      parentGooglePlayUrl: '',
      teacherAppStoreUrl: '',
      teacherGooglePlayUrl: '',
    },
  })

  useEffect(() => {
    if (!query.isSuccess) return
    form.reset({
      masgedName: query.data?.masgedName ?? currentName,
      primaryColor: query.data?.primaryColor?.trim() || DEFAULT_PRIMARY_COLOR,
      parentAppStoreUrl: query.data?.parentAppStoreUrl ?? '',
      parentGooglePlayUrl: query.data?.parentGooglePlayUrl ?? '',
      teacherAppStoreUrl: query.data?.teacherAppStoreUrl ?? '',
      teacherGooglePlayUrl: query.data?.teacherGooglePlayUrl ?? '',
    })
    setPreviewUrl(resolveLogoUrl(query.data?.logoUrl) ?? null)
    setLogoFile(null)
    setRemoveLogo(false)
  }, [query.isSuccess, query.data, currentName, form])

  const handleFileChange = (file: File | undefined) => {
    if (!file) return
    const ext = file.name.slice(file.name.lastIndexOf('.')).toLowerCase()
    if (!allowedExtensions.includes(ext)) {
      setFileError('يرجى اختيار صورة (JPG, PNG, GIF, WebP).')
      return
    }
    setFileError(null)
    setLogoFile(file)
    setRemoveLogo(false)
    setPreviewUrl(URL.createObjectURL(file))
  }

  const handleRemoveLogo = () => {
    setLogoFile(null)
    setPreviewUrl(null)
    setRemoveLogo(true)
    if (fileInputRef.current) fileInputRef.current.value = ''
  }

  const onSubmit = (values: SettingsFormValues) => {
    mutation.mutate({
      masgedName: values.masgedName.trim(),
      logoFile,
      removeLogo,
      primaryColor: values.primaryColor.trim(),
      parentAppStoreUrl: values.parentAppStoreUrl?.trim() || null,
      parentGooglePlayUrl: values.parentGooglePlayUrl?.trim() || null,
      teacherAppStoreUrl: values.teacherAppStoreUrl?.trim() || null,
      teacherGooglePlayUrl: values.teacherGooglePlayUrl?.trim() || null,
    })
  }

  if (query.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (query.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل الإعدادات. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title="الإعدادات"
        description="اسم المسجد والشعار المعروض في لوحة التحكم والتطبيق"
      />

      {mutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حفظ التغييرات. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {mutation.isSuccess && (
        <Alert className="mb-4 border-green-200 bg-green-50 text-green-800">
          تم حفظ الإعدادات بنجاح.
        </Alert>
      )}

      <Card className="max-w-2xl">
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <FormField
              control={form.control}
              name="masgedName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>اسم المسجد</FormLabel>
                  <FormControl>
                    <Input maxLength={200} placeholder="اسم المسجد أو الجمعية" {...field} />
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
                      <Input {...field} className="font-mono" dir="ltr" maxLength={7} />
                    </div>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="space-y-4">
              <div>
                <h3 className="text-sm font-semibold">روابط تطبيق أولياء الأمور</h3>
                <p className="text-sm text-muted-foreground">
                  تظهر في الموقع العام عند اختيار «أولياء الأمور»
                </p>
              </div>
              <FormField
                control={form.control}
                name="parentAppStoreUrl"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>App Store</FormLabel>
                    <FormControl>
                      <Input dir="ltr" maxLength={500} placeholder="https://apps.apple.com/..." {...field} />
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
                    <FormLabel>Google Play</FormLabel>
                    <FormControl>
                      <Input dir="ltr" maxLength={500} placeholder="https://play.google.com/..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="space-y-4">
              <div>
                <h3 className="text-sm font-semibold">روابط تطبيق المعلمين</h3>
                <p className="text-sm text-muted-foreground">
                  تظهر في الموقع العام عند اختيار «المعلمون»
                </p>
              </div>
              <FormField
                control={form.control}
                name="teacherAppStoreUrl"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>App Store</FormLabel>
                    <FormControl>
                      <Input dir="ltr" maxLength={500} placeholder="https://apps.apple.com/..." {...field} />
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
                    <FormLabel>Google Play</FormLabel>
                    <FormControl>
                      <Input dir="ltr" maxLength={500} placeholder="https://play.google.com/..." {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="space-y-2">
              <FormLabel>الشعار</FormLabel>
              {previewUrl ? (
                <div className="flex items-center gap-4">
                  <img
                    src={previewUrl}
                    alt="معاينة الشعار"
                    className="size-24 rounded-xl border object-contain"
                  />
                  <Button type="button" variant="outline" onClick={handleRemoveLogo}>
                    إزالة الشعار
                  </Button>
                </div>
              ) : (
                <p className="text-sm text-muted-foreground">لم يتم رفع شعار بعد.</p>
              )}
              <Input
                ref={fileInputRef}
                type="file"
                accept=".jpg,.jpeg,.png,.gif,.webp"
                onChange={(e) => handleFileChange(e.target.files?.[0])}
              />
              {fileError ? <p className="text-sm text-destructive">{fileError}</p> : null}
            </div>

            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending ? 'جاري الحفظ...' : 'حفظ'}
            </Button>
          </form>
        </Form>
      </Card>
    </div>
  )
}
