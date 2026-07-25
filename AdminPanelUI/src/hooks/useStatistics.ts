import { useQuery } from '@tanstack/react-query'
import { getStatistics } from '@/services/statisticsService'

export function useStatistics() {
  return useQuery({
    queryKey: ['statistics'],
    queryFn: getStatistics,
  })
}
