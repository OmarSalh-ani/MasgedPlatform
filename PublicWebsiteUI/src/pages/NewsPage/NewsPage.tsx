import { useOutletContext } from 'react-router-dom'
import { PageHero } from '@/components/PageHero'
import { NewsSection } from '@/pages/IndexPage/ContentSections'
import { SECTION_META } from '@/lib/siteNav'
import type { PublicWebsiteContent } from '@/types/publicIndex'

interface LayoutContext {
  content: PublicWebsiteContent
}

export function NewsPage() {
  const { content } = useOutletContext<LayoutContext>()
  const meta = SECTION_META.news

  return (
    <main>
      <PageHero badge={meta.badge} title={meta.title} subtitle={meta.subtitle} />
      <NewsSection items={content.news} showHeader={false} />
    </main>
  )
}
