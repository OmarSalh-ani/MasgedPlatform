import { useEffect, useMemo } from 'react'
import { Navigate, useSearchParams } from 'react-router-dom'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { useTestCertificate } from '@/hooks/useTestCertificate'
import { isAdmin } from '@/lib/authStorage'
import {
  calculateTotalAndGrade,
  formatScore,
} from '@/pages/test-certificate/testCertificateUtils'
import { TestCertificateNotifySection } from '@/pages/test-certificate/TestCertificateNotifySection'
import '@/pages/test-certificate/testCertificate.css'

export function TestCertificatePage() {
  const [searchParams] = useSearchParams()
  const testIdParam = searchParams.get('TestId')
  const testId = testIdParam ? Number(testIdParam) : NaN
  const { data: certificate, isLoading, isError } = useTestCertificate(
    Number.isFinite(testId) ? testId : undefined,
  )

  const scores = useMemo(() => {
    if (!certificate) return null
    return calculateTotalAndGrade(
      certificate.memorizationScore,
      certificate.tajweedScore,
      certificate.revisionScore,
    )
  }, [certificate])

  useEffect(() => {
    document.title = 'شهادة الاختبار'
  }, [])

  if (!isAdmin()) {
    return <Navigate to="/login" replace />
  }

  if (!testIdParam || !Number.isFinite(testId) || testId <= 0) {
    return (
      <div className="test-certificate-page p-5" dir="rtl">
        <Alert variant="destructive" className="mx-auto max-w-md">
          معرف الاختبار غير صحيح
        </Alert>
      </div>
    )
  }

  if (isLoading) {
    return (
      <div className="test-certificate-page p-5" dir="rtl">
        <Skeleton className="mx-auto h-96 max-w-3xl" />
      </div>
    )
  }

  if (isError || !certificate || !scores) {
    return (
      <div className="test-certificate-page p-5" dir="rtl">
        <Alert variant="destructive" className="mx-auto max-w-md">
          {isError ? 'الاختبار غير موجود' : 'تعذر تحميل شهادة الاختبار'}
        </Alert>
      </div>
    )
  }

  return (
    <div className="test-certificate-page" dir="rtl">
      <TestCertificateNotifySection
        testId={certificate.testId}
        studentName={certificate.studentName}
        grade={scores.grade}
      />

      <div className="no-print mb-4 flex justify-center">
        <Button type="button" onClick={() => window.print()}>
          طباعة الشهادة
        </Button>
      </div>

      <div className="certificate-container">
        <div className="certificate-header">
          <h1 className="certificate-title">شهادة الاختبار</h1>
        </div>

        <div className="student-info">
          <InfoRow label="اسم الطالب:" value={certificate.studentName} />
          <InfoRow label="اسم الحلقة:" value={certificate.circleName} />
          <InfoRow label="اسم المعلم:" value={certificate.teacherName} />
          <InfoRow label="تاريخ الاختبار:" value={certificate.testDate} />
          <InfoRow label="من:" value={certificate.testFrom} />
          <InfoRow label="إلى:" value={certificate.testTo} />
        </div>

        <table className="scores-table">
          <thead>
            <tr>
              <th>الحفظ</th>
              <th>التجويد</th>
              <th>المراجعة</th>
              <th>المجموع</th>
              <th>التقدير</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>{formatScore(certificate.memorizationScore)}</td>
              <td>{formatScore(certificate.tajweedScore)}</td>
              <td>{formatScore(certificate.revisionScore)}</td>
              <td className="total-row">{scores.total}</td>
              <td className="grade-row">{scores.grade}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  )
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="info-row">
      <span className="info-label">{label}</span>
      <span className="info-value">{value}</span>
    </div>
  )
}
