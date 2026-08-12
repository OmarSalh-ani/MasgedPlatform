import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { downloadBlob } from '@/lib/download'
import {
  createCircleVisitRating,
  exportCircleVisitRatingPdf,
  getCircleVisitNumber,
  getCircleVisitRatingCircles,
  getCircleVisitRatings,
  getCircleVisitRatingTeachers,
} from '@/services/circleVisitRatingService'
import type { CreateCircleVisitRatingPayload } from '@/types/circleVisitRating'

export const CIRCLE_VISIT_RATINGS_LIST_KEY = ['circle-visit-ratings', 'list'] as const

export function useCircleVisitRatings(pageNumber: number) {
  const query = useQuery({
    queryKey: [...CIRCLE_VISIT_RATINGS_LIST_KEY, pageNumber],
    queryFn: () => getCircleVisitRatings(pageNumber),
  })

  const exportMutation = useMutation({
    mutationFn: (id: number) => exportCircleVisitRatingPdf(id),
    onSuccess: (blob, id) => {
      const stamp = new Date().toISOString().slice(0, 19).replace(/[-:T]/g, '')
      downloadBlob(blob, `تقييم_حلقة_${id}_${stamp}.pdf`)
    },
    onError: () => {
      window.alert('تعذر تصدير التقرير. يرجى المحاولة مرة أخرى.')
    },
  })

  return { query, exportMutation }
}

export function useCircleVisitRatingTeachers() {
  return useQuery({
    queryKey: ['circle-visit-ratings', 'teachers'],
    queryFn: getCircleVisitRatingTeachers,
  })
}

export function useCircleVisitRatingCircles(teacherId: number | null) {
  return useQuery({
    queryKey: ['circle-visit-ratings', 'circles', teacherId],
    queryFn: () => getCircleVisitRatingCircles(teacherId!),
    enabled: teacherId != null && teacherId > 0,
  })
}

export function useCircleVisitNumber(teacherId: number | null, visitDate: string) {
  return useQuery({
    queryKey: ['circle-visit-ratings', 'visit-number', teacherId, visitDate],
    queryFn: () => getCircleVisitNumber(teacherId!, visitDate),
    enabled: teacherId != null && teacherId > 0 && Boolean(visitDate),
  })
}

export function useCreateCircleVisitRating() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateCircleVisitRatingPayload) => createCircleVisitRating(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: CIRCLE_VISIT_RATINGS_LIST_KEY })
    },
  })
}
