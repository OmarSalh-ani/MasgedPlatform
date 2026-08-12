import { useOutletContext } from 'react-router-dom'
import { PageHero } from '@/components/PageHero'
import { TipsSection } from '@/pages/IndexPage/ContentSections'
import { SECTION_META } from '@/lib/siteNav'
import type { PublicWebsiteContent } from '@/types/publicIndex'

interface LayoutContext {
  content: PublicWebsiteContent
}

export function TipsPage() {
  const { content } = useOutletContext<LayoutContext>()
  const meta = SECTION_META.tips

  return (
    <main>
      <PageHero badge={meta.badge} title={meta.title} subtitle={meta.subtitle} />
      <TipsSection items={content.tips} showHeader={false} />
    </main>
  )
}
