import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { deleteHeroSlide, getHeroSlides } from '@/services/heroSlidesService'

export const HERO_SLIDES_QUERY_KEY = ['heroSlides'] as const

export function useHeroSlides() {
  const queryClient = useQueryClient()

  const query = useQuery({
    queryKey: HERO_SLIDES_QUERY_KEY,
    queryFn: getHeroSlides,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => deleteHeroSlide(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: HERO_SLIDES_QUERY_KEY })
    },
  })

  return { query, deleteMutation }
}
