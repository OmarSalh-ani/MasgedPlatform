import { useEffect, useState } from 'react'
import { Navigate, useParams } from 'react-router-dom'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { useStudentCardPrint } from '@/hooks/useStudentCardPrint'
import { isAdmin } from '@/lib/authStorage'
import { StudentCardBack } from '@/pages/students/StudentCardBack'
import { StudentCardFront } from '@/pages/students/StudentCardFront'
import { StudentCardPrintControls } from '@/pages/students/StudentCardPrintControls'
import '@/pages/students/studentCardPrint.css'

export function StudentCardPrintPage() {
  const [showRuler, setShowRuler] = useState(false)
  const [displayCircleName, setDisplayCircleName] = useState('')
  const { id: idParam } = useParams()
  const studentId = idParam ? Number(idParam) : NaN
  const { masgedName, logoUrl } = useMasgedBranding()
  const { data: student, isLoading, isError } = useStudentCardPrint(
    Number.isFinite(studentId) ? studentId : undefined,
  )

  useEffect(() => {
    if (student) {
      setDisplayCircleName(student.circleName)
    }
  }, [student])

  useEffect(() => {
    document.title = `بطاقة ${masgedName}`
    const onBeforePrint = () => {
      document.body.style.setProperty('-webkit-print-color-adjust', 'exact')
      document.body.style.printColorAdjust = 'exact'
    }
    window.addEventListener('beforeprint', onBeforePrint)
    return () => window.removeEventListener('beforeprint', onBeforePrint)
  }, [masgedName])

  if (!isAdmin()) {
    return <Navigate to="/login" replace />
  }

  if (!idParam || !Number.isFinite(studentId) || studentId <= 0) {
    return <Navigate to="/" replace />
  }

  if (isLoading) {
    return (
      <div className="student-card-print-page p-5">
        <Skeleton className="mx-auto h-40 max-w-md" />
      </div>
    )
  }

  if (isError || !student) {
    return (
      <div className="student-card-print-page p-5">
        <Alert variant="destructive" className="mx-auto max-w-md">
          تعذر تحميل بيانات البطاقة
        </Alert>
      </div>
    )
  }

  return (
    <div className="student-card-print-page" dir="rtl">
      <StudentCardPrintControls
        circleOptions={student.circleOptions}
        selectedCircle={displayCircleName}
        onCircleChange={setDisplayCircleName}
        onToggleRuler={() => setShowRuler((v) => !v)}
      />

      <div id="card-ruler" className={`ruler${showRuler ? '' : ' hidden'}`}>
        <div className="ruler-text">Card Size: 85.5mm × 54mm</div>
        <div className="ruler-line" />
      </div>

      <div className="card-container">
        <StudentCardFront
          studentId={student.id}
          studentName={student.studentName}
          circleName={displayCircleName}
          fatherMobile={student.fatherMobile}
          imageUrl={student.imageUrl}
          masgedName={masgedName}
          logoUrl={logoUrl}
        />
        <StudentCardBack masgedName={masgedName} logoUrl={logoUrl} />
      </div>
    </div>
  )
}
