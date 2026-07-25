import { Download, Image, X } from 'lucide-react'
import { useEffect, useRef } from 'react'
import { Button } from '@/components/ui/button'

interface QRGeneratorResultProps {
  imageSrc: string
  onDownload: () => void
  onClear: () => void
}

export function QRGeneratorResult({ imageSrc, onDownload, onClear }: QRGeneratorResultProps) {
  const sectionRef = useRef<HTMLElement>(null)

  useEffect(() => {
    sectionRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [imageSrc])

  return (
    <section
      ref={sectionRef}
      className="mb-8 rounded-xl border bg-white p-6 text-center shadow-sm"
    >
      <h2 className="mb-5 flex items-center justify-center gap-2 text-xl font-semibold text-[#7C8738]">
        <Image className="h-5 w-5" />
        رمز QR المُنشأ
      </h2>

      <div className="inline-block rounded-xl bg-white p-5 shadow-md">
        <img src={imageSrc} alt="QR Code" className="mx-auto max-w-[300px] rounded-lg" />
      </div>

      <div className="mt-5 flex flex-wrap justify-center gap-3">
        <Button
          type="button"
          onClick={onDownload}
          className="gap-2 bg-gradient-to-br from-green-600 to-teal-500"
        >
          <Download className="h-4 w-4" />
          حفظ كـ PNG
        </Button>
        <Button type="button" variant="outline" onClick={onClear} className="gap-2">
          <X className="h-4 w-4" />
          مسح
        </Button>
      </div>
    </section>
  )
}
