import { Download, Share, X } from 'lucide-react'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { usePwaInstallPrompt } from '@/hooks/usePwaInstallPrompt'
import { PWA_APP_NAME } from '@/lib/pwaConstants'
import { cn } from '@/lib/utils'

export function PwaInstallBanner() {
  const { logoUrl } = useMasgedBranding()
  const { showBanner, canInstall, isIos, promptInstall, dismiss } = usePwaInstallPrompt()

  if (!showBanner) return null

  return (
    <div
      className={cn(
        'fixed inset-x-0 bottom-0 z-50 px-3 pb-3 md:bottom-auto md:top-16 md:px-6',
      )}
      role="region"
      aria-label="تثبيت التطبيق"
    >
      <Alert className="mx-auto flex max-w-4xl items-start gap-3 border-blue-200 bg-white p-3 shadow-lg md:p-4">
        <img
          src={logoUrl}
          alt={PWA_APP_NAME}
          className="mt-0.5 size-10 shrink-0 rounded-lg object-contain"
        />

        <div className="min-w-0 flex-1">
          <p className="font-semibold text-slate-900">{PWA_APP_NAME}</p>
          {canInstall ? (
            <p className="mt-1 text-sm text-slate-600">
              ثبّت التطبيق على جهازك للوصول السريع إلى لوحة التحكم.
            </p>
          ) : (
            <p className="mt-1 text-sm text-slate-600">
              للتثبيت على iPhone/iPad: اضغط{' '}
              <Share className="mx-0.5 inline size-3.5 align-text-bottom" aria-hidden />
              ثم «إضافة إلى الشاشة الرئيسية».
            </p>
          )}

          {canInstall && (
            <Button
              type="button"
              className="mt-3 h-9 px-4 py-2 text-sm"
              onClick={() => void promptInstall()}
            >
              <Download className="ml-1.5 size-4" aria-hidden />
              تثبيت التطبيق
            </Button>
          )}

          {isIos && !canInstall && (
            <p className="mt-2 text-xs text-slate-500">
              Safari → مشاركة → إضافة إلى الشاشة الرئيسية
            </p>
          )}
        </div>

        <button
          type="button"
          onClick={dismiss}
          className="shrink-0 rounded-md p-1 text-slate-500 hover:bg-slate-100 hover:text-slate-700"
          aria-label="إغلاق"
        >
          <X className="size-4" />
        </button>
      </Alert>
    </div>
  )
}
