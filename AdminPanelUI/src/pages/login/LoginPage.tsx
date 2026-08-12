import { Navigate } from 'react-router-dom'
import { useMasgedBranding } from '@/contexts/MasgedBrandingContext'
import { isAuthenticated, isSupervisorOnly } from '@/lib/authStorage'
import { LoginForm } from '@/pages/login/LoginForm'
import '@/pages/login/login.css'

export function LoginPage() {
  const { masgedName, logoUrl } = useMasgedBranding()
  if (isAuthenticated()) {
    return <Navigate to={isSupervisorOnly() ? '/circle-ratings' : '/'} replace />
  }

  return (
    <div className="login-page grid min-h-screen lg:grid-cols-2">
      <aside
        className="login-brand relative hidden flex-col items-center justify-center overflow-hidden px-10 py-16 lg:flex"
        aria-hidden={false}
      >
        <div className="login-brand-glow login-brand-glow--green" />
        <div className="login-brand-glow login-brand-glow--gold" />

        <div className="relative z-10 flex max-w-sm flex-col items-center text-center">
          <img
            src={logoUrl}
            alt={masgedName}
            className="mb-6 w-full max-w-[280px] drop-shadow-2xl"
          />
          <h2 className="text-xl font-bold leading-relaxed text-white">{masgedName}</h2>
          <p className="mt-4 text-lg font-medium leading-relaxed text-white/90">
            لتعليم القرآن الكريم والعلوم الشرعية
          </p>
          <div className="mt-10 h-px w-16 bg-gradient-to-l from-transparent via-[#c9a227] to-transparent" />
          <p className="mt-6 text-sm tracking-wide text-white/60">
            نظام إدارة المسجد
          </p>
        </div>
      </aside>

      <main className="flex flex-col items-center justify-center bg-[#f8f9f4] px-5 py-10 sm:px-8">
        <div className="login-form-card w-full max-w-[420px] rounded-2xl border border-slate-100 bg-white p-8 sm:p-10">
          <LoginForm />
        </div>
      </main>
    </div>
  )
}
