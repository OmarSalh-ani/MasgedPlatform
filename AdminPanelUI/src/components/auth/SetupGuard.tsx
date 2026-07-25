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
  })

  if (query.isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-[var(--color-layout-bg)]">
        <div className="h-10 w-10 animate-spin rounded-full border-4 border-primary/25 border-t-primary" />
      </div>
    )
  }

  const setupCompleted = query.data?.setupCompleted === true

  if (!setupCompleted && !isSetupRoute) {
    return <Navigate to="/setup" replace />
  }

  if (setupCompleted && isSetupRoute) {
    return <Navigate to="/login" replace />
  }

  return <Outlet />
}
