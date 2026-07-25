import type { SearchableDropdownOption } from '@/components/shared/SearchableDropdown'

export interface MemorizationRevisionStudentPick {
  id: number
  studentName: string
  label: string
}

export interface MemorizationRevisionPlanRow {
  status: string
  surahNameAr: string
  studentName: string
  fromAyah: number
  toAyah: number
  planType: string
}

export interface MemorizationRevisionReport {
  studentId: number
  studentName: string
  rows: MemorizationRevisionPlanRow[]
}

export function toMemorizationRevisionStudentDropdownOptions(
  students: MemorizationRevisionStudentPick[] | undefined,
): SearchableDropdownOption[] {
  return (students ?? []).map((student) => ({
    value: String(student.id),
    label: student.label,
  }))
}
