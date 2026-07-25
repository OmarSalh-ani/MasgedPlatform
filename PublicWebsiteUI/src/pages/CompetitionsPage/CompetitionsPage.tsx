import { useOutletContext } from 'react-router-dom'
import { PageHero } from '@/components/PageHero'
import { CompetitionsSection } from '@/pages/IndexPage/ContentSections'
import { SECTION_META } from '@/lib/siteNav'
import type { PublicWebsiteContent } from '@/types/publicIndex'

interface LayoutContext {
  content: PublicWebsiteContent
}

export function CompetitionsPage() {
  const { content } = useOutletContext<LayoutContext>()
  const meta = SECTION_META.competitions

  return (
    <main>
      <PageHero badge={meta.badge} title={meta.title} subtitle={meta.subtitle} />
      <CompetitionsSection items={content.competitions} showHeader={false} />
    </main>
  )
}
