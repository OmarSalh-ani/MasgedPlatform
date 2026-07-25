import { useQuery } from '@tanstack/react-query'
import { fetchTeacherCardPrint } from '@/services/teacherCardPrintService'

export function useTeacherCardPrint(id: number | undefined) {
  return useQuery({
    queryKey: ['teacherCardPrint', id],
    queryFn: () => fetchTeacherCardPrint(id!),
    enabled: Number.isFinite(id) && (id ?? 0) > 0,
    select: (response) => (response.success ? response.data : null),
  })
}
