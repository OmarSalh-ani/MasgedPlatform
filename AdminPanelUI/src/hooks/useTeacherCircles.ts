import { useQuery } from '@tanstack/react-query'
import { getTeacherCircles } from '@/services/teacherService'

export function useTeacherCircles(forGirls: boolean) {
  return useQuery({
    queryKey: ['teacher', 'circles', forGirls],
    queryFn: () => getTeacherCircles(forGirls),
  })
}
