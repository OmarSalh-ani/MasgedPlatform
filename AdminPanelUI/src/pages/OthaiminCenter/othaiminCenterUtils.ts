import type { SearchableDropdownOption } from '@/components/shared/SearchableDropdown'
import type { HomeFilters, HomeLookup, SelectedHomeStudent } from '@/types/home'
import type { HomeFilterFormState } from '@/pages/home/homeUtils'

export type { HomeFilterFormState }

export function buildAppliedOthaiminCenterFilters(
  form: HomeFilterFormState,
  pageNumber: number,
  pageSize: number,
  circleQuery?: number,
): HomeFilters {
  return {
    studentName: form.studentName.trim() || undefined,
    ageFrom: parseOptionalInt(form.ageFrom),
    ageTo: parseOptionalInt(form.ageTo),
    circleId: parseOptionalInt(form.circleId),
    fatherMobile: form.fatherMobile.trim() || undefined,
    womanActivityTypeId: parseOptionalInt(form.womanActivityTypeId),
    formStatus: form.formStatus || undefined,
    specialOnly: form.specialOnly || undefined,
    eliteOnly: form.eliteOnly || undefined,
    boysOnly: form.boysOnly || undefined,
    girlsOnly: form.girlsOnly || undefined,
    circleQuery,
    pageNumber,
    pageSize,
  }
}

export function getDefaultOthaiminCenterFilterForm(circleQuery?: number): HomeFilterFormState {
  return {
    studentName: '',
    ageFrom: '',
    ageTo: '',
    circleId: circleQuery ? String(circleQuery) : '',
    fatherMobile: '',
    womanActivityTypeId: '',
    formStatus: '',
    specialOnly: false,
    eliteOnly: false,
    boysOnly: false,
    girlsOnly: false,
  }
}

export function toOthaiminCenterLookupOptions(
  items: HomeLookup[] | undefined,
  allLabel: string,
): SearchableDropdownOption[] {
  return [
    { value: '', label: allLabel },
    ...(items ?? []).map((item) => ({ value: String(item.id), label: item.name })),
  ]
}

const SELECTED_STORAGE_KEY = 'othaiminCenterSelectedStudents'

export function loadOthaiminCenterSelectedStudents(): Map<number, SelectedHomeStudent> {
  try {
    const raw = sessionStorage.getItem(SELECTED_STORAGE_KEY)
    if (!raw) return new Map()
    const parsed = JSON.parse(raw) as SelectedHomeStudent[]
    return new Map(parsed.map((item) => [item.id, item]))
  } catch {
    return new Map()
  }
}

export function saveOthaiminCenterSelectedStudents(selected: Map<number, SelectedHomeStudent>) {
  sessionStorage.setItem(SELECTED_STORAGE_KEY, JSON.stringify([...selected.values()]))
}

export function clearOthaiminCenterSelectedStudentsStorage() {
  sessionStorage.removeItem(SELECTED_STORAGE_KEY)
}

function parseOptionalInt(value: string): number | undefined {
  if (!value.trim()) return undefined
  const parsed = Number(value)
  return Number.isNaN(parsed) ? undefined : parsed
}
