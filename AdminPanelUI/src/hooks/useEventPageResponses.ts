import { keepPreviousData, useMutation, useQuery } from '@tanstack/react-query'
import { downloadBlob } from '@/lib/download'
import { getEventPageLookups } from '@/services/eventPagesService'
import {
  exportEventPageResponsesExcel,
  getEventPageResponses,
} from '@/services/eventPageResponsesService'
import type { EventPageResponseFilters } from '@/types/eventPageResponse'

export const EVENT_PAGE_RESPONSES_QUERY_KEY = ['event-page-responses'] as const

export function useEventPageLookups() {
  return useQuery({
    queryKey: ['event-pages', 'lookups'],
    queryFn: getEventPageLookups,
  })
}

export function useEventPageResponses(filters: EventPageResponseFilters) {
  const listQuery = useQuery({
    queryKey: [...EVENT_PAGE_RESPONSES_QUERY_KEY, filters],
    queryFn: () => getEventPageResponses(filters),
    placeholderData: keepPreviousData,
  })

  const exportMutation = useMutation({
    mutationFn: () =>
      exportEventPageResponsesExcel({ activityName: filters.activityName }),
    onSuccess: (blob) => {
      downloadBlob(blob, `EventPageResponses_${new Date().toISOString().slice(0, 10)}.xlsx`)
    },
  })

  return { listQuery, exportMutation }
}
