import { useSearchParams } from 'react-router-dom'
import { PageHero } from '@/components/PageHero'
import { PageError, PageLoading } from '@/components/ContentPageStates'
import { useRegistrationConfig } from '@/hooks/usePublicIndex'
import { SECTION_META } from '@/lib/siteNav'
import { parseRegistrationMode } from '@/types/publicIndex'
import { RegistrationClosed } from '@/pages/RegistrationPage/RegistrationClosed'
import { RegistrationForm } from '@/pages/RegistrationPage/RegistrationForm'
import '@/pages/RegistrationPage/registration-page.css'

const REGISTRATION_STEPS = [
  'أدخل البيانات الشخصية للطالب',
  'أضف رقم جوال ولي الأمر مع اختيار الدولة',
  'اختر نوع النشاط المناسب وأرسل الطلب',
]

export function RegistrationPage() {
  const [searchParams] = useSearchParams()
  const mode = parseRegistrationMode(searchParams.get('q'))
  const registrationQuery = useRegistrationConfig(mode)
  const meta = SECTION_META.registration

  if (registrationQuery.isLoading) {
    return <PageLoading />
  }

  if (registrationQuery.isError || !registrationQuery.data) {
    return <PageError />
  }

  const config = registrationQuery.data

  if (!config.registrationEnabled) {
    return (
      <main>
        <PageHero badge={meta.badge} title={meta.title} subtitle={meta.subtitle} />
        <RegistrationClosed />
      </main>
    )
  }

  return (
    <main>
      <PageHero badge={meta.badge} title={meta.title} subtitle={meta.subtitle} />

      <section className="reg-page" id="registration">
        <div className="container reg-page__layout">
          <aside className="reg-sidebar" aria-label="إرشادات التسجيل">
            <div className="reg-sidebar__card">
              <span className="reg-sidebar__badge">
                <i className="fas fa-clipboard-list" aria-hidden="true" />
                خطوات التسجيل
              </span>
              <h2 className="reg-sidebar__title">ابدأ رحلة التسجيل</h2>
              <p className="reg-sidebar__text">
                املأ النموذج بدقة. جميع الحقول المميزة بـ (*) مطلوبة لإتمام طلب التسجيل.
              </p>
              <ol className="reg-sidebar__steps">
                {REGISTRATION_STEPS.map((step, index) => (
                  <li key={step} className="reg-sidebar__step">
                    <span className="reg-sidebar__step-num">{index + 1}</span>
                    <span>{step}</span>
                  </li>
                ))}
              </ol>
              <p className="reg-sidebar__note">
                <i className="fas fa-shield-alt" aria-hidden="true" />
                <span>بياناتك محفوظة بسرية تامة وتُستخدم فقط لأغراض التسجيل في الأنشطة.</span>
              </p>
            </div>
          </aside>

          <RegistrationForm mode={mode} config={config} />
        </div>
      </section>
    </main>
  )
}
