import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createActivity,
  deleteActivity,
  getActivity,
  getNextSortOrder,
  updateActivity,
} from '@/services/activityService'
import type { SaveActivityPayload } from '@/types/activity'
import { ACTIVITIES_QUERY_KEY } from '@/hooks/useActivities'

export function useActivityForm(activityId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = activityId !== undefined

  const activityQuery = useQuery({
    queryKey: ['activity', activityId],
    queryFn: () => getActivity(activityId!),
    enabled: isEdit,
  })

  const nextSortOrderQuery = useQuery({
    queryKey: ['activity', 'next-sort-order'],
    queryFn: getNextSortOrder,
    enabled: !isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: ACTIVITIES_QUERY_KEY })
    navigate('/activities')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveActivityPayload) =>
      isEdit ? updateActivity(activityId!, payload) : createActivity(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteActivity(activityId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    activityQuery,
    nextSortOrderQuery,
    saveMutation,
    deleteMutation,
  }
}
