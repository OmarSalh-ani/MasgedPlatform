import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createCompetition,
  deleteCompetition,
  getCompetition,
  getNextSortOrder,
  updateCompetition,
} from '@/services/competitionService'
import type { SaveCompetitionPayload } from '@/types/competition'
import { COMPETITIONS_QUERY_KEY } from '@/hooks/useCompetitions'

export function useCompetitionForm(competitionId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = competitionId !== undefined

  const competitionQuery = useQuery({
    queryKey: ['competition', competitionId],
    queryFn: () => getCompetition(competitionId!),
    enabled: isEdit,
  })

  const nextSortOrderQuery = useQuery({
    queryKey: ['competition', 'next-sort-order'],
    queryFn: getNextSortOrder,
    enabled: !isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: COMPETITIONS_QUERY_KEY })
    navigate('/competitions')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveCompetitionPayload) =>
      isEdit ? updateCompetition(competitionId!, payload) : createCompetition(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteCompetition(competitionId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    competitionQuery,
    nextSortOrderQuery,
    saveMutation,
    deleteMutation,
  }
}
