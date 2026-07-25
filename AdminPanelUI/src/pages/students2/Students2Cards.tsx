import {
  BookOpen,
  Calendar,
  Cake,
  Phone,
  User,
  UserCog,
  Users,
} from 'lucide-react'
import { Link } from 'react-router-dom'
import { PUBLIC_SITE_URL } from '@/lib/constants'
import { resolveImageUrl } from '@/lib/resolveImageUrl'
import type { Students2ListItem } from '@/types/students2'

const DEFAULT_IMAGE = `${PUBLIC_SITE_URL}/assets/images/quran.png`

interface Students2CardsProps {
  items: Students2ListItem[]
}

export function Students2Cards({ items }: Students2CardsProps) {
  return (
    <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-3">
      {items.map((item) => (
        <Students2Card key={item.id} item={item} />
      ))}
    </div>
  )
}

function Students2Card({ item }: { item: Students2ListItem }) {
  const registrationDate = item.registrationDate.slice(0, 10).replace(/-/g, '/')

  return (
    <article className="overflow-hidden rounded-2xl border-0 bg-white shadow-md transition hover:-translate-y-1 hover:shadow-xl">
      <div className="bg-gradient-to-br from-[#143b64] to-[#1e528e] px-4 py-5 text-center text-white">
        <img
          src={resolveImageUrl(item.imageUrl) || DEFAULT_IMAGE}
          alt="صورة الطالب"
          className="mx-auto size-20 rounded-full border-[3px] border-[#e0b13f] bg-white object-cover"
          onError={(event) => {
            event.currentTarget.src = DEFAULT_IMAGE
          }}
        />
        <h3 className="mt-3 text-xl font-bold">{item.name}</h3>
      </div>

      <div className="space-y-2.5 p-5">
        <DetailRow icon={<Cake className="size-4" />} label="العمر" value={`${item.age} سنة`} />
        <DetailRow icon={<User className="size-4" />} label="الجنس" value={item.gender} />
        <DetailRow icon={<UserCog className="size-4" />} label="اسم الوالد" value={item.fatherName} />
        <DetailRow icon={<Phone className="size-4" />} label="الهاتف" value={item.fatherPhone} />
        <DetailRow icon={<Users className="size-4" />} label="الحلقة" value={item.circleName || 'غير محدد'} />
        <DetailRow icon={<BookOpen className="size-4" />} label="نوع التسجيل" value={item.registrationType} />
        <DetailRow icon={<Calendar className="size-4" />} label="تاريخ التسجيل" value={registrationDate} />
      </div>

      <div className="border-t bg-slate-50 p-4">
        <Link
          to={`/parents-followup?id=${item.id}`}
          className="flex min-w-[120px] flex-1 items-center justify-center gap-2 rounded-full bg-gradient-to-br from-cyan-600 to-cyan-700 px-4 py-2.5 text-sm font-semibold text-white hover:from-cyan-700 hover:to-cyan-800"
        >
          <Users className="size-4" />
          متابعة الأهل
        </Link>
      </div>
    </article>
  )
}

function DetailRow({
  icon,
  label,
  value,
}: {
  icon: React.ReactNode
  label: string
  value: string
}) {
  return (
    <div className="flex items-center gap-2">
      <span className="flex size-8 shrink-0 items-center justify-center rounded-full bg-slate-100 text-[#e0b13f]">
        {icon}
      </span>
      <span className="min-w-24 font-semibold text-[#143b64]">{label}:</span>
      <span className="text-slate-600">{value}</span>
    </div>
  )
}
