import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createExpensive,
  deleteExpensiveAttachment,
  downloadExpensiveAttachment,
  getExpensive,
  updateExpensive,
} from '@/services/expensivesService'
import { EXPENSIVES_QUERY_KEY, EXPENSIVES_SUMMARY_QUERY_KEY } from '@/hooks/useExpensives'
import type { ExpensiveFormMode, SaveExpensivePayload } from '@/types/expensives'

export function useExpensiveForm(expensiveId?: number, mode: ExpensiveFormMode = 'create') {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = mode === 'edit' && expensiveId !== undefined
  const isView = mode === 'view' && expensiveId !== undefined

  const expensiveQuery = useQuery({
    queryKey: ['expensives', expensiveId],
    queryFn: () => getExpensive(expensiveId!),
    enabled: expensiveId !== undefined,
  })

  const invalidateList = () => {
    queryClient.invalidateQueries({ queryKey: EXPENSIVES_QUERY_KEY })
    queryClient.invalidateQueries({ queryKey: EXPENSIVES_SUMMARY_QUERY_KEY })
    if (expensiveId !== undefined) {
      queryClient.invalidateQueries({ queryKey: ['expensives', expensiveId] })
    }
  }

  const goToList = () => navigate('/expensives')

  const saveMutation = useMutation({
    mutationFn: (payload: SaveExpensivePayload) =>
      isEdit ? updateExpensive(expensiveId!, payload) : createExpensive(payload),
    onSuccess: () => {
      invalidateList()
      goToList()
    },
  })

  const deleteAttachmentMutation = useMutation({
    mutationFn: (fileName: string) => deleteExpensiveAttachment(expensiveId!, fileName),
    onSuccess: invalidateList,
  })

  const downloadAttachment = (fileName: string) =>
    downloadExpensiveAttachment(expensiveId!, fileName)

  return {
    isEdit,
    isView,
    expensiveQuery,
    saveMutation,
    deleteAttachmentMutation,
    downloadAttachment,
    goToList,
  }
}
