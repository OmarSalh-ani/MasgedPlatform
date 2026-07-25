import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Skeleton } from '@/components/ui/skeleton'
import { canModify } from '@/lib/authStorage'
import { useWhatsappQr } from '@/hooks/useWhatsappQr'
import type { WhatsappQrStatus } from '@/types/whatsappQr'

export function WhatsappQrPage() {
  const userCanModify = canModify()
  const { query, refreshMutation, healthMutation, createMutation, disconnectMutation, reconnectMutation } =
    useWhatsappQr()
  const [phoneNumber, setPhoneNumber] = useState('')
  const [status, setStatus] = useState<WhatsappQrStatus | null>(null)

  useEffect(() => {
    if (query.data) setStatus(query.data)
  }, [query.data])

  const applyStatus = (next: WhatsappQrStatus) => setStatus(next)

  useEffect(() => {
    const timer = window.setInterval(() => {
      if (!userCanModify) return
      refreshMutation.mutate(undefined, { onSuccess: applyStatus })
    }, 30000)
    return () => window.clearInterval(timer)
  }, [userCanModify, refreshMutation])

  if (!userCanModify) {
    return <Alert variant="destructive">ليس لديك صلاحية للوصول إلى صفحة ربط الواتساب</Alert>
  }

  if (query.isLoading && !status) {
    return <Skeleton className="h-96 w-full max-w-xl mx-auto" />
  }

  const pending =
    refreshMutation.isPending ||
    healthMutation.isPending ||
    createMutation.isPending ||
    disconnectMutation.isPending ||
    reconnectMutation.isPending

  return (
    <div className="mx-auto max-w-xl space-y-6 p-4">
      <PageHeader
        title="WhatsApp QR Code Scanner"
        description="Scan the QR code below to link your WhatsApp account for messaging"
        className="mb-0"
        actions={
          <Link
            to="/home"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            ← Back to Dashboard
          </Link>
        }
      />

      <section className="rounded-xl border bg-white p-6 text-center shadow-sm">
        <p className="rounded-md border border-yellow-200 bg-yellow-50 px-3 py-2 text-sm font-semibold text-yellow-900">
          {status?.statusText ?? '⏳ Loading QR code...'}
        </p>

        <div className="mt-4 rounded-lg bg-sky-50 p-4 text-right text-sm">
          <strong>Instructions:</strong>
          <ol className="mt-2 list-decimal pr-5">
            <li>Open WhatsApp on your mobile phone</li>
            <li>Go to Settings → Linked Devices</li>
            <li>Tap &quot;Link a Device&quot;</li>
            <li>Point your phone camera at the QR code below</li>
            <li>Wait for authentication to complete</li>
          </ol>
        </div>

        <div className="my-6 rounded-lg border border-dashed border-[#25D366] bg-slate-50 p-4">
          {status?.qrImageDataUrl ? (
            <img src={status.qrImageDataUrl} alt="WhatsApp QR Code" className="mx-auto max-w-[300px]" />
          ) : status?.bodyHtml ? (
            <div dangerouslySetInnerHTML={{ __html: status.bodyHtml }} />
          ) : null}
        </div>

        {status?.showCreateSession ? (
          <div className="mb-4 flex flex-wrap items-center justify-center gap-2">
            <Label htmlFor="whatsappPhone">Phone Number (Kuwait +965):</Label>
            <Input
              id="whatsappPhone"
              className="max-w-[200px]"
              placeholder="e.g. 51234567"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
            />
            <Button
              type="button"
              disabled={pending}
              onClick={() => createMutation.mutate(phoneNumber, { onSuccess: applyStatus })}
            >
              ➕ Create Session
            </Button>
          </div>
        ) : null}

        <div className="flex flex-wrap justify-center gap-2">
          <Button type="button" variant="outline" disabled={pending} onClick={() => refreshMutation.mutate(undefined, { onSuccess: applyStatus })}>
            🔄 Refresh QR Code
          </Button>
          <Button type="button" variant="outline" disabled={pending} onClick={() => healthMutation.mutate(undefined, { onSuccess: applyStatus })}>
            📊 Check Connection Status
          </Button>
          {status?.showDisconnect ? (
            <Button type="button" className="bg-red-600 text-white hover:opacity-90" disabled={pending} onClick={() => disconnectMutation.mutate(undefined, { onSuccess: applyStatus })}>
              🔌 Disconnect WhatsApp
            </Button>
          ) : null}
          {status?.showReconnect ? (
            <Button type="button" disabled={pending} onClick={() => reconnectMutation.mutate(undefined, { onSuccess: applyStatus })}>
              🔗 Reconnect WhatsApp
            </Button>
          ) : null}
        </div>

        <p className="mt-4 text-sm text-slate-500">💡 Click &quot;Refresh QR Code&quot; to get the latest QR code</p>
      </section>

      {query.isError ? <Alert variant="destructive">تعذر تحميل حالة الواتساب</Alert> : null}
    </div>
  )
}
