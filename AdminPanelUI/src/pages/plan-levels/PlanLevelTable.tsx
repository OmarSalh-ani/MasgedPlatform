import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { Button } from '@/components/ui/button'
import type { PlanLevelListItem } from '@/types/planLevel'
import { formatPlanLevelCreatedAt } from '@/types/planLevel'

interface PlanLevelTableProps {
  items: PlanLevelListItem[]
  onDelete: (id: number) => void
}

function getColumns(onDelete: (id: number) => void): DataTableColumn<PlanLevelListItem>[] {
  return [
    {
      id: 'levelName',
      header: 'اسم المستوى',
      accessor: 'levelName',
    },
    {
      id: 'quantity',
      header: 'القدرة',
      accessor: (row) => `${row.unitTypeDisplay} (${row.quantity})`,
    },
    {
      id: 'isGlobal',
      header: 'النوع',
      cell: (row) =>
        row.isGlobal ? (
          <span className="inline-flex rounded-full bg-green-600 px-2.5 py-0.5 text-xs font-medium text-white">
            عام
          </span>
        ) : (
          <span className="inline-flex rounded-full bg-purple-600 px-2.5 py-0.5 text-xs font-medium text-white">
            خاص بالمعلم
          </span>
        ),
    },
    {
      id: 'createdAt',
      header: 'تاريخ الإنشاء',
      accessor: (row) => formatPlanLevelCreatedAt(row.createdAt),
    },
    {
      id: 'actions',
      header: 'إجراءات',
      cell: (row) => (
        <Button
          type="button"
          variant="outline"
          className="rounded-lg border-red-200 px-3 py-1.5 text-sm text-red-600 hover:bg-red-50"
          onClick={() => onDelete(row.id)}
        >
          حذف
        </Button>
      ),
    },
  ]
}

export function PlanLevelTable({ items, onDelete }: PlanLevelTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(onDelete)}
      getRowKey={(row) => String(row.id)}
      emptyMessage="لا توجد مستويات بعد"
      showExport={false}
      showViewSwitcher={false}
    />
  )
}
