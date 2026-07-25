import { GraduationCap } from 'lucide-react'
import { useState } from 'react'
import { DataTable } from '@/components/shared/DataTable'
import type { DataTableColumn, DataTableViewMode } from '@/components/shared/dataTableTypes'
import { resolveImageUrl } from '@/lib/resolveImageUrl'
import { HomeStudentCard } from '@/pages/home/HomeStudentCard'
import {
  HomeStudentActionButtons,
  toSelectedStudent,
} from '@/pages/home/HomeStudentCardParts'
import {
  loadHomeStudentsLayout,
  saveHomeStudentsLayout,
} from '@/pages/home/homeUtils'
import { HOME_PAGE_SIZE_OPTIONS } from '@/types/home'
import type { HomeStudentListItem, SelectedHomeStudent } from '@/types/home'

interface HomeTableProps {
  items: HomeStudentListItem[]
  selectedIds: Set<number>
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  canModify: boolean
  onToggleStudent: (student: SelectedHomeStudent) => void
  onDelete: (id: number) => void
  onShowTests: (id: number, name: string) => void
  onShowReviews: (id: number, name: string) => void
  onPageChange: (page: number) => void
  onPageSizeChange: (size: number) => void
}

function getColumns(
  selectedIds: Set<number>,
  canModify: boolean,
  onToggleStudent: (student: SelectedHomeStudent) => void,
  onDelete: (id: number) => void,
  onShowTests: (id: number, name: string) => void,
  onShowReviews: (id: number, name: string) => void,
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
          onChange={() => onToggleStudent(toSelectedStudent(row))}
        />
      ),
    },
    {
      id: 'studentName',
      header: 'اسم الطالب',
      cell: (row) => (
        <div className="flex items-center gap-2">
          <div className="flex size-9 shrink-0 items-center justify-center overflow-hidden rounded-full bg-slate-100">
            {row.studentImage ? (
              <img src={resolveImageUrl(row.studentImage)} alt={row.studentName} className="size-full object-cover" />
            ) : (
              <GraduationCap className="size-4 text-[var(--color-primary)]" />
            )}
          </div>
          <div>
            <p className="font-semibold text-slate-900">{row.studentName}</p>
            <p className="text-xs text-slate-500">#{row.id}</p>
          </div>
        </div>
      ),
    },
    {
      id: 'circleName',
      header: 'الحلقة',
      accessor: (row) => row.circleName || '—',
    },
    {
      id: 'planLevelName',
      header: 'المستوى',
      cell: (row) => (
        <span className="inline-flex rounded-full bg-[#CBAC2D] px-2.5 py-0.5 text-xs font-bold text-white">
          {row.planLevelName}
        </span>
      ),
    },
    {
      id: 'fatherPhone',
      header: 'هاتف ولي الأمر',
      accessor: 'fatherPhone',
    },
    {
      id: 'completeFollowup',
      header: 'الاستمارة',
      accessor: 'completeFollowup',
    },
    {
      id: 'actions',
      header: 'إجراءات',
      className: 'min-w-[280px]',
      cell: (row) => (
        <HomeStudentActionButtons
          item={row}
          canModify={canModify}
          onDelete={onDelete}
          onShowTests={onShowTests}
          onShowReviews={onShowReviews}
          compact
        />
      ),
    },
  ]
}

function loadInitialViewMode(): DataTableViewMode {
  return loadHomeStudentsLayout() === 'grid' ? 'card' : 'list'
}

export function HomeTable({
  items,
  selectedIds,
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  canModify,
  onToggleStudent,
  onDelete,
  onShowTests,
  onShowReviews,
  onPageChange,
  onPageSizeChange,
}: HomeTableProps) {
  const [viewMode, setViewMode] = useState<DataTableViewMode>(loadInitialViewMode)

  const handleViewModeChange = (mode: DataTableViewMode) => {
    setViewMode(mode)
    saveHomeStudentsLayout(mode === 'card' ? 'grid' : 'list')
  }

  const itemProps = {
    canModify,
    onToggle: onToggleStudent,
    onDelete,
    onShowTests,
    onShowReviews,
  }

  return (
    <DataTable
      data={items}
      columns={getColumns(
        selectedIds,
        canModify,
        onToggleStudent,
        onDelete,
        onShowTests,
        onShowReviews,
      )}
      getRowKey={(row) => String(row.id)}
      emptyMessage="لا يوجد طلاب متاحين"
      showExport={false}
      viewMode={viewMode}
      onViewModeChange={handleViewModeChange}
      defaultViewMode={viewMode}
      renderCard={(row) => (
        <HomeStudentCard item={row} selected={selectedIds.has(row.id)} {...itemProps} />
      )}
      pagination={{
        pageNumber,
        pageSize,
        totalCount,
        totalPages,
        pageSizeOptions: HOME_PAGE_SIZE_OPTIONS,
        itemLabel: 'طالب',
        onPageChange,
        onPageSizeChange,
      }}
    />
  )
}
