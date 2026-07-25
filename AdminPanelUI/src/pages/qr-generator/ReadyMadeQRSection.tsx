import { Star } from 'lucide-react'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { ReadyMadeQRCard } from '@/pages/qr-generator/ReadyMadeQRCard'
import { READY_QR_DEFINITIONS, type ReadyQrType } from '@/types/qrGenerator'

const READY_QR_TYPES: ReadyQrType[] = [
  'parent',
  'teacher',
  'mosque',
  'maleRegister',
  'femaleRegister',
]

interface ReadyMadeQRSectionProps {
  onDownload: (imageSrc: string, title: string) => void
}

export function ReadyMadeQRSection({ onDownload }: ReadyMadeQRSectionProps) {
  const { masgedName } = useMasgedBranding()

  const getDefinition = (type: ReadyQrType) => {
    const base = READY_QR_DEFINITIONS[type]
    if (type !== 'mosque') return base
    return { ...base, description: `الموقع الرسمي لـ${masgedName}` }
  }

  return (
    <section className="rounded-xl border bg-white p-6 shadow-sm">
      <h2 className="mb-5 flex items-center gap-2 text-xl font-semibold text-[#7C8738]">
        <Star className="h-5 w-5" />
        رموز QR جاهزة
      </h2>

      <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">
        {READY_QR_TYPES.map((type) => (
          <ReadyMadeQRCard
            key={type}
            type={type}
            definition={getDefinition(type)}
            onDownload={onDownload}
          />
        ))}
      </div>
    </section>
  )
}
