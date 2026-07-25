import type { SearchableDropdownOption } from '@/components/shared/SearchableDropdown'
import type { HomeFilters, HomeLookup, SelectedHomeStudent } from '@/types/home'

export interface HomeFilterFormState {
  studentName: string
  ageFrom: string
  ageTo: string
  circleId: string
  fatherMobile: string
  womanActivityTypeId: string
  formStatus: string
  specialOnly: boolean
  eliteOnly: boolean
  boysOnly: boolean
  girlsOnly: boolean
}

export function buildAppliedHomeFilters(
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

export function getDefaultHomeFilterForm(circleQuery?: number): HomeFilterFormState {
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

export function toHomeLookupDropdownOptions(
  items: HomeLookup[] | undefined,
  allLabel: string,
): SearchableDropdownOption[] {
  return [
    { value: '', label: allLabel },
    ...(items ?? []).map((item) => ({ value: String(item.id), label: item.name })),
  ]
}

const SELECTED_STORAGE_KEY = 'homeSelectedStudents'

export function loadSelectedStudents(): Map<number, SelectedHomeStudent> {
  try {
    const raw = sessionStorage.getItem(SELECTED_STORAGE_KEY)
    if (!raw) return new Map()
    const parsed = JSON.parse(raw) as SelectedHomeStudent[]
    return new Map(parsed.map((item) => [item.id, item]))
  } catch {
    return new Map()
  }
}

export function saveSelectedStudents(selected: Map<number, SelectedHomeStudent>) {
  sessionStorage.setItem(SELECTED_STORAGE_KEY, JSON.stringify([...selected.values()]))
}

export function clearSelectedStudentsStorage() {
  sessionStorage.removeItem(SELECTED_STORAGE_KEY)
}

export type HomeStudentsLayout = 'grid' | 'list'

const LAYOUT_STORAGE_KEY = 'homeStudentsLayout'

export function loadHomeStudentsLayout(): HomeStudentsLayout {
  try {
    const value = localStorage.getItem(LAYOUT_STORAGE_KEY)
    return value === 'list' ? 'list' : 'grid'
  } catch {
    return 'grid'
  }
}

export function saveHomeStudentsLayout(layout: HomeStudentsLayout) {
  localStorage.setItem(LAYOUT_STORAGE_KEY, layout)
}

function parseOptionalInt(value: string): number | undefined {
  if (!value.trim()) return undefined
  const parsed = Number(value)
  return Number.isNaN(parsed) ? undefined : parsed
}
