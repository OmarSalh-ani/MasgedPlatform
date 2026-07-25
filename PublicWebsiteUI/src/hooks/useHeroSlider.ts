import { useEffect, useState } from 'react'
import type { PublicHeroSlideItem } from '@/types/publicIndex'
import { resolveImageUrl } from '@/lib/resolveImageUrl'

export function useHeroSlider(slides: PublicHeroSlideItem[]) {
  const [activeIndex, setActiveIndex] = useState(0)

  const goTo = (index: number) => setActiveIndex(index % Math.max(slides.length, 1))

  useEffect(() => {
    if (slides.length <= 1) return
    const timer = window.setInterval(() => {
      setActiveIndex((current) => (current + 1) % slides.length)
    }, 5000)
    return () => window.clearInterval(timer)
  }, [slides.length])

  const mappedSlides = slides.map((slide, index) => ({
    ...slide,
    imageSrc: resolveImageUrl(slide.imageUrl),
    isActive: index === activeIndex,
    goTo,
  }))

  return mappedSlides
}
