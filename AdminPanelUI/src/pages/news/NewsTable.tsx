import { Link } from 'react-router-dom'
import { Pencil, Trash2 } from 'lucide-react'
import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { Button } from '@/components/ui/button'
import type { NewsListItem } from '@/types/news'
import { formatNewsDate } from '@/types/news'

interface NewsTableProps {
  items: NewsListItem[]
  emptyMessage: string
  onDelete: (id: number) => void
}

function getColumns(onDelete: (id: number) => void): DataTableColumn<NewsListItem>[] {
  return [
    {
      id: 'title',
      header: 'العنوان',
      accessor: 'title',
    },
    {
      id: 'newsDate',
      header: 'التاريخ',
      accessor: (row) => formatNewsDate(row.newsDate),
    },
    {
      id: 'actions',
      header: 'إجراءات',
      className: 'text-center',
      cell: (row) => (
        <div className="flex flex-nowrap justify-center gap-2">
          <Link
            to={`/news/${row.id}/edit`}
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

export function NewsTable({ items, emptyMessage, onDelete }: NewsTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(onDelete)}
      getRowKey={(row) => String(row.id)}
      emptyMessage={emptyMessage}
      title="قائمة الأخبار"
      showExport={false}
    />
  )
}
