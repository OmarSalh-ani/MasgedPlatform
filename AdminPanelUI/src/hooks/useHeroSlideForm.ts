import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import { useNavigate } from 'react-router-dom'

import {

  createHeroSlide,

  deleteHeroSlide,

  getHeroSlide,

  getNextHeroSlideSortOrder,

  updateHeroSlide,

} from '@/services/heroSlidesService'

import type { SaveHeroSlidePayload } from '@/types/heroSlide'

import { HERO_SLIDES_QUERY_KEY } from '@/hooks/useHeroSlides'



export function useHeroSlideForm(heroSlideId?: number) {

  const navigate = useNavigate()

  const queryClient = useQueryClient()

  const isEdit = heroSlideId !== undefined



  const heroSlideQuery = useQuery({

    queryKey: ['heroSlide', heroSlideId],

    queryFn: () => getHeroSlide(heroSlideId!),

    enabled: isEdit,

  })



  const nextSortOrderQuery = useQuery({

    queryKey: ['heroSlide', 'next-sort-order'],

    queryFn: getNextHeroSlideSortOrder,

    enabled: !isEdit,

  })



  const invalidateAndGoToList = () => {

    queryClient.invalidateQueries({ queryKey: HERO_SLIDES_QUERY_KEY })

    navigate('/hero-slides')

  }



  const saveMutation = useMutation({

    mutationFn: (payload: SaveHeroSlidePayload) =>

      isEdit ? updateHeroSlide(heroSlideId!, payload) : createHeroSlide(payload),

    onSuccess: invalidateAndGoToList,

  })



  const deleteMutation = useMutation({

    mutationFn: () => deleteHeroSlide(heroSlideId!, true),

    onSuccess: invalidateAndGoToList,

  })



  return {

    isEdit,

    heroSlideQuery,

    nextSortOrderQuery,

    saveMutation,

    deleteMutation,

  }

}

