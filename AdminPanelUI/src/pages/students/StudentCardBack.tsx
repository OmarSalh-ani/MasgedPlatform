interface StudentCardBackProps {
  masgedName: string
  logoUrl: string
}

export function StudentCardBack({ masgedName, logoUrl }: StudentCardBackProps) {
  return (
    <div className="card card-back" id="back-card">
      <div className="islamic-pattern" />

      <div className="back-header">
        <div className="logo-dual back-logo">
          <img
            src={logoUrl}
            alt="شعار المسجد"
            className="card-logo-img card-logo-img--back"
          />
        </div>
        <div className="back-mosque-header">
          <div className="back-mosque-name">{masgedName}</div>
        </div>
        <div className="header-spacer header-spacer--back" aria-hidden />
      </div>

      <div className="back-content">
        <div className="contact-info">
          <div className="contact-row">
            <span className="contact-label contact-label--found">
              عند العثور على البطاقة الرجاء الاتصال على :
            </span>
            <span className="contact-value">65887310</span>
          </div>
        </div>

        <div className="qr-block">
          <span className="qr-block-title">الموقع الرسمي</span>
          <div className="qr-section">
            <img src="/qrcode.png" alt="QR Code للموقع الإلكتروني والحلقات" className="qr-code-img" />
          </div>
        </div>
      </div>
    </div>
  )
}
