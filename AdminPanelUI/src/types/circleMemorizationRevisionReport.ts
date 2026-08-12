export interface CircleMemorizationTeacherOption {
  id: number
  name: string
}

export type CircleReportExportFormat = 'pdf' | 'excel'

export function toCircleMemorizationTeacherDropdownOptions(
  teachers: CircleMemorizationTeacherOption[] | undefined,
) {
  return (teachers ?? []).map((teacher) => ({
    value: String(teacher.id),
    label: teacher.name,
  }))
}
