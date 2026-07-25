import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useParentsFollowup } from '@/hooks/useParentsFollowup'
import { ParentsFollowupForm } from '@/pages/parents-followup/ParentsFollowupForm'
import { ParentsFollowupSuccessDialog } from '@/pages/parents-followup/ParentsFollowupSuccessDialog'
import '@/pages/parents-followup/parentsFollowup.css'

export function ParentsFollowupPage() {
  const [searchParams] = useSearchParams()
  const [successOpen, setSuccessOpen] = useState(false)
  const printMode = searchParams.get('Print') === '1' || searchParams.get('print') === '1'
  const idParam = searchParams.get('Id') ?? searchParams.get('id')
  const studentId = idParam ? Number(idParam) : NaN
  const validId = Number.isFinite(studentId) && studentId > 0

  const { query, submitMutation, getSubmitErrorMessage } = useParentsFollowup(
    validId ? studentId : undefined,
  )
  const submitErrorMessage = submitMutation.isError
    ? getSubmitErrorMessage(submitMutation.error)
    : null

  useEffect(() => {
    document.title = 'استمارة تسجيل الطالب'
  }, [])

  const handleDownloadPdf = () => {
    const element = document.getElementById('customForm')
    if (!element) return

    const script = document.createElement('script')
    script.src = 'https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js'
    script.onload = () => {
      const html2pdf = (window as Window & { html2pdf?: () => { set: (o: object) => { from: (el: HTMLElement) => { save: () => void } } } }).html2pdf
      if (!html2pdf) return
      html2pdf()
        .set({
          margin: [0.5, 0.5, 0.5, 0.5],
          filename: 'exported-a4-colored.pdf',
          image: { type: 'jpeg', quality: 1 },
          html2canvas: { scale: 2, useCORS: true, backgroundColor: null },
          jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' },
        })
        .from(element)
        .save()
    }
    document.body.appendChild(script)
  }

  if (!validId) {
    return (
      <div className="parents-followup-page">
        <Alert variant="destructive" className="mx-auto max-w-md">
          رابط غير صالح. يرجى استخدام الرابط المرسل إليكم.
        </Alert>
      </div>
    )
  }

  if (query.isLoading) {
    return (
      <div className="parents-followup-page p-5">
        <Skeleton className="mx-auto h-40 max-w-md" />
      </div>
    )
  }

  if (query.isError) {
    return (
      <div className="parents-followup-page">
        <Alert variant="destructive" className="mx-auto max-w-md">
          تعذر تحميل بيانات الطالب.
        </Alert>
      </div>
    )
  }

  return (
    <div className="parents-followup-page">
      {printMode && (
        <button type="button" className="pdf-btn" onClick={handleDownloadPdf}>
          Download as PDF
        </button>
      )}
      <ParentsFollowupForm
        query={query}
        submitMutation={submitMutation}
        submitErrorMessage={submitErrorMessage}
        onSuccess={() => setSuccessOpen(true)}
      />
      <ParentsFollowupSuccessDialog
        open={successOpen}
        onClose={() => {
          setSuccessOpen(false)
          window.location.reload()
        }}
      />
    </div>
  )
}
