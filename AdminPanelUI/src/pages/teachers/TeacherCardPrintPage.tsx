import { useEffect, useState } from 'react'
import { Navigate, useParams } from 'react-router-dom'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { useTeacherCardPrint } from '@/hooks/useTeacherCardPrint'
import { isAdmin } from '@/lib/authStorage'
import { TeacherCardBack } from '@/pages/teachers/TeacherCardBack'
import { TeacherCardFront } from '@/pages/teachers/TeacherCardFront'
import { TeacherCardPrintControls } from '@/pages/teachers/TeacherCardPrintControls'
import '@/pages/teachers/teacherCardPrint.css'

export function TeacherCardPrintPage() {
  const [showRuler, setShowRuler] = useState(false)
  const { id: idParam } = useParams()
  const teacherId = idParam ? Number(idParam) : NaN
  const { masgedName, logoUrl } = useMasgedBranding()
  const { data: teacher, isLoading, isError } = useTeacherCardPrint(
    Number.isFinite(teacherId) ? teacherId : undefined,
  )

  useEffect(() => {
    document.title =
      'بطاقة المشرفين في الشيخ مبارك عبدالله مبارك الصباح'
    const onBeforePrint = () => {
      document.body.style.setProperty('-webkit-print-color-adjust', 'exact')
      document.body.style.printColorAdjust = 'exact'
    }
    window.addEventListener('beforeprint', onBeforePrint)
    return () => window.removeEventListener('beforeprint', onBeforePrint)
  }, [])

  if (!isAdmin()) {
    return <Navigate to="/login" replace />
  }

  if (!idParam || !Number.isFinite(teacherId) || teacherId <= 0) {
    return <Navigate to="/" replace />
  }

  if (isLoading) {
    return (
      <div className="teacher-card-print-page p-5">
        <Skeleton className="mx-auto h-40 max-w-md" />
      </div>
    )
  }

  if (isError || !teacher) {
    return (
      <div className="teacher-card-print-page p-5">
        <Alert variant="destructive" className="mx-auto max-w-md">
          تعذر تحميل بيانات البطاقة
        </Alert>
      </div>
    )
  }

  return (
    <div className="teacher-card-print-page" dir="rtl">
      <TeacherCardPrintControls onToggleRuler={() => setShowRuler((v) => !v)} />

      <div id="card-ruler" className={`ruler${showRuler ? '' : ' hidden'}`}>
        <div className="ruler-text">Card Size: 85.5mm × 54mm</div>
        <div className="ruler-line" />
      </div>

      <div className="card-container">
        <TeacherCardFront teacher={teacher} masgedName={masgedName} logoUrl={logoUrl} />
        <TeacherCardBack masgedName={masgedName} logoUrl={logoUrl} />
      </div>
    </div>
  )
}
