import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createFilesManager,
  deleteFilesManager,
  getFilesManager,
  updateFilesManager,
} from '@/services/filesManagerService'
import type { SaveFilesManagerPayload } from '@/types/filesManager'
import { FILES_MANAGERS_QUERY_KEY } from '@/hooks/useFilesManagers'

export function useFilesManagerForm(filesManagerId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = filesManagerId !== undefined

  const filesManagerQuery = useQuery({
    queryKey: ['filesManager', filesManagerId],
    queryFn: () => getFilesManager(filesManagerId!),
    enabled: isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: FILES_MANAGERS_QUERY_KEY })
    navigate('/files-manager')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveFilesManagerPayload) =>
      isEdit ? updateFilesManager(filesManagerId!, payload) : createFilesManager(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteFilesManager(filesManagerId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    filesManagerQuery,
    saveMutation,
    deleteMutation,
  }
}
