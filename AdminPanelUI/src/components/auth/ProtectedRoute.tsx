import { useEffect, useState } from 'react'
import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { clearAdminAuth, getAdminToken, isAuthenticated, updateAdminSession } from '@/lib/authStorage'
import { getSession } from '@/services/authService'

export function ProtectedRoute() {
  const location = useLocation()
  const [status, setStatus] = useState<'loading' | 'authenticated' | 'unauthenticated'>(() =>
    isAuthenticated() ? 'loading' : 'unauthenticated',
  )

  useEffect(() => {
    if (!isAuthenticated()) {
      setStatus('unauthenticated')
      return
    }

    let cancelled = false

    getSession()
      .then((session) => {
        if (cancelled) return
        const token = getAdminToken()
        if (token) {
          updateAdminSession(session)
        }
        setStatus('authenticated')
      })
      .catch(() => {
        if (cancelled) return
        clearAdminAuth()
        setStatus('unauthenticated')
      })

    return () => {
      cancelled = true
    }
  }, [location.pathname])

  if (status === 'loading') {
    return (
      <div className="flex min-h-screen items-center justify-center bg-[#f8f9f4]">
        <div className="h-10 w-10 animate-spin rounded-full border-4 border-[#7c8738]/25 border-t-[#7c8738]" />
      </div>
    )
  }

  if (status === 'unauthenticated') {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  return <Outlet />
}
