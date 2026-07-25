import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteSocialLinkFromList, getSocialLinks } from '@/services/socialLinksService'

export const SOCIAL_LINKS_QUERY_KEY = ['socialLinks'] as const

export function useSocialLinks() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: SOCIAL_LINKS_QUERY_KEY,
    queryFn: getSocialLinks,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteSocialLinkFromList(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: SOCIAL_LINKS_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
