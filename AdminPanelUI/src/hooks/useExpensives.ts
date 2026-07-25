import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  deleteExpensive,
  exportExpensivesToExcel,
  getExpensiveSummary,
  getExpensives,
} from '@/services/expensivesService'

export const EXPENSIVES_QUERY_KEY = ['expensives'] as const
export const EXPENSIVES_SUMMARY_QUERY_KEY = ['expensives', 'summary'] as const

export function useExpensives() {
  const queryClient = useQueryClient()

  const listQuery = useQuery({
    queryKey: EXPENSIVES_QUERY_KEY,
    queryFn: getExpensives,
  })

  const summaryQuery = useQuery({
    queryKey: EXPENSIVES_SUMMARY_QUERY_KEY,
    queryFn: getExpensiveSummary,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteExpensive(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: EXPENSIVES_QUERY_KEY })
      queryClient.invalidateQueries({ queryKey: EXPENSIVES_SUMMARY_QUERY_KEY })
    },
  })

  const exportMutation = useMutation({
    mutationFn: () => exportExpensivesToExcel(),
  })

  return { listQuery, summaryQuery, deleteMutation, exportMutation }
}
