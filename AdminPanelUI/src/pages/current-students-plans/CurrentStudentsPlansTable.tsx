import { Link } from 'react-router-dom'
import { Eye, Trash2 } from 'lucide-react'
import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import { Button } from '@/components/ui/button'
import type { CurrentStudentPlanListItem } from '@/types/currentStudentPlan'
import {
  CURRENT_STUDENT_PLAN_PAGE_SIZE_OPTIONS,
  formatPlanCreatedAt,
  formatPlanDate,
} from '@/types/currentStudentPlan'

interface CurrentStudentsPlansTableProps {
  items: CurrentStudentPlanListItem[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  onDelete: (id: number) => void
  onPageChange: (page: number) => void
  onPageSizeChange: (size: number) => void
}

function getColumns(
  onDelete: (id: number) => void,
): DataTableColumn<CurrentStudentPlanListItem>[] {
  return [
    {
      id: 'circleName',
      header: 'الحلقة',
      accessor: 'circleName',
    },
    {
      id: 'studentName',
      header: 'الطالب',
      accessor: 'studentName',
    },
    {
      id: 'planName',
      header: 'الخطة',
      accessor: 'planName',
    },
    {
      id: 'fromDate',
      header: 'من تاريخ',
      accessor: (row) => formatPlanDate(row.fromDate),
    },
    {
      id: 'toDate',
      header: 'إلى تاريخ',
      accessor: (row) => formatPlanDate(row.toDate),
    },
    {
      id: 'totalDays',
      header: 'عدد الايام',
      accessor: 'totalDays',
    },
    {
      id: 'elapsedDays',
      header: 'الايام المنقضية',
      accessor: 'elapsedDays',
    },
    {
      id: 'remainingDays',
      header: 'الايام حتى انتهاء الخطة',
      accessor: 'remainingDays',
    },
    {
      id: 'createdAt',
      header: 'تاريخ الإنشاء',
      accessor: (row) => formatPlanCreatedAt(row.createdAt),
    },
    {
      id: 'actions',
      header: 'إجراءات',
      className: 'text-center',
      cell: (row) => (
        <div className="flex flex-nowrap justify-center gap-2">
          <Link
            to={`/student-plans?studentId=${row.studentId}&planId=${row.id}`}
            className="inline-flex items-center gap-1 rounded-lg bg-cyan-600 px-3 py-1.5 text-xs text-white hover:opacity-90"
          >
            <Eye className="size-3.5" />
            عرض
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

export function CurrentStudentsPlansTable({
  items,
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  onDelete,
  onPageChange,
  onPageSizeChange,
}: CurrentStudentsPlansTableProps) {
  return (
    <DataTable
      data={items}
      columns={getColumns(onDelete)}
      getRowKey={(row) => String(row.id)}
      emptyMessage="لا توجد خطط حالية."
      showExport={false}
      pagination={{
        pageNumber,
        pageSize,
        totalCount,
        totalPages,
        pageSizeOptions: CURRENT_STUDENT_PLAN_PAGE_SIZE_OPTIONS,
        itemLabel: 'خطة',
        onPageChange,
        onPageSizeChange,
      }}
    />
  )
}
