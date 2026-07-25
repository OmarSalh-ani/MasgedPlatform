import { useEffect, useState } from 'react'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { PageHeader } from '@/components/shared/PageHeader'
import { useQRGenerator } from '@/hooks/useQRGenerator'
import { QRGeneratorDownloadOverlay } from '@/pages/qr-generator/QRGeneratorDownloadOverlay'
import { QRGeneratorForm, type QRGeneratorFormValues } from '@/pages/qr-generator/QRGeneratorForm'
import { QRGeneratorResult } from '@/pages/qr-generator/QRGeneratorResult'
import { ReadyMadeQRSection } from '@/pages/qr-generator/ReadyMadeQRSection'

export function QRGeneratorPage() {
  const { masgedName } = useMasgedBranding()

  useEffect(() => {
    document.title = `مولد رمز QR - ${masgedName}`
  }, [masgedName])

  const [formResetKey, setFormResetKey] = useState(0)
  const {
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
  } = useQRGenerator()

  const handleGenerate = (values: QRGeneratorFormValues) => {
    void generateCustomQr(values)
  }

  const handleClear = () => {
    clearCustomQr()
    setFormResetKey((key) => key + 1)
  }

  return (
    <div>
      <PageHeader
        title="مولد رمز QR"
        description="إنشاء وحفظ رموز QR للروابط"
        gradientClassName="bg-gradient-to-br from-[#7C8738] to-[#1a5f8a]"
      />

      <QRGeneratorForm
        key={formResetKey}
        isGenerating={isGenerating}
        errorMessage={errorMessage}
        successMessage={successMessage}
        onSubmit={handleGenerate}
      />

      {customImageSrc && (
        <QRGeneratorResult
          imageSrc={customImageSrc}
          onDownload={() => void downloadCustomQr()}
          onClear={handleClear}
        />
      )}

      <ReadyMadeQRSection onDownload={(src, title) => void downloadReadyQr(src, title)} />

      {isDownloading && <QRGeneratorDownloadOverlay message={downloadMessage} />}
    </div>
  )
}
