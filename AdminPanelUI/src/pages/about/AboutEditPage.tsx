import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { PageHeader } from '@/components/shared/PageHeader'
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
import { Textarea } from '@/components/ui/textarea'
import { useAbout } from '@/hooks/useAbout'

const aboutSchema = z.object({
  content: z.string().nullable().optional(),
  address: z
    .string()
    .max(500, 'العنوان يجب ألا يتجاوز 500 حرف')
    .nullable()
    .optional(),
  mapsUrl: z
    .string()
    .max(1000, 'رابط الخريطة يجب ألا يتجاوز 1000 حرف')
    .nullable()
    .optional(),
})

type AboutFormValues = z.infer<typeof aboutSchema>

export function AboutEditPage() {
  const { query, mutation } = useAbout()

  const form = useForm<AboutFormValues>({
    resolver: zodResolver(aboutSchema),
    defaultValues: {
      content: '',
      address: '',
      mapsUrl: '',
    },
  })

  useEffect(() => {
    if (query.data) {
      form.reset({
        content: query.data.content ?? '',
        address: query.data.address ?? '',
        mapsUrl: query.data.mapsUrl ?? '',
      })
    }
  }, [query.data, form])

  const onSubmit = (values: AboutFormValues) => {
    mutation.mutate({
      content: values.content ?? null,
      address: values.address ?? null,
      mapsUrl: values.mapsUrl ?? null,
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
        تعذر تحميل بيانات «عن الجمعية». يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  return (
    <div>
      <PageHeader
        title="عن الجمعية"
        description="نص «عن الجمعية» في تذييل الصفحة الرئيسية"
      />

      {mutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حفظ التغييرات. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <Card>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-1">
            <FormField
              control={form.control}
              name="content"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>المحتوى</FormLabel>
                  <FormControl>
                    <Textarea rows={6} {...field} value={field.value ?? ''} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="address"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>العنوان</FormLabel>
                  <FormControl>
                    <Input
                      maxLength={500}
                      placeholder="العنوان الفعلي للجمعية"
                      {...field}
                      value={field.value ?? ''}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="mapsUrl"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>رابط الخريطة (Google Maps)</FormLabel>
                  <FormControl>
                    <Input
                      maxLength={1000}
                      placeholder="https://maps.google.com/..."
                      {...field}
                      value={field.value ?? ''}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending ? 'جاري الحفظ...' : 'حفظ'}
            </Button>
          </form>
        </Form>
      </Card>
    </div>
  )
}
