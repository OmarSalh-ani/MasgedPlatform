import { Download } from 'lucide-react'
import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { generateQrDataUrl } from '@/lib/qrCode'
import {
  QR_COLOR_OPTIONS,
  READY_QR_SIZE,
  type ReadyQrDefinition,
  type ReadyQrType,
} from '@/types/qrGenerator'

const selectClassName =
  'h-9 w-full rounded-md border-2 border-slate-200 bg-white px-2 text-sm focus:border-[#7C8738] focus:outline-none'

interface ReadyMadeQRCardProps {
  type: ReadyQrType
  definition: ReadyQrDefinition
  onDownload: (imageSrc: string, title: string) => void
}

export function ReadyMadeQRCard({ definition, onDownload }: ReadyMadeQRCardProps) {
  const [color, setColor] = useState(definition.defaultColor)
  const [imageSrc, setImageSrc] = useState('')

  useEffect(() => {
    let cancelled = false

    generateQrDataUrl(definition.url, READY_QR_SIZE, color)
      .then((dataUrl) => {
        if (!cancelled) setImageSrc(dataUrl)
      })
      .catch(() => {
        if (!cancelled) setImageSrc('')
      })

    return () => {
      cancelled = true
    }
  }, [definition.url, color])

  const colorOptions = definition.defaultColorOrder.map((value) =>
    QR_COLOR_OPTIONS.find((option) => option.value === value)
  ).filter((option): option is (typeof QR_COLOR_OPTIONS)[number] => Boolean(option))

  return (
    <article className="rounded-xl border-2 border-transparent bg-slate-50 p-5 text-center transition hover:-translate-y-1 hover:border-[#7C8738] hover:shadow-md">
      <h3 className="mb-2 text-lg font-semibold text-[#7C8738]">{definition.title}</h3>
      <p className="mb-4 text-sm text-slate-600">{definition.description}</p>

      <select
        className={`${selectClassName} mb-4`}
        value={color}
        onChange={(event) => setColor(event.target.value)}
        aria-label={`لون ${definition.title}`}
      >
        {colorOptions.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>

      <div className="mb-4 flex justify-center">
        {imageSrc ? (
          <img
            src={imageSrc}
            alt={definition.title}
            className="max-w-[200px] rounded-lg"
          />
        ) : (
          <div className="h-[200px] w-[200px] animate-pulse rounded-lg bg-slate-200" />
        )}
      </div>

      <Button
        type="button"
        disabled={!imageSrc}
        onClick={() => onDownload(imageSrc, definition.title)}
        className="gap-2 bg-gradient-to-br from-green-600 to-teal-500 px-4 py-2 text-sm"
      >
        <Download className="h-4 w-4" />
        حفظ
      </Button>
    </article>
  )
}
