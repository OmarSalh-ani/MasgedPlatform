import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { getSetupStatus } from '@/services/masgedSettingsService'

export const SETUP_STATUS_QUERY_KEY = ['setupStatus'] as const

export function SetupGuard() {
  const location = useLocation()
  const isSetupRoute = location.pathname === '/setup'

  const query = useQuery({
    queryKey: SETUP_STATUS_QUERY_KEY,
    queryFn: getSetupStatus,
    staleTime: 30_000,
    retry: 1,
  })

  if (query.isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-[var(--color-layout-bg)]">
        <div className="h-10 w-10 animate-spin rounded-full border-4 border-primary/25 border-t-primary" />
      </div>
    )
  }

  // Fail open: a transient API error must never trap an existing install on /setup.
  // Only force the wizard when the API successfully reports setup is incomplete.
  if (query.isError) {
    if (isSetupRoute) {
      return <Navigate to="/login" replace />
    }
    return <Outlet />
  }

  const setupCompleted = query.data?.setupCompleted === true
  const setupIncomplete = query.data?.setupCompleted === false

  if (setupIncomplete && !isSetupRoute) {
    return <Navigate to="/setup" replace />
  }

  if (setupCompleted && isSetupRoute) {
    return <Navigate to="/login" replace />
  }

  return <Outlet />
}
