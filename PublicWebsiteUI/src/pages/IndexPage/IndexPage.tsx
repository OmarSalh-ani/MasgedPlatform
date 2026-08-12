import { useOutletContext } from 'react-router-dom'
import { RegisterCtaBand } from '@/components/RegisterCtaBand'
import { HOME_PREVIEW_LIMIT } from '@/lib/constants'
import { SITE_ROUTES } from '@/lib/siteNav'
import { useHeroSlider } from '@/hooks/useHeroSlider'
import {
  ActivitiesSection,
  TipsSection,
  MosquesSection,
  NewsSection,
  PortalSection,
} from '@/pages/IndexPage/ContentSections'
import { HeroSection } from '@/pages/IndexPage/HeroSection'
import type { PublicRegistrationConfig, PublicWebsiteContent } from '@/types/publicIndex'

interface LayoutContext {
  content: PublicWebsiteContent
  registration: PublicRegistrationConfig | undefined
}

export function IndexPage() {
  const { content, registration } = useOutletContext<LayoutContext>()
  const heroSlides = useHeroSlider(content.heroSlides)
  const labels = registration?.labels
  const showActivities = labels?.showActivitiesSection ?? false

  return (
    <main>
      <HeroSection
        slides={heroSlides}
        showRegisterButton={registration?.registrationEnabled ?? false}
        showActivitiesButton={showActivities}
      />
      <TipsSection
        items={content.tips}
        limit={HOME_PREVIEW_LIMIT.tips}
        viewAllHref={SITE_ROUTES.tips}
        showCta
      />
      <MosquesSection
        items={content.mosques}
        limit={HOME_PREVIEW_LIMIT.mosques}
        viewAllHref={SITE_ROUTES.mosques}
        showCta
      />
      <NewsSection
        items={content.news}
        limit={HOME_PREVIEW_LIMIT.news}
        viewAllHref={SITE_ROUTES.news}
        showCta
      />
      {showActivities && (
        <ActivitiesSection
          items={content.activities}
          limit={HOME_PREVIEW_LIMIT.activities}
          viewAllHref={SITE_ROUTES.activities}
          showCta
        />
      )}
      <PortalSection />
      <RegisterCtaBand enabled={registration?.registrationEnabled ?? false} />
    </main>
  )
}
