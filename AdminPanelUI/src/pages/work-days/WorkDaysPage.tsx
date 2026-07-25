import { useEffect, useState } from 'react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import { useWorkDays } from '@/hooks/useWorkDays'

export function WorkDaysPage() {
  const { query, mutation } = useWorkDays()
  const [selectedDays, setSelectedDays] = useState<number[]>([])
  const [validationError, setValidationError] = useState<string | null>(null)

  useEffect(() => {
    if (query.data?.dayNumbers) {
      setSelectedDays(query.data.dayNumbers)
    }
  }, [query.data?.dayNumbers])

  const toggleDay = (dayNumber: number) => {
    setValidationError(null)
    setSelectedDays((prev) =>
      prev.includes(dayNumber) ? prev.filter((d) => d !== dayNumber) : [...prev, dayNumber],
    )
  }

  const handleSave = async () => {
    if (selectedDays.length === 0) {
      setValidationError('يجب اختيار يوم عمل واحد على الأقل')
      return
    }

    setValidationError(null)
    await mutation.mutateAsync(selectedDays)
  }

  if (query.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-10 w-64" />
        <Skeleton className="h-48 w-full max-w-lg" />
      </div>
    )
  }

  if (query.isError) {
    return (
      <Alert variant="destructive">
        {query.error instanceof Error ? query.error.message : 'تعذر تحميل أيام العمل'}
      </Alert>
    )
  }

  const dayLabels = query.data?.dayLabels ?? []

  return (
    <div className="space-y-6">
      <PageHeader title="أيام العمل" description="حدد أيام العمل في الأسبوع (من السبت إلى الجمعة)" />

      {mutation.isSuccess && (
        <Alert>تم حفظ أيام العمل بنجاح</Alert>
      )}

      {mutation.isError && (
        <Alert variant="destructive">
          {mutation.error instanceof Error ? mutation.error.message : 'تعذر حفظ أيام العمل'}
        </Alert>
      )}

      {validationError && <Alert variant="destructive">{validationError}</Alert>}

      <Card className="max-w-lg p-6">
        <p className="mb-4 text-sm text-muted-foreground">
          الأيام غير المحددة تُعتبر إجازة. لن يُسمح بتسجيل الحضور أو الانصراف في أيام الإجازة،
          وتظهر في التقارير كـ «اجازة» بدلاً من «غائب».
        </p>

        <div className="space-y-3">
          {dayLabels.map((day) => (
            <label
              key={day.number}
              className="flex cursor-pointer items-center gap-3 rounded-md border px-4 py-3 hover:bg-muted/50"
            >
              <input
                type="checkbox"
                checked={selectedDays.includes(day.number)}
                onChange={() => toggleDay(day.number)}
                className="size-4 accent-[#7C8738]"
              />
              <span className="font-medium">{day.nameAr}</span>
            </label>
          ))}
        </div>

        <div className="mt-6 flex justify-end">
          <Button onClick={handleSave} disabled={mutation.isPending}>
            {mutation.isPending ? 'جاري الحفظ...' : 'حفظ'}
          </Button>
        </div>
      </Card>
    </div>
  )
}
