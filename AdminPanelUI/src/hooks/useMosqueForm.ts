import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createMosque,
  deleteMosque,
  getMosque,
  getNextMosqueSortOrder,
  updateMosque,
} from '@/services/mosqueService'
import type { SaveMosquePayload } from '@/types/mosque'
import { MOSQUES_QUERY_KEY } from '@/hooks/useMosques'

export function useMosqueForm(mosqueId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = mosqueId !== undefined

  const mosqueQuery = useQuery({
    queryKey: ['mosque', mosqueId],
    queryFn: () => getMosque(mosqueId!),
    enabled: isEdit,
  })

  const nextSortOrderQuery = useQuery({
    queryKey: ['mosque', 'next-sort-order'],
    queryFn: getNextMosqueSortOrder,
    enabled: !isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: MOSQUES_QUERY_KEY })
    navigate('/mosques')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveMosquePayload) =>
      isEdit ? updateMosque(mosqueId!, payload) : createMosque(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteMosque(mosqueId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    mosqueQuery,
    nextSortOrderQuery,
    saveMutation,
    deleteMutation,
  }
}
