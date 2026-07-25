import { Plus, X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import type { TeacherMapLocation } from '@/types/teacher'

interface TeacherManualLocationsProps {
  locations: TeacherMapLocation[]
  onChange: (locations: TeacherMapLocation[]) => void
}

function extractCoordsFromUrl(url: string): { lat: string; lng: string } {
  const match = url.match(/@(-?\d+\.\d+),(-?\d+\.\d+)|place\/(-?\d+\.\d+),(-?\d+\.\d+)/)
  if (!match) return { lat: '', lng: '' }
  return { lat: match[1] || match[3] || '', lng: match[2] || match[4] || '' }
}

export function TeacherManualLocations({ locations, onChange }: TeacherManualLocationsProps) {
  const addLocation = () => {
    onChange([...locations, { url: '', lat: '', lng: '' }])
  }

  const updateLocation = (index: number, patch: Partial<TeacherMapLocation>) => {
    const next = locations.map((item, i) => (i === index ? { ...item, ...patch } : item))
    onChange(next)
  }

  const removeLocation = (index: number) => {
    onChange(locations.filter((_, i) => i !== index))
  }

  return (
    <div className="space-y-4">
      {locations.map((location, index) => (
        <div key={index} className="relative rounded-lg border bg-slate-50 p-4">
          <button
            type="button"
            className="absolute left-2 top-2 text-red-600"
            onClick={() => removeLocation(index)}
            aria-label="حذف الموقع"
          >
            <X className="size-4" />
          </button>
          <div className="mb-3 pt-2">
            <label className="mb-1 block text-sm font-semibold">رابط جوجل ماب</label>
            <Input
              value={location.url}
              onChange={(e) => {
                const url = e.target.value
                const coords = extractCoordsFromUrl(url)
                updateLocation(index, { url, lat: coords.lat, lng: coords.lng })
              }}
              placeholder="انسخ رابط الموقع هنا"
            />
          </div>
          <div className="grid gap-3 sm:grid-cols-2">
            <div>
              <label className="mb-1 block text-sm font-semibold">خط العرض</label>
              <Input value={location.lat ?? ''} readOnly placeholder="سيتم استخراجه تلقائياً" />
            </div>
            <div>
              <label className="mb-1 block text-sm font-semibold">خط الطول</label>
              <Input value={location.lng ?? ''} readOnly placeholder="سيتم استخراجه تلقائياً" />
            </div>
          </div>
        </div>
      ))}
      <Button type="button" variant="outline" onClick={addLocation}>
        <Plus className="size-4" />
        إضافة رابط جوجل ماب جديد
      </Button>
    </div>
  )
}
