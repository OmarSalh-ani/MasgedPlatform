import { Outlet, useNavigate } from 'react-router-dom'
import { AdminSidebar } from '@/components/layout/AdminSidebar'
import { AdminTopBar } from '@/components/layout/AdminTopBar'
import { useAdminSidebar } from '@/hooks/useAdminSidebar'
import { clearAdminAuth } from '@/lib/authStorage'
import { cn } from '@/lib/utils'

export function AdminLayout() {
  const navigate = useNavigate()
  const { sidebarOpen, toggleSidebar, closeSidebarOnMobile } = useAdminSidebar()

  const handleLogout = () => {
    clearAdminAuth()
    navigate('/login', { replace: true })
  }

  return (
    <div className="admin-shell-bg min-h-screen">
      <AdminSidebar
        open={sidebarOpen}
        onClose={closeSidebarOnMobile}
      />

      <div
        className={cn(
          'relative flex min-h-screen flex-col transition-[margin] duration-300',
          sidebarOpen && 'md:mr-[var(--sidebar-width)]',
        )}
      >
        <AdminTopBar
          sidebarOpen={sidebarOpen}
          onMenuToggle={toggleSidebar}
          onLogout={handleLogout}
        />

        <div className="flex-1 px-3 pb-6 pt-4 md:px-6 md:pb-8">
          <main className="admin-main-panel min-h-[calc(100vh-7.5rem)] rounded-2xl p-4 md:p-6">
            <Outlet />
          </main>
        </div>
      </div>
    </div>
  )
}
