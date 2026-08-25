import { Link } from 'react-router-dom'
import { Copy, Pencil, Trash2 } from 'lucide-react'
import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { Button } from '@/components/ui/button'
import { PUBLIC_SITE_URL } from '@/lib/constants'
import type { EventPageListItem } from '@/types/eventPage'

interface EventPagesTableProps {
  items: EventPageListItem[]
  emptyMessage: string
  onDelete: (id: number) => void
  onCopied: () => void
}

function publicUrl(slug: string) {
  return `${PUBLIC_SITE_URL.replace(/\/$/, '')}/p/${slug}`
}

function getColumns(
  onDelete: (id: number) => void,
  onCopied: () => void,
): DataTableColumn<EventPageListItem>[] {
  return [
    { id: 'activityName', header: 'اسم النشاط', accessor: 'activityName' },
    { id: 'courseTitle', header: 'العنوان', accessor: 'courseTitle' },
    { id: 'slug', header: 'الرابط', accessor: 'slug' },
    {
      id: 'status',
      header: 'الحالة',
      accessor: (row) =>
        `${row.isPublished ? 'منشورة' : 'غير منشورة'} / ${row.isRegistrationOpen ? 'التسجيل مفتوح' : 'التسجيل مغلق'}`,
    },
    {
      id: 'actions',
      header: 'إجراءات',
      className: 'text-center',
      cell: (row) => (
        <div className="flex flex-nowrap justify-center gap-2">
          <Button
            type="button"
            variant="outline"
            className="px-3 py-1.5 text-xs"
            onClick={() => {
              void navigator.clipboard.writeText(publicUrl(row.slug)).then(onCopied)
            }}
          >
            <Copy className="size-3.5" />
            نسخ الرابط
          </Button>
          <Link
            to={`/event-pages/${row.id}/edit`}
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

export function EventPagesTable({ items, emptyMessage, onDelete, onCopied }: EventPagesTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(onDelete, onCopied)}
      getRowKey={(row) => String(row.id)}
      emptyMessage={emptyMessage}
      title="صفحات التسجيل"
      showExport={false}
    />
  )
}
