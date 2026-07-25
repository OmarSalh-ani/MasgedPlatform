/** Apply branding primary color to CSS variables used by the public site. */
export function applyPrimaryColor(color: string | null | undefined): void {
  const primary = normalizeHex(color) ?? '#2563eb'
  const root = document.documentElement
  root.style.setProperty('--primary', primary)
  root.style.setProperty('--primary-dark', darkenHex(primary, 0.15))
  root.style.setProperty('--primary-light', lightenHex(primary, 0.25))
  root.style.setProperty('--secondary', lightenHex(primary, 0.25))
  root.style.setProperty('--secondary-light', lightenHex(primary, 0.45))
  root.style.setProperty('--bg-alt', mixWithWhite(primary, 0.85))
  root.style.setProperty('--border', mixWithWhite(primary, 0.7))
  root.style.setProperty('--border-strong', mixWithWhite(primary, 0.55))
}

function normalizeHex(value: string | null | undefined): string | null {
  if (!value) return null
  const trimmed = value.trim()
  return /^#[0-9A-Fa-f]{6}$/.test(trimmed) ? trimmed.toLowerCase() : null
}

function hexToRgb(hex: string): [number, number, number] {
  const n = parseInt(hex.slice(1), 16)
  return [(n >> 16) & 255, (n >> 8) & 255, n & 255]
}

function rgbToHex(r: number, g: number, b: number): string {
  return `#${[r, g, b].map((c) => Math.round(c).toString(16).padStart(2, '0')).join('')}`
}

function darkenHex(hex: string, amount: number): string {
  const [r, g, b] = hexToRgb(hex)
  return rgbToHex(r * (1 - amount), g * (1 - amount), b * (1 - amount))
}

function lightenHex(hex: string, amount: number): string {
  const [r, g, b] = hexToRgb(hex)
  return rgbToHex(
    r + (255 - r) * amount,
    g + (255 - g) * amount,
    b + (255 - b) * amount,
  )
}

function mixWithWhite(hex: string, whiteAmount: number): string {
  return lightenHex(hex, whiteAmount)
}
