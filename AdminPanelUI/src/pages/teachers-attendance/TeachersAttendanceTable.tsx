import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn } from '@/components/shared/dataTableTypes'
import {
  formatHoursWorked,
  type TeachersAttendanceRow,
} from '@/types/teachersAttendance'

interface TeachersAttendanceTableProps {
  items: TeachersAttendanceRow[]
  onExport: () => void
  isExporting: boolean
}

const columns: DataTableColumn<TeachersAttendanceRow>[] = [
  {
    id: 'teacherName',
    header: 'اسم المعلم',
    accessor: 'teacherName',
  },
  {
    id: 'attendanceDateTime',
    header: 'وقت الحضور',
    accessor: 'attendanceDateTime',
  },
  {
    id: 'departureDateTime',
    header: 'وقت المغادرة',
    accessor: (row) => row.departureDateTime ?? '-',
  },
  {
    id: 'hoursWorked',
    header: 'عدد الساعات',
    accessor: (row) => formatHoursWorked(row.hoursWorked),
    className: 'font-semibold text-[#7C8738]',
  },
  {
    id: 'status',
    header: 'الحالة',
    cell: (row) => <StatusBadge status={row.status} statusClass={row.statusClass} />,
  },
]

export function TeachersAttendanceTable({
  items,
  onExport,
  isExporting,
}: TeachersAttendanceTableProps) {
  return (
    <DataTable
      data={items}
      columns={columns}
      getRowKey={(row, index) => `${row.teacherName}-${row.attendanceDateTime}-${index}`}
      onExport={onExport}
      isExporting={isExporting}
    />
  )
}

function StatusBadge({
  status,
  statusClass,
}: {
  status: string
  statusClass: string
}) {
  const isPresent = statusClass === 'status-present'
  return (
    <span
      className={`inline-block rounded-full px-3 py-1 text-xs font-semibold ${
        isPresent ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
      }`}
    >
      {status}
    </span>
  )
}
