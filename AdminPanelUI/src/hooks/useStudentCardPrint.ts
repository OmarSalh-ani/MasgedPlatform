import { useQuery } from '@tanstack/react-query'
import { fetchStudentCardPrint } from '@/services/studentCardPrintService'

export function useStudentCardPrint(id: number | undefined) {
  return useQuery({
    queryKey: ['studentCardPrint', id],
    queryFn: () => fetchStudentCardPrint(id!),
    enabled: Number.isFinite(id) && (id ?? 0) > 0,
    select: (response) => (response.success ? response.data : null),
  })
}
