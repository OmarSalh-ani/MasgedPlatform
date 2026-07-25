import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createStudent,
  getStudent,
  getStudentFormData,
  updateStudent,
} from '@/services/studentService'
import { HOME_QUERY_KEY } from '@/hooks/useHome'
import type { SaveStudentPayload } from '@/types/student'

export const STUDENT_FORM_DATA_KEY = ['student', 'form-data'] as const

export function useStudent(studentId?: number) {
  const queryClient = useQueryClient()
  const isEdit = studentId !== undefined

  const formDataQuery = useQuery({
    queryKey: STUDENT_FORM_DATA_KEY,
    queryFn: getStudentFormData,
  })

  const studentQuery = useQuery({
    queryKey: ['student', studentId],
    queryFn: () => getStudent(studentId!),
    enabled: isEdit,
  })

  const saveMutation = useMutation({
    mutationFn: (payload: SaveStudentPayload) =>
      isEdit ? updateStudent(studentId!, payload) : createStudent(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: HOME_QUERY_KEY })
      if (isEdit) {
        queryClient.invalidateQueries({ queryKey: ['student', studentId] })
      }
    },
  })

  return {
    isEdit,
    formDataQuery,
    studentQuery,
    saveMutation,
  }
}
