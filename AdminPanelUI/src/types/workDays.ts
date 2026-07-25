import { z } from 'zod'

export interface WorkDayLabel {
  number: number
  nameAr: string
  enabled: boolean
}

export interface WorkDays {
  dayNumbers: number[]
  dayLabels: WorkDayLabel[]
}

export const workDaysSchema = z.object({
  dayNumbers: z.array(z.number().int().min(0).max(6)).min(1, 'يجب اختيار يوم عمل واحد على الأقل'),
})

export type WorkDaysFormValues = z.infer<typeof workDaysSchema>

export const WORK_DAYS_QUERY_KEY = ['workDays'] as const
