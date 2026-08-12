import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  deleteCircle,
  deleteCirclePlans,
  exportCirclesExcel,
  getCircles,
} from '@/services/circlesService'

export const CIRCLES_QUERY_KEY = ['circles'] as const

export function useCircles(teacherId?: number) {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: [...CIRCLES_QUERY_KEY, teacherId ?? 'all'],
    queryFn: () => getCircles(teacherId),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteCircle(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CIRCLES_QUERY_KEY })
    },
  })

  const deletePlansMutation = useMutation({
    mutationFn: (circleIds: number[]) => deleteCirclePlans(circleIds),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CIRCLES_QUERY_KEY })
    },
  })

  const exportMutation = useMutation({
    mutationFn: exportCirclesExcel,
    onSuccess: (blob) => {
      const url = URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = `Circles_${new Date().toISOString().slice(0, 19).replace(/[-:T]/g, '')}.xlsx`
      link.click()
      URL.revokeObjectURL(url)
    },
  })

  return { query, deleteMutation, deletePlansMutation, exportMutation }
}
