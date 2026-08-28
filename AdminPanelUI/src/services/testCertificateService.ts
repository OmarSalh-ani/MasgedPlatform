import api from '@/lib/axios'
import type { ApiResponse } from '@/types/api'
import type {
  SendTestCertificateNotificationPayload,
  SendTestCertificateNotificationResult,
  TestCertificate,
} from '@/types/testCertificate'

export async function fetchTestCertificate(
  testId: number,
): Promise<ApiResponse<TestCertificate>> {
  const { data } = await api.get<ApiResponse<TestCertificate>>(
    `/admintestcertificate/${testId}`,
  )
  return data
}

export async function sendTestCertificateNotification(
  testId: number,
  payload: SendTestCertificateNotificationPayload,
): Promise<ApiResponse<SendTestCertificateNotificationResult>> {
  const { data } = await api.post<ApiResponse<SendTestCertificateNotificationResult>>(
    `/admintestcertificate/${testId}/notify`,
    payload,
  )
  return data
}
