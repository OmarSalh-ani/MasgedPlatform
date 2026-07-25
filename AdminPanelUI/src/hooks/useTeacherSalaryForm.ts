import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  calculateTeacherSalary,
  createTeacherSalary,
  getTeacherSalary,
  getTeacherSalaryFormTeachers,
  updateTeacherSalary,
} from '@/services/teacherSalaryService'
import type { SaveTeacherSalaryPayload } from '@/types/teacherSalary'
import { TEACHER_SALARIES_QUERY_KEY } from '@/hooks/useTeacherSalaries'

export function useTeacherSalaryForm(id?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = id !== undefined

  const salaryQuery = useQuery({
    queryKey: [...TEACHER_SALARIES_QUERY_KEY, 'detail', id],
    queryFn: () => getTeacherSalary(id!),
    enabled: isEdit,
  })

  const teachersQuery = useQuery({
    queryKey: [...TEACHER_SALARIES_QUERY_KEY, 'form-teachers'],
    queryFn: getTeacherSalaryFormTeachers,
    enabled: !isEdit,
  })

  const calculateMutation = useMutation({
    mutationFn: calculateTeacherSalary,
  })

  const createMutation = useMutation({
    mutationFn: (payload: SaveTeacherSalaryPayload) => createTeacherSalary(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEACHER_SALARIES_QUERY_KEY })
      navigate('/teacher-salaries')
    },
  })

  const updateMutation = useMutation({
    mutationFn: (payload: SaveTeacherSalaryPayload) => updateTeacherSalary(id!, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEACHER_SALARIES_QUERY_KEY })
      navigate('/teacher-salaries')
    },
  })

  return {
    salaryQuery,
    teachersQuery,
    calculateMutation,
    createMutation,
    updateMutation,
  }
}
