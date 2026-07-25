import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  autoCalculateTeacherSalaries,
  deleteTeacherSalary,
  getTeacherSalariesList,
  getTeacherSalaryFilterOptions,
  paySelectedTeacherSalaries,
  type TeacherSalaryListFilters,
} from '@/services/teacherSalaryService'

export const TEACHER_SALARIES_QUERY_KEY = ['teacher-salaries'] as const

export function useTeacherSalaries(filters: TeacherSalaryListFilters) {
  const queryClient = useQueryClient()

  const filterOptionsQuery = useQuery({
    queryKey: [...TEACHER_SALARIES_QUERY_KEY, 'filter-options'],
    queryFn: getTeacherSalaryFilterOptions,
  })

  const listQuery = useQuery({
    queryKey: [...TEACHER_SALARIES_QUERY_KEY, 'list', filters],
    queryFn: () => getTeacherSalariesList(filters),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteTeacherSalary(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEACHER_SALARIES_QUERY_KEY })
    },
  })

  const autoCalculateMutation = useMutation({
    mutationFn: (payload: { month: number; year: number }) =>
      autoCalculateTeacherSalaries(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEACHER_SALARIES_QUERY_KEY })
    },
  })

  const payMutation = useMutation({
    mutationFn: (salaryIds: number[]) => paySelectedTeacherSalaries(salaryIds),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEACHER_SALARIES_QUERY_KEY })
    },
  })

  return {
    filterOptionsQuery,
    listQuery,
    deleteMutation,
    autoCalculateMutation,
    payMutation,
  }
}
