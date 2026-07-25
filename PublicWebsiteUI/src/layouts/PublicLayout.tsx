import { Outlet } from 'react-router-dom'
import { PageError, PageLoading } from '@/components/ContentPageStates'
import { ImageModal } from '@/components/ImageModal'
import { MobileAppBanner } from '@/components/MobileAppBanner'
import { useRegistrationConfig, useWebsiteContent } from '@/hooks/usePublicIndex'
import { SiteFooter } from '@/pages/IndexPage/SiteFooter'
import { SiteHeader } from '@/pages/IndexPage/SiteHeader'

export function PublicLayout() {
  const contentQuery = useWebsiteContent()
  const registrationQuery = useRegistrationConfig('default')

  if (contentQuery.isLoading || registrationQuery.isLoading) {
    return <PageLoading />
  }

  if (contentQuery.isError || registrationQuery.isError || !contentQuery.data) {
    return <PageError />
  }

  const showActivitiesNav = registrationQuery.data?.labels.showActivitiesNav ?? false

  return (
    <>
      <SiteHeader showActivitiesNav={showActivitiesNav} />
      <Outlet context={{ content: contentQuery.data, registration: registrationQuery.data }} />
      <MobileAppBanner />
      <SiteFooter about={contentQuery.data.about} socialLinks={contentQuery.data.socialLinks} />
      <ImageModal />
    </>
  )
}
