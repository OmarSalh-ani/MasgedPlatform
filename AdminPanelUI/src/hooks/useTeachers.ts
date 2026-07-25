import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteTeacher, exportTeachersExcel, getTeachers } from '@/services/teacherService'

export const TEACHERS_QUERY_KEY = ['teachers'] as const

export function useTeachers() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: TEACHERS_QUERY_KEY,
    queryFn: getTeachers,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteTeacher(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: TEACHERS_QUERY_KEY })
    },
  })

  const exportMutation = useMutation({
    mutationFn: exportTeachersExcel,
  })

  return { query, deleteMutation, exportMutation }
}
