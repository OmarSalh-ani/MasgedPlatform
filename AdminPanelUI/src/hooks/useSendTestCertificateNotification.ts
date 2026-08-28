import { useMutation } from '@tanstack/react-query'
import { sendTestCertificateNotification } from '@/services/testCertificateService'
import type { SendTestCertificateNotificationPayload } from '@/types/testCertificate'

export function useSendTestCertificateNotification(testId: number) {
  return useMutation({
    mutationFn: (payload: SendTestCertificateNotificationPayload) =>
      sendTestCertificateNotification(testId, payload).then((response) => response.data),
  })
}
