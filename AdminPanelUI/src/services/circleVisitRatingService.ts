import api from '@/lib/axios'
import type { ApiResponse, PagedResult } from '@/types/api'
import type {
  CircleVisitRatingCircleOption,
  CircleVisitRatingDetail,
  CircleVisitRatingListItem,
  CircleVisitRatingTeacherOption,
  CreateCircleVisitRatingPayload,
} from '@/types/circleVisitRating'

const BASE = '/admincirclevitrating'

export async function getCircleVisitRatings(
  pageNumber: number,
  pageSize = 15,
): Promise<PagedResult<CircleVisitRatingListItem>> {
  const { data } = await api.get<PagedResult<CircleVisitRatingListItem>>(BASE, {
    params: { pageNumber, pageSize },
  })
  return data
}

export async function getCircleVisitRatingTeachers(): Promise<
  CircleVisitRatingTeacherOption[]
> {
  const { data } = await api.get<ApiResponse<CircleVisitRatingTeacherOption[]>>(
    `${BASE}/teachers`,
  )
  return data.data
}

export async function getCircleVisitRatingCircles(
  teacherId: number,
): Promise<CircleVisitRatingCircleOption[]> {
  const { data } = await api.get<ApiResponse<CircleVisitRatingCircleOption[]>>(
    `${BASE}/circles`,
    { params: { teacherId } },
  )
  return data.data
}

export async function getCircleVisitNumber(
  teacherId: number,
  visitDate: string,
): Promise<number> {
  const { data } = await api.get<ApiResponse<{ visitNumber: number }>>(
    `${BASE}/visit-number`,
    { params: { teacherId, visitDate } },
  )
  return data.data.visitNumber
}

export async function createCircleVisitRating(
  payload: CreateCircleVisitRatingPayload,
): Promise<CircleVisitRatingDetail> {
  const { data } = await api.post<ApiResponse<CircleVisitRatingDetail>>(BASE, payload)
  return data.data
}

export async function exportCircleVisitRatingPdf(id: number): Promise<Blob> {
  const { data } = await api.get<Blob>(`${BASE}/${id}/export-pdf`, {
    responseType: 'blob',
  })
  return data
}
