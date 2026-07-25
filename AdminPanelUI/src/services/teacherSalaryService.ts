import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  AutoCalculateMonthResult,
  PaySelectedSalariesResult,
  SalaryCalculationResult,
  SaveTeacherSalaryPayload,
  TeacherSalary,
  TeacherSalaryFilterOptions,
  TeacherSalaryFormTeacher,
  TeacherSalaryListItem,
  TeacherSalaryReport,
} from '@/types/teacherSalary'

export interface TeacherSalaryListFilters {
  month?: number
  year?: number
  teacherId?: number
}

export async function getTeacherSalaryFilterOptions(): Promise<TeacherSalaryFilterOptions> {
  const { data } = await api.get<ApiResponse<TeacherSalaryFilterOptions>>(
    '/adminteachersalaries/filter-options',
  )
  return data.data
}

export async function getTeacherSalariesList(
  filters: TeacherSalaryListFilters,
): Promise<PagedResult<TeacherSalaryListItem>> {
  const { data } = await api.get<PagedResult<TeacherSalaryListItem>>('/adminteachersalaries', {
    params: filters,
  })
  return data
}

export async function getTeacherSalaryFormTeachers(): Promise<TeacherSalaryFormTeacher[]> {
  const { data } = await api.get<ApiResponse<TeacherSalaryFormTeacher[]>>(
    '/adminteachersalaries/form-teachers',
  )
  return data.data
}

export async function getTeacherSalary(id: number): Promise<TeacherSalary> {
  const { data } = await api.get<ApiResponse<TeacherSalary>>(`/adminteachersalaries/${id}`)
  return data.data
}

export async function createTeacherSalary(
  payload: SaveTeacherSalaryPayload,
): Promise<TeacherSalary> {
  const { data } = await api.post<ApiResponse<TeacherSalary>>('/adminteachersalaries', payload)
  return data.data
}

export async function updateTeacherSalary(
  id: number,
  payload: SaveTeacherSalaryPayload,
): Promise<TeacherSalary> {
  const { data } = await api.put<ApiResponse<TeacherSalary>>(
    `/adminteachersalaries/${id}`,
    payload,
  )
  return data.data
}

export async function deleteTeacherSalary(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/adminteachersalaries/${id}`)
  return data.data
}

export async function calculateTeacherSalary(payload: {
  teacherId: number
  month: number
  year: number
  baseSalary: number
  dayOffDate?: string | null
}): Promise<SalaryCalculationResult> {
  const { data } = await api.post<ApiResponse<SalaryCalculationResult>>(
    '/adminteachersalaries/calculate-salary',
    payload,
  )
  return data.data
}

export async function autoCalculateTeacherSalaries(payload: {
  month: number
  year: number
}): Promise<AutoCalculateMonthResult> {
  const { data } = await api.post<ApiResponse<AutoCalculateMonthResult>>(
    '/adminteachersalaries/auto-calculate',
    payload,
  )
  return data.data
}

export async function paySelectedTeacherSalaries(
  salaryIds: number[],
): Promise<PaySelectedSalariesResult> {
  const { data } = await api.post<ApiResponse<PaySelectedSalariesResult>>(
    '/adminteachersalaries/pay',
    { salaryIds },
  )
  return data.data
}

export async function getTeacherSalaryReport(
  month: number,
  year: number,
): Promise<TeacherSalaryReport> {
  const { data } = await api.get<ApiResponse<TeacherSalaryReport>>(
    '/adminteachersalaries/report',
    { params: { month, year } },
  )
  return data.data
}

export async function exportTeacherSalaryReport(month: number, year: number): Promise<Blob> {
  const { data } = await api.get<Blob>('/adminteachersalaries/report/export', {
    params: { month, year },
    responseType: 'blob',
  })
  return data
}
