export const PHOTO_ASPECT_WIDTH = 4
export const PHOTO_ASPECT_HEIGHT = 6
export const PHOTO_ASPECT_RATIO = PHOTO_ASPECT_WIDTH / PHOTO_ASPECT_HEIGHT
export const PHOTO_ASPECT_TOLERANCE = 0.05

export const PHOTO_ASPECT_ERROR_MESSAGE =
  'يجب أن تكون الصورة بمقاس 4×6 (العرض × الارتفاع)'

export async function getImageDimensions(file: File): Promise<{ width: number; height: number }> {
  if (typeof createImageBitmap === 'function') {
    const bitmap = await createImageBitmap(file, { imageOrientation: 'from-image' })
    const dimensions = { width: bitmap.width, height: bitmap.height }
    bitmap.close()
    return dimensions
  }

  return new Promise((resolve, reject) => {
    const url = URL.createObjectURL(file)
    const img = new Image()
    img.onload = () => {
      URL.revokeObjectURL(url)
      resolve({ width: img.naturalWidth, height: img.naturalHeight })
    }
    img.onerror = () => {
      URL.revokeObjectURL(url)
      reject(new Error('failed to load image'))
    }
    img.src = url
  })
}

export function isPhotoAspectRatioValid(width: number, height: number): boolean {
  if (width <= 0 || height <= 0) return false
  const ratio = width / height
  return Math.abs(ratio - PHOTO_ASPECT_RATIO) / PHOTO_ASPECT_RATIO <= PHOTO_ASPECT_TOLERANCE
}

export async function validatePhotoAspectRatio(file: File): Promise<boolean> {
  try {
    const { width, height } = await getImageDimensions(file)
    return isPhotoAspectRatioValid(width, height)
  } catch {
    return false
  }
}
