import { resolveImageUrl } from '@/lib/resolveImageUrl'

export function resolveLogoUrl(logoUrl: string | null | undefined): string | undefined {
  const resolved = resolveImageUrl(logoUrl ?? '')
  return resolved || undefined
}