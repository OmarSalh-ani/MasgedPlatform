import type { TeacherCardPrint } from '@/types/teacherCardPrint'
import { resolveImageUrl } from '@/lib/resolveImageUrl'

interface TeacherCardFrontProps {
  teacher: TeacherCardPrint
  masgedName: string
  logoUrl: string
}

export function TeacherCardFront({ teacher, masgedName, logoUrl }: TeacherCardFrontProps) {
  return (
    <div className="card card-front" id="front-card">
      <div className="supervisor-badge">مشرف</div>

      <div className="front-header">
        <div className="logo-dual mosque-logo">
          <img src={logoUrl} alt="شعار المسجد" className="card-logo-img" />
        </div>
        <div className="mosque-header">
          <div className="mosque-name">بطاقة المشرف</div>
          <div className="card-type">حلقات {masgedName}</div>
        </div>
        <div className="header-spacer" aria-hidden />
      </div>

      <div className="card-content">
        <div className="photo-section">
          {teacher.imageUrl ? (
            <img
              src={resolveImageUrl(teacher.imageUrl)}
              alt="صورة المشرف"
              className="teacher-photo"
              crossOrigin="anonymous"
            />
          ) : null}
        </div>

        <div className="info-section">
          <div className="info-row">
            <span className="info-label">رقم المشرف:</span>
            <span className="info-value">{teacher.id}</span>
          </div>
          <div className="info-row">
            <span className="info-label">اسم المشرف:</span>
            <span className="info-value">{teacher.name}</span>
          </div>
          <div className="info-row">
            <span className="info-label">المنصب:</span>
            <span className="info-value">مشرف عام</span>
          </div>
        </div>
      </div>

      <div className="decorative-bottom" />
    </div>
  )
}
