import html2canvas from 'html2canvas'

export function printFront(): void {
  document.body.classList.add('print-front-only')
  document.body.classList.remove('print-back-only')
  window.print()
  document.body.classList.remove('print-front-only')
}

export function printBack(): void {
  document.body.classList.add('print-back-only')
  document.body.classList.remove('print-front-only')
  window.print()
  document.body.classList.remove('print-back-only')
}

async function saveCardAsPng(elementId: string, fileName: string): Promise<void> {
  const card = document.getElementById(elementId)
  if (!card) return

  const canvas = await html2canvas(card, {
    scale: 3,
    width: card.offsetWidth,
    height: card.offsetHeight,
    useCORS: true,
  })

  const link = document.createElement('a')
  link.download = fileName
  link.href = canvas.toDataURL('image/png')
  link.click()
}

export function saveFrontAsPng(): Promise<void> {
  return saveCardAsPng('front-card', 'front-card.png')
}

export function saveBackAsPng(): Promise<void> {
  return saveCardAsPng('back-card', 'back-card.png')
}
