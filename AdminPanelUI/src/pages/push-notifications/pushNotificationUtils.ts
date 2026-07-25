import type { PushNotificationFilterForm, PushNotificationStudentFilters } from '@/types/pushNotification'

export function buildPushNotificationFilters(
  form: PushNotificationFilterForm,
  pageNumber: number,
  pageSize: number,
): PushNotificationStudentFilters {
  return {
    studentName: form.studentName.trim() || undefined,
    ageFrom: parseOptionalInt(form.ageFrom),
    ageTo: parseOptionalInt(form.ageTo),
    circleId: parseOptionalInt(form.circleId),
    fatherMobile: form.fatherMobile.trim() || undefined,
    formStatus: form.formStatus || undefined,
    specialOnly: form.specialOnly || undefined,
    boysOnly: form.boysOnly || undefined,
    girlsOnly: form.girlsOnly || undefined,
    pageNumber,
    pageSize,
  }
}

export function getDefaultPushNotificationFilterForm(): PushNotificationFilterForm {
  return {
    studentName: '',
    ageFrom: '',
    ageTo: '',
    circleId: '',
    fatherMobile: '',
    formStatus: '',
    specialOnly: false,
    boysOnly: false,
    girlsOnly: false,
  }
}

function parseOptionalInt(value: string): number | undefined {
  if (!value.trim()) return undefined
  const parsed = Number(value)
  return Number.isNaN(parsed) ? undefined : parsed
}
