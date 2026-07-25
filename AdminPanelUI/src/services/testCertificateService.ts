import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type { TestCertificate } from '@/types/testCertificate'

export async function fetchTestCertificate(
  testId: number,
): Promise<ApiResponse<TestCertificate>> {
  const { data } = await api.get<ApiResponse<TestCertificate>>(
    `/admintestcertificate/${testId}`,
  )
  return data
}
