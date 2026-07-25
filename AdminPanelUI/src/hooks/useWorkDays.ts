import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { getWorkDays, saveWorkDays } from '@/services/workDaysService'
import { WORK_DAYS_QUERY_KEY } from '@/types/workDays'

export function useWorkDays() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: WORK_DAYS_QUERY_KEY,
    queryFn: getWorkDays,
  })

  const mutation = useMutation({
    mutationFn: (dayNumbers: number[]) => saveWorkDays(dayNumbers),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: WORK_DAYS_QUERY_KEY })
    },
  })

  return { query, mutation }
}
