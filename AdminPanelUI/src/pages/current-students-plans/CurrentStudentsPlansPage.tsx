import { useState } from 'react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useCurrentStudentsPlans } from '@/hooks/useCurrentStudentsPlans'
import { CurrentStudentsPlansFilters } from '@/pages/current-students-plans/CurrentStudentsPlansFilters'
import { CurrentStudentsPlansTable } from '@/pages/current-students-plans/CurrentStudentsPlansTable'
import { DeleteCurrentStudentPlanDialog } from '@/pages/current-students-plans/dialogs/DeleteCurrentStudentPlanDialog'
import {
  buildCurrentStudentPlanFilters,
  CURRENT_STUDENT_PLAN_PAGE_SIZE,
} from '@/types/currentStudentPlan'

export function CurrentStudentsPlansPage() {
  const [deleteId, setDeleteId] = useState<number | null>(null)
  const [studentId, setStudentId] = useState('')
  const [appliedStudentId, setAppliedStudentId] = useState('')
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(CURRENT_STUDENT_PLAN_PAGE_SIZE)

  const appliedFilters = buildCurrentStudentPlanFilters(appliedStudentId, pageNumber, pageSize)
  const { query, deleteMutation } = useCurrentStudentsPlans(appliedFilters)

  const handleDeleteConfirm = () => {
    if (deleteId === null) return
    deleteMutation.mutate(deleteId, {
      onSettled: () => setDeleteId(null),
    })
  }

  const applyFilters = () => {
    setAppliedStudentId(studentId)
    setPageNumber(1)
  }

  const handlePageSizeChange = (size: number) => {
    setPageSize(size)
    setPageNumber(1)
  }

  if (query.isLoading && !query.data) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (query.isError) {
    return (
      <Alert variant="destructive">
        تعذر تحميل خطط الطلاب الحالية. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  const list = query.data
  const items = list?.items ?? []

  return (
    <div>
      <PageHeader title="خطط الطلاب الحالية" />

      <CurrentStudentsPlansFilters
        studentId={studentId}
        isLoading={query.isFetching}
        onStudentIdChange={setStudentId}
        onApply={applyFilters}
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف الخطة. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <CurrentStudentsPlansTable
        items={items}
        pageNumber={list?.pageNumber ?? pageNumber}
        pageSize={list?.pageSize ?? pageSize}
        totalCount={list?.totalCount ?? 0}
        totalPages={list?.totalPages ?? 0}
        onDelete={setDeleteId}
        onPageChange={setPageNumber}
        onPageSizeChange={handlePageSizeChange}
      />

      <DeleteCurrentStudentPlanDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
