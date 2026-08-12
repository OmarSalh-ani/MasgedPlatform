import type { UseFormReturn } from 'react-hook-form'
import { SearchableDropdown } from '@/components/shared/SearchableDropdown'
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import type { CircleRatingFormValues } from '@/pages/circle-ratings/circleRatingFormSchema'
import {
  toCircleDropdownOptions,
  toTeacherDropdownOptions,
  type CircleVisitRatingCircleOption,
  type CircleVisitRatingTeacherOption,
} from '@/types/circleVisitRating'

interface CircleRatingMetaFieldsProps {
  form: UseFormReturn<CircleRatingFormValues>
  teachers: CircleVisitRatingTeacherOption[]
  circles: CircleVisitRatingCircleOption[]
  visitNumber: number | undefined
  onTeacherChange: () => void
}

export function CircleRatingMetaFields({
  form,
  teachers,
  circles,
  visitNumber,
  onTeacherChange,
}: CircleRatingMetaFieldsProps) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm space-y-5">
      <div className="grid gap-4 md:grid-cols-2">
        <FormField
          control={form.control}
          name="teacherId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>المعلم *</FormLabel>
              <FormControl>
                <SearchableDropdown
                  id="teacherPick"
                  value={field.value}
                  onChange={(value) => {
                    field.onChange(value)
                    onTeacherChange()
                  }}
                  options={toTeacherDropdownOptions(teachers)}
                  placeholder="— اختر المعلم —"
                  searchPlaceholder="ابحث باسم المعلم..."
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="quranCircleId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>الحلقة *</FormLabel>
              <FormControl>
                <SearchableDropdown
                  id="circlePick"
                  value={field.value}
                  onChange={field.onChange}
                  options={toCircleDropdownOptions(circles)}
                  disabled={!form.watch('teacherId')}
                  placeholder="— اختر الحلقة —"
                  searchPlaceholder="ابحث باسم الحلقة..."
                  emptyMessage="لا توجد حلقات لهذا المعلم"
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <FormField
          control={form.control}
          name="visitDate"
          render={({ field }) => (
            <FormItem>
              <FormLabel>تاريخ الزيارة *</FormLabel>
              <FormControl>
                <Input type="date" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="visitTime"
          render={({ field }) => (
            <FormItem>
              <FormLabel>وقت الزيارة *</FormLabel>
              <FormControl>
                <Input type="time" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormItem>
          <FormLabel>رقم الزيارة هذا الشهر</FormLabel>
          <FormControl>
            <Input
              readOnly
              value={visitNumber != null ? String(visitNumber) : '—'}
              className="bg-slate-50"
            />
          </FormControl>
        </FormItem>
      </div>
    </div>
  )
}
