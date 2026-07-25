import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RouterProvider } from 'react-router-dom'
import { PwaBrandingSync } from '@/components/shared/PwaBrandingSync'
import { PwaInstallBanner } from '@/components/shared/PwaInstallBanner'
import { MasgedBrandingProvider } from '@/contexts/MasgedBrandingContext'
import { router } from '@/router'

const queryClient = new QueryClient()

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <MasgedBrandingProvider>
        <PwaBrandingSync />
        <PwaInstallBanner />
        <RouterProvider router={router} />
      </MasgedBrandingProvider>
    </QueryClientProvider>
  )
}
