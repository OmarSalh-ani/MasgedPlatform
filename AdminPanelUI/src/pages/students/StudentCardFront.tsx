import { resolveImageUrl } from '@/lib/resolveImageUrl'

interface StudentCardFrontProps {
  studentId: number
  studentName: string
  circleName: string
  fatherMobile: string
  imageUrl: string | null
  masgedName: string
  logoUrl: string
}

export function StudentCardFront({
  studentId,
  studentName,
  circleName,
  fatherMobile,
  imageUrl,
  masgedName,
  logoUrl,
}: StudentCardFrontProps) {
  return (
    <div className="card card-front" id="front-card">
      <div className="front-header">
        <div className="logo-dual mosque-logo">
          <img src={logoUrl} alt="شعار المسجد" className="card-logo-img" />
        </div>
        <div className="mosque-header">
          <div className="mosque-name">بطاقة الطالب</div>
          <div className="card-type">حلقات {masgedName}</div>
        </div>
        <div className="header-spacer" aria-hidden />
      </div>

      <div className="card-content">
        <div className="photo-section">
          {imageUrl ? (
            <img
              src={resolveImageUrl(imageUrl)}
              alt="صورة الطالب"
              className="student-photo"
              crossOrigin="anonymous"
            />
          ) : null}
        </div>

        <div className="info-section">
          <div className="info-row">
            <span className="info-label">رقم الطالب:</span>
            <span className="info-value">{studentId}</span>
          </div>
          <div className="info-row">
            <span className="info-label">اسم الطالب:</span>
            <span className="info-value">{studentName}</span>
          </div>
          <div className="info-row">
            <span className="info-label">اسم الحلقة:</span>
            <span className="info-value" id="circle-name-display">
              {circleName}
            </span>
          </div>
          <div className="info-row">
            <span className="info-label">رقم ولي الأمر:</span>
            <span className="info-value">{fatherMobile}</span>
          </div>
        </div>
      </div>

      <div className="decorative-bottom" />
    </div>
  )
}
