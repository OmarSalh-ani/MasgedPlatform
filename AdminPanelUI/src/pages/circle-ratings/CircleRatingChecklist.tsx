import type { UseFormReturn } from 'react-hook-form'
import {
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import type { CircleRatingFormValues } from '@/pages/circle-ratings/circleRatingFormSchema'
import { CIRCLE_VISIT_RATING_VALUES } from '@/types/circleVisitRating'

interface CircleRatingChecklistProps {
  form: UseFormReturn<CircleRatingFormValues>
}

const selectClassName =
  'flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm'

export function CircleRatingChecklist({ form }: CircleRatingChecklistProps) {
  const items = form.watch('items')

  return (
    <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
      <h2 className="mb-4 border-b border-slate-100 pb-2 text-lg font-semibold text-slate-800">
        عناصر التقييم
      </h2>
      <div className="overflow-x-auto">
        <table className="min-w-full text-sm">
          <thead>
            <tr className="border-b border-slate-200 bg-slate-50">
              <th className="px-3 py-2 text-right font-semibold text-slate-700">التسلسل</th>
              <th className="px-3 py-2 text-right font-semibold text-slate-700">البند</th>
              <th className="px-3 py-2 text-right font-semibold text-slate-700">التقييم</th>
              <th className="px-3 py-2 text-right font-semibold text-slate-700">الملاحظات</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item, index) => (
              <tr key={item.sequence} className="border-b border-slate-100 last:border-0">
                <td className="px-3 py-3 text-center tabular-nums text-slate-600">
                  {item.sequence}
                </td>
                <td className="px-3 py-3 text-right font-medium text-slate-800">
                  {item.criterion}
                </td>
                <td className="px-3 py-3 min-w-[10rem]">
                  <FormField
                    control={form.control}
                    name={`items.${index}.rating`}
                    render={({ field }) => (
                      <FormItem>
                        <FormControl>
                          <select className={selectClassName} {...field}>
                            <option value="">— اختر —</option>
                            {CIRCLE_VISIT_RATING_VALUES.map((rating) => (
                              <option key={rating} value={rating}>
                                {rating}
                              </option>
                            ))}
                          </select>
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </td>
                <td className="px-3 py-3 min-w-[12rem]">
                  <FormField
                    control={form.control}
                    name={`items.${index}.notes`}
                    render={({ field }) => (
                      <FormItem>
                        <FormControl>
                          <Input placeholder="ملاحظات (اختياري)" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
