import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createTip,
  deleteTip,
  getTip,
  getNextSortOrder,
  updateTip,
} from '@/services/tipService'
import type { SaveTipPayload } from '@/types/tip'
import { TIPS_QUERY_KEY } from '@/hooks/useTips'

export function useTipForm(tipId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = tipId !== undefined

  const tipQuery = useQuery({
    queryKey: ['tip', tipId],
    queryFn: () => getTip(tipId!),
    enabled: isEdit,
  })

  const nextSortOrderQuery = useQuery({
    queryKey: ['tip', 'next-sort-order'],
    queryFn: getNextSortOrder,
    enabled: !isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: TIPS_QUERY_KEY })
    navigate('/tips')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveTipPayload) =>
      isEdit ? updateTip(tipId!, payload) : createTip(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteTip(tipId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    tipQuery,
    nextSortOrderQuery,
    saveMutation,
    deleteMutation,
  }
}
