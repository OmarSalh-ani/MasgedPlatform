import { Navigate, useOutletContext } from 'react-router-dom'
import { PageHero } from '@/components/PageHero'
import { ActivitiesSection } from '@/pages/IndexPage/ContentSections'
import { SECTION_META, SITE_ROUTES } from '@/lib/siteNav'
import type { PublicRegistrationConfig, PublicWebsiteContent } from '@/types/publicIndex'

interface LayoutContext {
  content: PublicWebsiteContent
  registration: PublicRegistrationConfig | undefined
}

export function ActivitiesPage() {
  const { content, registration } = useOutletContext<LayoutContext>()
  const meta = SECTION_META.activities

  if (!registration?.labels.showActivitiesSection) {
    return <Navigate to={SITE_ROUTES.home} replace />
  }

  return (
    <main>
      <PageHero badge={meta.badge} title={meta.title} subtitle={meta.subtitle} />
      <ActivitiesSection items={content.activities} showHeader={false} />
    </main>
  )
}
