import { useCallback, useEffect, useRef, useState } from 'react'
import type { PublicMosqueItem } from '@/types/publicIndex'
import { resolveImageUrl } from '@/lib/resolveImageUrl'

export interface MosqueCarouselCard extends PublicMosqueItem {
  index: number
  imageSrc: string
  isPlaceholder: boolean
}

function scrollCarouselToCard(
  viewport: HTMLElement,
  card: HTMLElement,
  behavior: ScrollBehavior = 'smooth',
) {
  const cardRect = card.getBoundingClientRect()
  const viewportRect = viewport.getBoundingClientRect()
  const delta = cardRect.left - viewportRect.left

  viewport.scrollBy({ left: delta, behavior })
}

export function useMosqueCarousel(items: PublicMosqueItem[], fallbackLogoUrl: string) {
  const viewportRef = useRef<HTMLDivElement>(null)
  const [activeIndex, setActiveIndex] = useState(0)
  const pausedRef = useRef(false)

  const count = items.length

  const scrollToIndex = useCallback(
    (index: number, behavior: ScrollBehavior = 'smooth') => {
      const viewport = viewportRef.current
      if (!viewport || count === 0) return

      const normalized = ((index % count) + count) % count
      const card = viewport.querySelector<HTMLElement>(`[data-carousel-index="${normalized}"]`)
      if (card) scrollCarouselToCard(viewport, card, behavior)
      setActiveIndex(normalized)
    },
    [count],
  )

  const goNext = useCallback(() => scrollToIndex(activeIndex + 1), [activeIndex, scrollToIndex])
  const goPrev = useCallback(() => scrollToIndex(activeIndex - 1), [activeIndex, scrollToIndex])
  const goTo = useCallback((index: number) => scrollToIndex(index), [scrollToIndex])

  const pause = useCallback(() => {
    pausedRef.current = true
  }, [])

  const resume = useCallback(() => {
    pausedRef.current = false
  }, [])

  useEffect(() => {
    setActiveIndex(0)
    const viewport = viewportRef.current
    if (viewport) viewport.scrollTo({ left: 0, behavior: 'auto' })
  }, [items.length])

  useEffect(() => {
    if (count <= 1) return

    const timer = window.setInterval(() => {
      if (pausedRef.current) return
      setActiveIndex((current) => {
        const next = (current + 1) % count
        const viewport = viewportRef.current
        const card = viewport?.querySelector<HTMLElement>(`[data-carousel-index="${next}"]`)
        if (viewport && card) scrollCarouselToCard(viewport, card, 'smooth')
        return next
      })
    }, 5000)

    return () => window.clearInterval(timer)
  }, [count])

  useEffect(() => {
    const viewport = viewportRef.current
    if (!viewport || count === 0) return

    const syncActiveIndex = () => {
      const cards = viewport.querySelectorAll<HTMLElement>('[data-carousel-index]')
      const viewportRect = viewport.getBoundingClientRect()
      const viewportCenter = viewportRect.left + viewportRect.width / 2

      let closestIndex = 0
      let closestDistance = Infinity

      cards.forEach((card) => {
        const cardRect = card.getBoundingClientRect()
        const cardCenter = cardRect.left + cardRect.width / 2
        const distance = Math.abs(cardCenter - viewportCenter)
        const index = Number(card.dataset.carouselIndex)

        if (distance < closestDistance) {
          closestDistance = distance
          closestIndex = index
        }
      })

      setActiveIndex(closestIndex)
    }

    viewport.addEventListener('scroll', syncActiveIndex, { passive: true })
    return () => viewport.removeEventListener('scroll', syncActiveIndex)
  }, [count])

  const cards: MosqueCarouselCard[] = items.map((item, index) => {
    const resolved = resolveImageUrl(item.imageUrl)
    return {
      ...item,
      index,
      imageSrc: resolved || fallbackLogoUrl,
      isPlaceholder: !resolved,
    }
  })

  return {
    viewportRef,
    cards,
    activeIndex,
    goTo,
    goNext,
    goPrev,
    count,
    pause,
    resume,
  }
}
