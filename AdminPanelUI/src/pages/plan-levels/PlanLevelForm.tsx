import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
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
  savePlanLevelSchema,
  type SavePlanLevelFormValues,
} from '@/pages/plan-levels/savePlanLevelSchema'
import { PLAN_UNIT_TYPE_OPTIONS } from '@/types/planLevel'

interface PlanLevelFormProps {
  editingId: number | null
  isPending: boolean
  onSubmit: (values: SavePlanLevelFormValues) => void
  onCancelEdit: () => void
}

export function PlanLevelForm({
  editingId,
  isPending,
  onSubmit,
  onCancelEdit,
}: PlanLevelFormProps) {
  const form = useForm<SavePlanLevelFormValues>({
    resolver: zodResolver(savePlanLevelSchema),
    defaultValues: {
      levelName: '',
      unitType: PLAN_UNIT_TYPE_OPTIONS[0].value,
      quantity: 1,
    },
  })

  useEffect(() => {
    if (editingId === null) {
      form.reset({
        levelName: '',
        unitType: PLAN_UNIT_TYPE_OPTIONS[0].value,
        quantity: 1,
      })
    }
  }, [editingId, form])

  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className="flex flex-wrap items-end gap-4"
      >
        <FormField
          control={form.control}
          name="levelName"
          render={({ field }) => (
            <FormItem className="min-w-[220px] flex-1">
              <FormLabel>اسم المستوى</FormLabel>
              <FormControl>
                <Input {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="unitType"
          render={({ field }) => (
            <FormItem className="min-w-[220px] flex-1">
              <FormLabel>نوع القدرة</FormLabel>
              <FormControl>
                <select
                  {...field}
                  value={field.value}
                  onChange={(event) =>
                    field.onChange(Number(event.target.value) as SavePlanLevelFormValues['unitType'])
                  }
                  className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
                >
                  {PLAN_UNIT_TYPE_OPTIONS.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="quantity"
          render={({ field }) => (
            <FormItem className="min-w-[220px] flex-1">
              <FormLabel>الكمية</FormLabel>
              <FormControl>
                <Input
                  {...field}
                  type="number"
                  min={1}
                  max={1000}
                  onChange={(event) => field.onChange(event.target.valueAsNumber)}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <div className="flex gap-2">
          <Button type="submit" disabled={isPending}>
            {isPending ? 'جاري الحفظ...' : 'حفظ'}
          </Button>
          {editingId !== null && (
            <Button type="button" variant="outline" onClick={onCancelEdit}>
              إلغاء الطي
            </Button>
          )}
        </div>
      </form>
    </Form>
  )
}
