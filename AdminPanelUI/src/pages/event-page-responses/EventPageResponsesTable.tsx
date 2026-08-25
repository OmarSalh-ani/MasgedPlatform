import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn, DataTablePaginationConfig } from '@/components/shared/dataTableTypes'
import type { EventPageResponseListItem } from '@/types/eventPageResponse'

interface EventPageResponsesTableProps {
  items: EventPageResponseListItem[]
  fieldLabels: string[]
  activityName: string
  emptyMessage: string
  isExporting: boolean
  onExport: () => void
  pagination: DataTablePaginationConfig
}

function formatDate(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('ar-KW')
}

function compactAnswers(row: EventPageResponseListItem) {
  if (row.values.length === 0) return '—'
  return row.values.map((item) => `${item.fieldLabel}: ${item.value}`).join(' | ')
}

function getColumns(
  fieldLabels: string[],
  activityFiltered: boolean,
): DataTableColumn<EventPageResponseListItem>[] {
  const columns: DataTableColumn<EventPageResponseListItem>[] = [
    {
      id: 'submittedAt',
      header: 'تاريخ التسجيل',
      accessor: (row) => formatDate(row.submittedAt),
    },
  ]

  if (!activityFiltered) {
    columns.push({ id: 'activityName', header: 'اسم النشاط', accessor: 'activityName' })
    columns.push({
      id: 'answers',
      header: 'الإجابات',
      accessor: (row) => compactAnswers(row),
    })
    return columns
  }

  for (const label of fieldLabels) {
    columns.push({
      id: label,
      header: label,
      accessor: (row) => row.values.find((item) => item.fieldLabel === label)?.value ?? '',
    })
  }

  return columns
}

export function EventPageResponsesTable({
  items,
  fieldLabels,
  activityName,
  emptyMessage,
  isExporting,
  onExport,
  pagination,
}: EventPageResponsesTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(fieldLabels, Boolean(activityName))}
      getRowKey={(row) => String(row.id)}
      emptyMessage={emptyMessage}
      title="ردود التسجيل"
      isExporting={isExporting}
      onExport={onExport}
      pagination={pagination}
    />
  )
}
