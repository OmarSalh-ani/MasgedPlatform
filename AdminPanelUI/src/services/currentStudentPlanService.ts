import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  CurrentStudentPlanListItem,
  CurrentStudentPlanStudentLookup,
  CurrentStudentPlanStudentLookupFilters,
} from '@/types/currentStudentPlan'

export async function getCurrentStudentsPlansList(
  pageNumber: number,
  pageSize: number,
  studentId?: string,
): Promise<PagedResult<CurrentStudentPlanListItem>> {
  const { data } = await api.get<PagedResult<CurrentStudentPlanListItem>>(
    '/admincurrentstudentsplans',
    {
      params: {
        pageNumber,
        pageSize,
        studentId: studentId ? Number(studentId) : undefined,
      },
    },
  )
  return data
}

export async function getCurrentStudentsPlansStudents(
  filters: CurrentStudentPlanStudentLookupFilters,
): Promise<PagedResult<CurrentStudentPlanStudentLookup>> {
  const { data } = await api.get<PagedResult<CurrentStudentPlanStudentLookup>>(
    '/admincurrentstudentsplans/students',
    { params: filters },
  )
  return data
}

export async function deleteCurrentStudentPlan(id: number): Promise<boolean> {
  const { data } = await api.delete<ApiResponse<boolean>>(`/admincurrentstudentsplans/${id}`)
  return data.data
}
