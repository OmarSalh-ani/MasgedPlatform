import type {
  SelectedWhatsappSenderStudent,
  WhatsappSenderFilterForm,
  WhatsappSenderFilters,
} from '@/types/whatsappSender'

export function buildWhatsappSenderFilters(
  form: WhatsappSenderFilterForm,
  pageNumber: number,
  pageSize: number,
): WhatsappSenderFilters {
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

export function getDefaultWhatsappSenderFilterForm(): WhatsappSenderFilterForm {
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

const SELECTED_STORAGE_KEY = 'whatsappSenderSelectedStudents'

export function loadWhatsappSenderSelection() {
  try {
    const raw = sessionStorage.getItem(SELECTED_STORAGE_KEY)
    if (!raw) return new Map<number, import('@/types/whatsappSender').SelectedWhatsappSenderStudent>()
    const parsed = JSON.parse(raw) as import('@/types/whatsappSender').SelectedWhatsappSenderStudent[]
    return new Map(parsed.map((item) => [item.id, item]))
  } catch {
    return new Map()
  }
}

export function saveWhatsappSenderSelection(
  selected: Map<number, import('@/types/whatsappSender').SelectedWhatsappSenderStudent>,
) {
  sessionStorage.setItem(SELECTED_STORAGE_KEY, JSON.stringify([...selected.values()]))
}

export function clearWhatsappSenderSelectionStorage() {
  sessionStorage.removeItem(SELECTED_STORAGE_KEY)
}

function parseOptionalInt(value: string): number | undefined {
  if (!value.trim()) return undefined
  const parsed = Number(value)
  return Number.isNaN(parsed) ? undefined : parsed
}

const ARABIC_INDIC_DIGITS = /[\u0660-\u0669]/
const EASTERN_ARABIC_INDIC_DIGITS = /[\u06F0-\u06F9]/

export function containsArabicDigits(value: string | null | undefined): boolean {
  if (!value) return false
  return ARABIC_INDIC_DIGITS.test(value) || EASTERN_ARABIC_INDIC_DIGITS.test(value)
}

export function hasArabicPhoneNumber(student: { fatherPhone?: string | null }): boolean {
  return containsArabicDigits(student.fatherPhone)
}

export function filterValidWhatsappStudents(
  students: SelectedWhatsappSenderStudent[],
): SelectedWhatsappSenderStudent[] {
  return students.filter((student) => !hasArabicPhoneNumber(student))
}

export function partitionByArabicPhone(
  students: SelectedWhatsappSenderStudent[],
): {
  valid: SelectedWhatsappSenderStudent[]
  skipped: SelectedWhatsappSenderStudent[]
} {
  const valid: SelectedWhatsappSenderStudent[] = []
  const skipped: SelectedWhatsappSenderStudent[] = []
  for (const student of students) {
    if (hasArabicPhoneNumber(student)) skipped.push(student)
    else valid.push(student)
  }
  return { valid, skipped }
}
