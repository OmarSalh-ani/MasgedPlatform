import { List, Pencil, Printer, Trash2 } from 'lucide-react'
import { Link } from 'react-router-dom'
import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { Button } from '@/components/ui/button'
import { TeacherCard } from '@/pages/teachers/TeacherCard'
import type { TeacherListItem } from '@/types/teacher'

interface TeachersTableProps {
  items: TeacherListItem[]
  emptyMessage: string
  canModify: boolean
  onDelete: (id: number, name: string) => void
}

function getColumns(
  canModify: boolean,
  onDelete: (id: number, name: string) => void,
): DataTableColumn<TeacherListItem>[] {
  return [
    {
      id: 'name',
      header: 'اسم المعلم',
      accessor: 'name',
    },
    {
      id: 'id',
      header: 'رقم المعلم',
      accessor: 'id',
    },
    {
      id: 'mobile',
      header: 'الموبايل',
      accessor: (row) => row.mobile ?? '—',
    },
    {
      id: 'email',
      header: 'البريد الإلكتروني',
      accessor: 'email',
    },
    {
      id: 'circleCount',
      header: 'الحلقات',
      accessor: (row) => `${row.circleCount} حلقة`,
    },
    {
      id: 'usersManage',
      header: 'الصلاحية',
      cell: (row) =>
        row.usersManage ? (
          <span className="inline-flex rounded-full bg-emerald-600 px-2.5 py-0.5 text-xs font-semibold text-white">
            أدمن عام
          </span>
        ) : (
          '—'
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
              to={`/teachers/${row.id}/edit`}
              className="inline-flex items-center gap-1 rounded-lg bg-gradient-to-br from-[var(--color-primary)] to-[#1a5f8a] px-3 py-1.5 text-xs text-white hover:opacity-90"
            >
              <Pencil className="size-3.5" />
              تعديل
            </Link>
          )}
          <Link
            to={`/circles?teacher=${row.id}`}
            className="inline-flex items-center gap-1 rounded-lg bg-amber-600 px-3 py-1.5 text-xs text-white hover:bg-amber-700"
          >
            <List className="size-3.5" />
            الحلقات
          </Link>
          <Link
            to={`/teachers/${row.id}/card-print`}
            target="_blank"
            rel="noreferrer"
            className="inline-flex items-center gap-1 rounded-lg bg-amber-600 px-3 py-1.5 text-xs text-white hover:bg-amber-700"
          >
            <Printer className="size-3.5" />
            طباعة
          </Link>
          {canModify && (
            <Button
              type="button"
              variant="outline"
              className="border-red-200 px-3 py-1.5 text-xs text-red-600 hover:bg-red-50"
              onClick={() => onDelete(row.id, row.name)}
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

export function TeachersTable({ items, emptyMessage, canModify, onDelete }: TeachersTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(canModify, onDelete)}
      getRowKey={(row) => String(row.id)}
      emptyMessage={emptyMessage}
      title="قائمة المعلمين"
      defaultViewMode="card"
      showExport={false}
      renderCard={(row) => (
        <TeacherCard item={row} canModify={canModify} onDelete={onDelete} />
      )}
    />
  )
}
