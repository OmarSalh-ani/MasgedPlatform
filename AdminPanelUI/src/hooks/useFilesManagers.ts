import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  deleteFilesManager,
  exportFilesManagersExcel,
  getFilesManagers,
} from '@/services/filesManagerService'

export const FILES_MANAGERS_QUERY_KEY = ['filesManagers'] as const

export function useFilesManagers() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: FILES_MANAGERS_QUERY_KEY,
    queryFn: getFilesManagers,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteFilesManager(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: FILES_MANAGERS_QUERY_KEY })
    },
  })

  const exportMutation = useMutation({
    mutationFn: exportFilesManagersExcel,
  })

  return { query, deleteMutation, exportMutation }
}
