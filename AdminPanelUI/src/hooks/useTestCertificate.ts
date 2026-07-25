import { useQuery } from '@tanstack/react-query'
import { fetchTestCertificate } from '@/services/testCertificateService'

export function useTestCertificate(testId: number | undefined) {
  return useQuery({
    queryKey: ['testCertificate', testId],
    queryFn: () => fetchTestCertificate(testId!),
    enabled: Number.isFinite(testId) && (testId ?? 0) > 0,
    select: (response) => (response.success ? response.data : null),
  })
}
