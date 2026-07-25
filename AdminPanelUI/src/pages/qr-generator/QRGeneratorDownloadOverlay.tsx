interface QRGeneratorDownloadOverlayProps {
  message: string
}

export function QRGeneratorDownloadOverlay({ message }: QRGeneratorDownloadOverlayProps) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm">
      <div className="mx-4 w-full max-w-xs rounded-2xl bg-white p-8 text-center shadow-xl">
        <div className="mx-auto mb-4 h-12 w-12 animate-spin rounded-full border-4 border-slate-200 border-t-[#7C8738]" />
        <p className="text-base font-semibold text-slate-800">{message}</p>
      </div>
    </div>
  )
}
