import { useOutletContext } from 'react-router-dom'
import { PageHero } from '@/components/PageHero'
import { MosquesSection } from '@/pages/IndexPage/MosquesSection'
import { SECTION_META } from '@/lib/siteNav'
import type { PublicWebsiteContent } from '@/types/publicIndex'

interface LayoutContext {
  content: PublicWebsiteContent
}

export function MosquesPage() {
  const { content } = useOutletContext<LayoutContext>()
  const meta = SECTION_META.mosques

  return (
    <main>
      <PageHero badge={meta.badge} title={meta.title} subtitle={meta.subtitle} />
      <MosquesSection items={content.mosques} showHeader={false} />
    </main>
  )
}
