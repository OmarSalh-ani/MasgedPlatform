import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  deleteCurrentStudentPlan,
  getCurrentStudentsPlansList,
  getCurrentStudentsPlansStudents,
} from '@/services/currentStudentPlanService'
import type {
  CurrentStudentPlanFilters,
  CurrentStudentPlanStudentLookupFilters,
} from '@/types/currentStudentPlan'

export const CURRENT_STUDENTS_PLANS_QUERY_KEY = ['current-students-plans', 'list'] as const

export function useCurrentStudentsPlanStudents(
  filters: CurrentStudentPlanStudentLookupFilters | null,
) {
  return useQuery({
    queryKey: ['current-students-plans', 'students', filters],
    queryFn: () => getCurrentStudentsPlansStudents(filters!),
    enabled: filters != null,
  })
}

export function useCurrentStudentsPlans(appliedFilters: CurrentStudentPlanFilters | null) {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: [...CURRENT_STUDENTS_PLANS_QUERY_KEY, appliedFilters],
    queryFn: () =>
      getCurrentStudentsPlansList(
        appliedFilters!.pageNumber,
        appliedFilters!.pageSize,
        appliedFilters!.studentId || undefined,
      ),
    enabled: appliedFilters != null,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteCurrentStudentPlan(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CURRENT_STUDENTS_PLANS_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
