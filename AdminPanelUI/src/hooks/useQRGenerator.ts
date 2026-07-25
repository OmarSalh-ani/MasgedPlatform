import { useCallback, useState } from 'react'
import { downloadDataUrl, generateQrDataUrl } from '@/lib/qrCode'
import { CUSTOM_QR_SIZE } from '@/types/qrGenerator'

interface GenerateQrInput {
  url: string
  color: string
}

export function useQRGenerator() {
  const [customImageSrc, setCustomImageSrc] = useState<string | null>(null)
  const [isGenerating, setIsGenerating] = useState(false)
  const [isDownloading, setIsDownloading] = useState(false)
  const [downloadMessage, setDownloadMessage] = useState('جاري الحفظ...')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  const clearMessages = useCallback(() => {
    setErrorMessage(null)
    setSuccessMessage(null)
  }, [])

  const generateCustomQr = useCallback(async ({ url, color }: GenerateQrInput) => {
    clearMessages()
    setIsGenerating(true)
    setCustomImageSrc(null)

    try {
      const dataUrl = await generateQrDataUrl(url, CUSTOM_QR_SIZE, color)
      setCustomImageSrc(dataUrl)
      setSuccessMessage('تم إنشاء رمز QR بنجاح')
    } catch {
      setErrorMessage('حدث خطأ أثناء إنشاء رمز QR')
    } finally {
      setIsGenerating(false)
    }
  }, [clearMessages])

  const clearCustomQr = useCallback(() => {
    setCustomImageSrc(null)
    clearMessages()
  }, [clearMessages])

  const downloadCustomQr = useCallback(async () => {
    if (!customImageSrc) return

    setDownloadMessage('جاري حفظ رمز QR...')
    setIsDownloading(true)
    await new Promise((resolve) => setTimeout(resolve, 500))
    downloadDataUrl(customImageSrc, 'qr-code.png')
    setIsDownloading(false)
  }, [customImageSrc])

  const downloadReadyQr = useCallback(async (imageSrc: string, title: string) => {
    if (!imageSrc) return

    setDownloadMessage(`جاري حفظ ${title}...`)
    setIsDownloading(true)
    await new Promise((resolve) => setTimeout(resolve, 500))
    downloadDataUrl(imageSrc, `${title}-qr.png`)
    setIsDownloading(false)
  }, [])

  return {
    customImageSrc,
    isGenerating,
    isDownloading,
    downloadMessage,
    errorMessage,
    successMessage,
    generateCustomQr,
    clearCustomQr,
    downloadCustomQr,
    downloadReadyQr,
    clearMessages,
  }
}
