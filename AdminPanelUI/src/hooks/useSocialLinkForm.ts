import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createSocialLink,
  deleteSocialLink,
  getNextSocialLinkSortOrder,
  getSocialLink,
  updateSocialLink,
} from '@/services/socialLinkService'
import type { SaveSocialLinkPayload } from '@/types/socialLink'
import { SOCIAL_LINKS_QUERY_KEY } from '@/hooks/useSocialLinks'

export function useSocialLinkForm(socialLinkId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = socialLinkId !== undefined

  const socialLinkQuery = useQuery({
    queryKey: ['socialLink', socialLinkId],
    queryFn: () => getSocialLink(socialLinkId!),
    enabled: isEdit,
  })

  const nextSortOrderQuery = useQuery({
    queryKey: ['socialLink', 'next-sort-order'],
    queryFn: getNextSocialLinkSortOrder,
    enabled: !isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: SOCIAL_LINKS_QUERY_KEY })
    navigate('/social-links')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveSocialLinkPayload) =>
      isEdit ? updateSocialLink(socialLinkId!, payload) : createSocialLink(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteSocialLink(socialLinkId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    socialLinkQuery,
    nextSortOrderQuery,
    saveMutation,
    deleteMutation,
  }
}
