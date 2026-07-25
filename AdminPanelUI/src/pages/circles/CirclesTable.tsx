import { Pencil, Trash2, Users } from 'lucide-react'
import { Link } from 'react-router-dom'
import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { Button } from '@/components/ui/button'
import { CircleCard } from '@/pages/circles/CircleCard'
import type { CircleListItem } from '@/types/circle'
import { formatCircleCreatedAt } from '@/types/circle'

interface CirclesTableProps {
  items: CircleListItem[]
  emptyMessage: string
  canModify: boolean
  onDelete: (id: number) => void
  onExport: () => void
  isExporting: boolean
}

function getColumns(
  canModify: boolean,
  onDelete: (id: number) => void,
): DataTableColumn<CircleListItem>[] {
  return [
    {
      id: 'name',
      header: 'اسم الحلقة',
      accessor: 'name',
    },
    {
      id: 'teacherName',
      header: 'المعلم',
      accessor: 'teacherName',
    },
    {
      id: 'studentsCount',
      header: 'الطلاب',
      accessor: (row) => `${row.studentsCount} طالب`,
    },
    {
      id: 'createdAt',
      header: 'تاريخ الإنشاء',
      accessor: (row) => formatCircleCreatedAt(row.createdAt),
    },
    {
      id: 'forGirls',
      header: 'القسم',
      cell: (row) => (
        <span className="inline-flex rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-700">
          {row.forGirls ? 'نساء' : 'رجال'}
        </span>
      ),
    },
    {
      id: 'actions',
      header: 'إجراءات',
      className: 'text-center',
      cell: (row) => (
        <div className="flex flex-nowrap justify-center gap-2">
          {canModify && (
            <Link
              to={`/circles/${row.id}/edit`}
              className="inline-flex items-center gap-1 rounded-lg bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-3 py-1.5 text-xs text-white hover:opacity-90"
            >
              <Pencil className="size-3.5" />
              تعديل
            </Link>
          )}
          <a
            href={`/home?circle=${row.id}`}
            target="_blank"
            rel="noreferrer"
            className="inline-flex items-center gap-1 rounded-lg bg-emerald-600 px-3 py-1.5 text-xs text-white hover:bg-emerald-700"
          >
            <Users className="size-3.5" />
            الطلاب
          </a>
          {canModify && (
            <Button
              type="button"
              variant="outline"
              className="border-red-200 px-3 py-1.5 text-xs text-red-600 hover:bg-red-50"
              onClick={() => onDelete(row.id)}
            >
              <Trash2 className="size-3.5" />
              حذف
            </Button>
          )}
        </div>
      ),
    },
  ]
}

export function CirclesTable({
  items,
  emptyMessage,
  canModify,
  onDelete,
  onExport,
  isExporting,
}: CirclesTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(canModify, onDelete)}
      getRowKey={(row) => String(row.id)}
      emptyMessage={emptyMessage}
      defaultViewMode="card"
      onExport={onExport}
      isExporting={isExporting}
      renderCard={(row) => (
        <CircleCard item={row} canModify={canModify} onDelete={onDelete} />
      )}
    />
  )
}
