import { Link } from 'react-router-dom'
import { Pencil, Trash2 } from 'lucide-react'
import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { Button } from '@/components/ui/button'
import { resolveImageUrl } from '@/lib/resolveImageUrl'
import type { HeroSlideListItem } from '@/types/heroSlide'

interface HeroSlidesTableProps {
  items: HeroSlideListItem[]
  emptyMessage: string
  onDelete: (id: number) => void
}

function getColumns(onDelete: (id: number) => void): DataTableColumn<HeroSlideListItem>[] {
  return [
    {
      id: 'image',
      header: 'الصورة',
      cell: (row) => {
        const imageSrc = resolveImageUrl(row.imageUrl)
        if (!imageSrc) return '—'

        return (
          <img
            src={imageSrc}
            alt=""
            className="h-[45px] w-20 rounded-lg object-cover"
            onError={(e) => {
              e.currentTarget.style.display = 'none'
            }}
          />
        )
      },
    },
    {
      id: 'sortOrder',
      header: 'الترتيب',
      accessor: 'sortOrder',
    },
    {
      id: 'imageUrl',
      header: 'رابط الصورة',
      accessor: (row) => row.imageUrl ?? '—',
    },
    {
      id: 'actions',
      header: 'إجراءات',
      className: 'text-center',
      cell: (row) => (
        <div className="flex flex-nowrap justify-center gap-2">
          <Link
            to={`/hero-slides/${row.id}/edit`}
            className="inline-flex items-center gap-1 rounded-lg bg-[var(--color-primary)] px-3 py-1.5 text-xs text-white hover:opacity-90"
          >
            <Pencil className="size-3.5" />
            تعديل
          </Link>
          <Button
            type="button"
            variant="outline"
            className="border-red-200 px-3 py-1.5 text-xs text-red-600 hover:bg-red-50"
            onClick={() => onDelete(row.id)}
          >
            <Trash2 className="size-3.5" />
            حذف
          </Button>
        </div>
      ),
    },
  ]
}

export function HeroSlidesTable({ items, emptyMessage, onDelete }: HeroSlidesTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(onDelete)}
      getRowKey={(row) => String(row.id)}
      emptyMessage={emptyMessage}
      title="قائمة الصور"
      showExport={false}
    />
  )
}
