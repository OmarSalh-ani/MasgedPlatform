import { Link } from 'react-router-dom'
import { Pencil, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import type { SocialLinkListItem } from '@/types/socialLink'

interface SocialLinksTableProps {
  items: SocialLinkListItem[]
  onDelete: (id: number) => void
}

export function SocialLinksTable({ items, onDelete }: SocialLinksTableProps) {
  return (
    <ul className="divide-y divide-slate-100">
      {items.map((item) => (
        <li
          key={item.id}
          className="flex flex-wrap items-center justify-between gap-2 px-5 py-4"
        >
          <div>
            <strong className="block">{item.platformName}</strong>
            <span className="text-sm text-slate-500">{item.url}</span>
          </div>
          <div className="flex gap-2">
            <Link
              to={`/social-links/${item.id}/edit`}
              className="inline-flex items-center gap-1 rounded-lg bg-[var(--color-primary)] px-3 py-1.5 text-sm text-white hover:opacity-90"
            >
              <Pencil className="size-4" />
              تعديل
            </Link>
            <Button
              type="button"
              variant="outline"
              className="rounded-lg border-red-200 px-3 py-1.5 text-sm text-red-600 hover:bg-red-50"
              onClick={() => onDelete(item.id)}
            >
              <Trash2 className="size-4" />
              حذف
            </Button>
          </div>
        </li>
      ))}
    </ul>
  )
}
