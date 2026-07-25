import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createTeacher,
  deleteTeacher,
  getTeacher,
  getTeacherMosques,
  updateTeacher,
} from '@/services/teacherService'
import type { SaveTeacherPayload } from '@/types/teacher'
import { TEACHERS_QUERY_KEY } from '@/hooks/useTeachers'

export function useTeacherForm(teacherId?: number) {
  const queryClient = useQueryClient()
  const isEdit = teacherId !== undefined

  const teacherQuery = useQuery({
    queryKey: ['teacher', teacherId],
    queryFn: () => getTeacher(teacherId!),
    enabled: isEdit,
  })

  const mosquesQuery = useQuery({
    queryKey: ['teacher', 'mosques'],
    queryFn: getTeacherMosques,
  })

  const invalidateList = () => {
    queryClient.invalidateQueries({ queryKey: TEACHERS_QUERY_KEY })
    if (isEdit) {
      queryClient.invalidateQueries({ queryKey: ['teacher', teacherId] })
    }
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveTeacherPayload) =>
      isEdit ? updateTeacher(teacherId!, payload) : createTeacher(payload),
    onSuccess: () => invalidateList(),
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteTeacher(teacherId!, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEACHERS_QUERY_KEY })
    },
  })

  return {
    isEdit,
    teacherQuery,
    mosquesQuery,
    saveMutation,
    deleteMutation,
  }
}
