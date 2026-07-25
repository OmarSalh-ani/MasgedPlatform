import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { getAbout, saveAbout } from '@/services/aboutService'
import type { UpdateAboutRequest } from '@/types/about'

const ABOUT_QUERY_KEY = ['about'] as const

export function useAbout() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: ABOUT_QUERY_KEY,
    queryFn: getAbout,
  })

  const mutation = useMutation({
    mutationFn: (request: UpdateAboutRequest) => saveAbout(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ABOUT_QUERY_KEY })
    },
  })

  return { query, mutation }
}
