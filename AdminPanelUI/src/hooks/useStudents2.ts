import { useQuery } from '@tanstack/react-query'
import { getStudents2 } from '@/services/students2Service'

export const STUDENTS2_QUERY_KEY = ['students2', 'list'] as const

export function useStudents2(search: string) {
  const listQuery = useQuery({
    queryKey: [...STUDENTS2_QUERY_KEY, search],
    queryFn: () => getStudents2(search),
  })

  return { listQuery }
}
