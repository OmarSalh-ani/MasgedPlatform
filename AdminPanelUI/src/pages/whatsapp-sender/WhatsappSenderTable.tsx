import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import {
  WHATSAPP_SENDER_PAGE_SIZE_OPTIONS,
  formatWhatsappSenderDate,
  type HomeStudentListItem,
  type SelectedWhatsappSenderStudent,
} from '@/types/whatsappSender'

interface WhatsappSenderTableProps {
  items: HomeStudentListItem[]
  selectedIds: Set<number>
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  onToggleStudent: (student: SelectedWhatsappSenderStudent) => void
  onPageChange: (page: number) => void
  onPageSizeChange: (size: number) => void
}

function getColumns(
  selectedIds: Set<number>,
  onToggleStudent: (student: SelectedWhatsappSenderStudent) => void,
): DataTableColumn<HomeStudentListItem>[] {
  return [
    {
      id: 'select',
      header: 'تحديد',
      className: 'text-center',
      cell: (row) => (
        <input
          type="checkbox"
          checked={selectedIds.has(row.id)}
          onChange={() =>
            onToggleStudent({
              id: row.id,
              studentName: row.studentName,
              fatherName: row.fatherName,
              fatherPhone: row.fatherPhone,
              circleName: row.circleName,
            })
          }
        />
      ),
    },
    {
      id: 'id',
      header: 'رقم الطالب',
      accessor: 'id',
      className: 'text-center',
    },
    {
      id: 'studentName',
      header: 'اسم الطالب',
      accessor: 'studentName',
      className: 'text-center',
    },
    {
      id: 'fatherName',
      header: 'اسم الأب',
      accessor: 'fatherName',
      className: 'text-center',
    },
    {
      id: 'fatherPhone',
      header: 'هاتف ولي الأمر',
      accessor: 'fatherPhone',
      className: 'text-center',
    },
    {
      id: 'birthdate',
      header: 'تاريخ الميلاد',
      accessor: (row) => formatWhatsappSenderDate(row.birthdate),
      className: 'text-center',
    },
    {
      id: 'age',
      header: 'العمر',
      accessor: 'age',
      className: 'text-center',
    },
    {
      id: 'circleName',
      header: 'الحلقة',
      accessor: 'circleName',
      className: 'text-center',
    },
    {
      id: 'isSpecial',
      header: 'طالب مميز',
      accessor: 'isSpecial',
      className: 'text-center',
    },
    {
      id: 'createdAt',
      header: 'تاريخ التسجيل',
      accessor: (row) => formatWhatsappSenderDate(row.createdAt),
      className: 'text-center',
    },
    {
      id: 'completeFollowup',
      header: 'الاستمارة متكملة',
      accessor: 'completeFollowup',
      className: 'text-center',
    },
  ]
}

export function WhatsappSenderTable({
  items,
  selectedIds,
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  onToggleStudent,
  onPageChange,
  onPageSizeChange,
}: WhatsappSenderTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(selectedIds, onToggleStudent)}
      getRowKey={(row) => String(row.id)}
      emptyMessage="لا يوجد طلاب متاحين"
      showExport={false}
      showViewSwitcher={false}
      pagination={{
        pageNumber,
        pageSize,
        totalCount,
        totalPages,
        pageSizeOptions: WHATSAPP_SENDER_PAGE_SIZE_OPTIONS,
        itemLabel: 'طالب',
        onPageChange,
        onPageSizeChange,
      }}
    />
  )
}
