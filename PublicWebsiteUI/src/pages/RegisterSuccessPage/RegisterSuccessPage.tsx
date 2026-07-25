import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { getRegisterSuccess } from '@/services/publicIndexService'
import '@/styles/register-success.css'

export function RegisterSuccessPage() {
  const query = useQuery({
    queryKey: ['register-success'],
    queryFn: getRegisterSuccess,
  })

  if (query.isLoading) {
    return (
      <div className="success-page">
        <div className="success-card">
          <div className="state-spinner" style={{ margin: '0 auto' }} aria-hidden="true" />
          <p className="success-text" style={{ marginTop: '1rem' }}>جاري التحميل...</p>
        </div>
      </div>
    )
  }

  if (query.isError || !query.data) {
    return (
      <div className="success-page">
        <div className="success-card">
          <p className="success-text">تعذر تحميل الصفحة</p>
          <Link to="/" className="btn btn-primary success-back">عودة للرئيسية</Link>
        </div>
      </div>
    )
  }

  const data = query.data

  return (
    <div className="success-page">
      <div className="success-card">
        <div className="success-icon-wrap">
          <i className="fas fa-check" aria-hidden="true" />
        </div>
        <h1>تم التسجيل بنجاح</h1>
        <p className="success-text" id="HeadLbl">{data.headText}</p>
        <p className="success-text" id="TitleLbl">{data.titleText}</p>

        {data.socialLinks.length > 0 && (
          <div className="success-social">
            {data.socialLinks.map((link) => (
              <a
                key={link.id}
                href={link.url || '#'}
                className="success-social-link"
                target="_blank"
                rel="noreferrer"
              >
                <i className={link.resolvedIconClass} aria-hidden="true" />
                {link.platformName}
              </a>
            ))}
          </div>
        )}

        <div className="success-whatsapp">
          <p id="SubscribeLbl">{data.subscribeText}</p>
          <a className="btn-whatsapp" target="_blank" rel="noreferrer" id="WhatsappLink" href={data.whatsappUrl}>
            <i className="fab fa-whatsapp" aria-hidden="true" />
            اشترك الآن
          </a>
        </div>

        <p className="success-footer-note">بحمى الرحمن، لا تنسى ذكر الله</p>
        <Link className="btn btn-primary success-back" to="/">
          <i className="fas fa-arrow-right" aria-hidden="true" />
          عودة لصفحة التسجيل
        </Link>
      </div>
    </div>
  )
}
