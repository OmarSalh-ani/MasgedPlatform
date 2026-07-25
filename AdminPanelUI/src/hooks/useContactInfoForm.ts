import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import {
  createContactInfo,
  deleteContactInfo,
  getContactInfo,
  getNextContactInfoSortOrder,
  updateContactInfo,
} from '@/services/contactInfoService'
import type { SaveContactInfoPayload } from '@/types/contactInfo'
import { CONTACT_INFOS_QUERY_KEY } from '@/hooks/useContactInfos'

export function useContactInfoForm(contactInfoId?: number) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = contactInfoId !== undefined

  const contactInfoQuery = useQuery({
    queryKey: ['contactInfo', contactInfoId],
    queryFn: () => getContactInfo(contactInfoId!),
    enabled: isEdit,
  })

  const nextSortOrderQuery = useQuery({
    queryKey: ['contactInfo', 'next-sort-order'],
    queryFn: getNextContactInfoSortOrder,
    enabled: !isEdit,
  })

  const invalidateAndGoToList = () => {
    queryClient.invalidateQueries({ queryKey: CONTACT_INFOS_QUERY_KEY })
    navigate('/contact-info')
  }

  const saveMutation = useMutation({
    mutationFn: (payload: SaveContactInfoPayload) =>
      isEdit ? updateContactInfo(contactInfoId!, payload) : createContactInfo(payload),
    onSuccess: invalidateAndGoToList,
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteContactInfo(contactInfoId!),
    onSuccess: invalidateAndGoToList,
  })

  return {
    isEdit,
    contactInfoQuery,
    nextSortOrderQuery,
    saveMutation,
    deleteMutation,
  }
}
