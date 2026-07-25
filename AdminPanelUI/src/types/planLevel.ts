export type PlanUnitTypeValue = 0 | 1 | 3

export interface PlanLevelListItem {
  id: number
  levelName: string
  unitType: PlanUnitTypeValue | number
  unitTypeDisplay: string
  quantity: number
  createdAt: string
  isGlobal: boolean
}

export interface PlanLevelDto extends PlanLevelListItem {}

export interface SavePlanLevelPayload {
  levelName: string
  unitType: PlanUnitTypeValue
  quantity: number
}

export const PLAN_UNIT_TYPE_OPTIONS: { value: PlanUnitTypeValue; label: string }[] = [
  { value: 0, label: 'صفحة' },
  { value: 1, label: 'ربع' },
  { value: 3, label: 'سطر' },
]

export function formatPlanLevelCreatedAt(value: string): string {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value

  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')

  return `${year}/${month}/${day} ${hours}:${minutes}`
}
