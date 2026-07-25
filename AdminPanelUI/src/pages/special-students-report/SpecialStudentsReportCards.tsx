import { Phone, Star, Users } from 'lucide-react'
import { resolveImageUrl } from '@/lib/resolveImageUrl'
import type { SpecialStudentsReportItem } from '@/types/specialStudentsReport'

const PLACEHOLDER_IMAGE =
  'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iODAiIGhlaWdodD0iODAiIHZpZXdCb3g9IjAgMCA4MCA4MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGNpcmNsZSBjeD0iNDAiIGN5PSI0MCIgcj0iNDAiIGZpbGw9IiNGM0Y0RjYiLz4KPGNpcmNsZSBjeD0iNDAiIGN5PSIzMiIgcj0iMTIiIGZpbGw9IiM5Q0EzQUYiLz4KPHBhdGggZD0iTTIwIDY4QzIwIDU5LjE2MzQgMjcuMTYzNCA1MiAzNiA1Mkg0NEM1Mi44MzY2IDUyIDYwIDU5LjE2MzQgNjAgNjhWNzJIMjBWNjhaIiBmaWxsPSIjOUNBM0FGIi8+Cjwvc3ZnPgo='

interface SpecialStudentsReportCardsProps {
  items: SpecialStudentsReportItem[]
}

export function SpecialStudentsReportCards({ items }: SpecialStudentsReportCardsProps) {
  return (
    <div className="grid gap-5 sm:grid-cols-2 xl:grid-cols-3 print:grid-cols-2">
      {items.map((item, index) => (
        <article
          key={`${item.circleId ?? 'none'}-${item.studentName}-${index}`}
          className="overflow-hidden rounded-2xl border-2 border-[#ffd700] bg-gradient-to-br from-white to-slate-50 shadow-md transition hover:-translate-y-1 hover:shadow-xl print:break-inside-avoid print:shadow-none"
        >
          <div className="p-5">
            <div className="mb-4 flex items-center gap-3">
              <img
                src={resolveImageUrl(item.imageUrl) || PLACEHOLDER_IMAGE}
                alt={item.studentName}
                className="size-20 rounded-full border-[3px] border-[#ffd700] object-cover"
                onError={(event) => {
                  event.currentTarget.src = PLACEHOLDER_IMAGE
                }}
              />
              <div className="min-w-0 flex-1">
                <span className="mb-2 inline-flex items-center gap-1 rounded-full bg-gradient-to-r from-[#ffd700] to-[#ffed4e] px-3 py-1 text-xs font-bold text-[#8b6914]">
                  <Star className="size-3 fill-current" />
                  طالب مميز
                </span>
                <h3 className="text-lg font-bold text-[#2c5aa0]">{item.studentName}</h3>
              </div>
            </div>

            <div className="space-y-2">
              <p className="flex items-center gap-2 font-semibold text-emerald-600">
                <Users className="size-4 shrink-0" />
                الحلقة: {item.circleName}
              </p>
              <p className="flex items-center gap-2 text-slate-500">
                <Phone className="size-4 shrink-0" />
                <span>
                  هاتف الوالد:{' '}
                  <span dir="ltr" className="inline-block">
                    {item.fatherPhone}
                  </span>
                </span>
              </p>
            </div>
          </div>
        </article>
      ))}
    </div>
  )
}
