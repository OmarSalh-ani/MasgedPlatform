import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteContactInfoFromList, getContactInfos } from '@/services/contactInfosService'

export const CONTACT_INFOS_QUERY_KEY = ['contactInfos'] as const

export function useContactInfos() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: CONTACT_INFOS_QUERY_KEY,
    queryFn: getContactInfos,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteContactInfoFromList(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CONTACT_INFOS_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
