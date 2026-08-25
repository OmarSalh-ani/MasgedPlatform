import api from '@/lib/axios'
import type {
  EventPageResponseFilters,
  EventPageResponsesPage,
} from '@/types/eventPageResponse'

export async function getEventPageResponses(
  filters: EventPageResponseFilters,
): Promise<EventPageResponsesPage> {
  const { data } = await api.get<EventPageResponsesPage>('/admineventpageresponses', {
    params: {
      activityName: filters.activityName || undefined,
      pageNumber: filters.pageNumber,
      pageSize: filters.pageSize,
    },
  })
  return data
}

export async function exportEventPageResponsesExcel(
  filters: Omit<EventPageResponseFilters, 'pageNumber' | 'pageSize'>,
): Promise<Blob> {
  const { data } = await api.get<Blob>('/admineventpageresponses/export/excel', {
    params: { activityName: filters.activityName || undefined },
    responseType: 'blob',
  })
  return data
}
