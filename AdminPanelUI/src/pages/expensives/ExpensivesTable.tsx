import { Eye, Pencil, Trash2 } from 'lucide-react'
import { Link } from 'react-router-dom'
import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { Button } from '@/components/ui/button'
import { formatAmount, formatDateGeorgian } from '@/lib/utils'
import { ExpensiveCard } from '@/pages/expensives/ExpensiveCard'
import type { ExpensiveListItem } from '@/types/expensives'

interface ExpensivesTableProps {
  items: ExpensiveListItem[]
  emptyMessage: string
  onDelete: (id: number) => void
  onExport: () => void
  isExporting: boolean
}

function getColumns(onDelete: (id: number) => void): DataTableColumn<ExpensiveListItem>[] {
  return [
    {
      id: 'id',
      header: '#',
      accessor: 'id',
    },
    {
      id: 'reason',
      header: 'السبب',
      accessor: 'reason',
    },
    {
      id: 'supplier',
      header: 'المورد',
      accessor: 'supplier',
    },
    {
      id: 'totalAmount',
      header: 'المبلغ',
      accessor: (row) => `${formatAmount(row.totalAmount)} د.ك`,
      className: 'font-semibold text-[var(--color-primary)]',
    },
    {
      id: 'createdAt',
      header: 'التاريخ',
      accessor: (row) => formatDateGeorgian(row.createdAt),
    },
    {
      id: 'createdBy',
      header: 'المسؤول',
      accessor: (row) => row.createdBy || '—',
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
          <Link
            to={`/expensives/${row.id}`}
            className="inline-flex items-center gap-1 rounded-lg bg-[var(--color-primary)] px-3 py-1.5 text-xs text-white hover:opacity-90"
          >
            <Eye className="size-3.5" />
            عرض
          </Link>
          <Link
            to={`/expensives/${row.id}/edit`}
            className="inline-flex items-center gap-1 rounded-lg bg-cyan-600 px-3 py-1.5 text-xs text-white hover:bg-cyan-700"
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

export function ExpensivesTable({
  items,
  emptyMessage,
  onDelete,
  onExport,
  isExporting,
}: ExpensivesTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(onDelete)}
      getRowKey={(row) => String(row.id)}
      emptyMessage={emptyMessage}
      defaultViewMode="card"
      onExport={onExport}
      isExporting={isExporting}
      renderCard={(row) => <ExpensiveCard item={row} onDelete={onDelete} />}
    />
  )
}
