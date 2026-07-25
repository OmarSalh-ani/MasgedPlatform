import { useCallback, useState } from 'react'
import { getHomeStudentQrToken } from '@/services/homeService'
import { downloadDataUrl, generateQrDataUrl } from '@/lib/qrCode'
import {
  STUDENT_QR_DEFAULT_COLOR,
  STUDENT_QR_FILENAME_PREFIX,
  STUDENT_QR_SIZE,
} from '@/types/qrGenerator'

function sanitizeFilename(value: string): string {
  return value.replace(/[^\w\u0600-\u06FF-]+/g, '-').replace(/-+/g, '-').replace(/^-|-$/g, '')
}

export function useStudentQrDownload() {
  const [isDownloading, setIsDownloading] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const downloadStudentQr = useCallback(async (studentId: number, studentName: string): Promise<boolean> => {
    setErrorMessage(null)
    setIsDownloading(true)

    try {
      const token = await getHomeStudentQrToken(studentId)
      const dataUrl = await generateQrDataUrl(token, STUDENT_QR_SIZE, STUDENT_QR_DEFAULT_COLOR)
      const safeName = sanitizeFilename(studentName) || String(studentId)
      downloadDataUrl(dataUrl, `${STUDENT_QR_FILENAME_PREFIX}-${studentId}-${safeName}.png`)
      return true
    } catch {
      setErrorMessage('تعذر إنشاء رمز QR')
      return false
    } finally {
      setIsDownloading(false)
    }
  }, [])

  return {
    isDownloading,
    errorMessage,
    downloadStudentQr,
  }
}
