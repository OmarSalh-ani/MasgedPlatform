import { BarChart3, CheckSquare, MessageCircle, PlusCircle, Shuffle, Square, Users } from 'lucide-react'
import { Link } from 'react-router-dom'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { CreateCircleDialog } from '@/pages/home/dialogs/CreateCircleDialog'
import { DeleteStudentDialog } from '@/pages/home/dialogs/DeleteStudentDialog'
import { HomeWhatsappDialog } from '@/pages/home/dialogs/HomeWhatsappDialog'
import { TransferStudentsDialog } from '@/pages/home/dialogs/TransferStudentsDialog'
import { HomeTable } from '@/pages/home/HomeTable'
import { OthaiminCenterStudentReviewsDialog } from '@/pages/OthaiminCenter/dialogs/OthaiminCenterStudentReviewsDialog'
import { OthaiminCenterStudentTestsDialog } from '@/pages/OthaiminCenter/dialogs/OthaiminCenterStudentTestsDialog'
import { OthaiminCenterFilters } from '@/pages/OthaiminCenter/OthaiminCenterFilters'
import { buildAppliedOthaiminCenterFilters } from '@/pages/OthaiminCenter/othaiminCenterUtils'
import { OTHAIMIN_CENTER_GRADIENT, OTHAIMIN_CENTER_MODE_LABEL } from '@/pages/OthaiminCenter/othaiminCenterTheme'
import { useOthaiminCenterPage } from '@/pages/OthaiminCenter/useOthaiminCenterPage'
import '@/pages/OthaiminCenter/othaiminCenter.css'

export function OthaiminCenterPage() {
  const page = useOthaiminCenterPage()

  if (page.listQuery.isLoading && !page.list) {
    return (
      <div className="othaimin-center-mode space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  const handleWhatsapp = (payload: { message: string; image?: File | null }) => {
    page.whatsappMutation.mutate(
      { studentIds: page.selectedList.map((item) => item.id), message: payload.message, image: payload.image },
      { onSuccess: (message) => { page.setActionMessage(message); page.setWhatsappOpen(false) } },
    )
  }

  const handleTransfer = (circleId: number) => {
    page.transferMutation.mutate(
      { studentIds: page.selectedList.map((item) => item.id), circleId },
      { onSuccess: () => { page.setTransferOpen(false); page.clearSelection(); page.applyFilters(page.pageNumber) } },
    )
  }

  const handleCreateCircle = (payload: { circleName: string; teacherId: number }) => {
    page.createCircleMutation.mutate(
      { circleName: payload.circleName, teacherId: payload.teacherId, studentIds: page.selectedList.map((item) => item.id) },
      { onSuccess: () => { page.setCreateCircleOpen(false); page.clearSelection(); page.applyFilters(1) } },
    )
  }

  return (
    <div className="othaimin-center-mode mx-auto max-w-7xl space-y-6">
      <PageHeader
        title={page.pageTitle}
        description={`إدارة الطلاب والمتابعة — ${OTHAIMIN_CENTER_MODE_LABEL}`}
        gradientClassName={OTHAIMIN_CENTER_GRADIENT}
        className="mb-0"
      >
        <span className="mt-3 inline-block rounded-full bg-white/20 px-4 py-1 text-sm font-semibold">
          {OTHAIMIN_CENTER_MODE_LABEL}
        </span>
      </PageHeader>

      <div className="text-center">
        <Link to="/statistics" className="inline-flex items-center gap-2 rounded-full bg-[var(--color-primary)] px-6 py-3 text-white hover:opacity-90">
          <BarChart3 className="size-5" />
          عرض الإحصائيات
        </Link>
      </div>

      {page.actionMessage ? <Alert>{page.actionMessage}</Alert> : null}
      {page.listQuery.isError ? <Alert variant="destructive">تعذر تحميل قائمة الطلاب</Alert> : null}

      <OthaiminCenterFilters form={page.filterForm} options={page.filterOptionsQuery.data} isGirlTeacher={page.isGirlTeacher} isExporting={page.exportMutation.isPending} onChange={page.setFilterForm} onApply={() => page.applyFilters(1)} onClear={page.handleClearFilters} onExport={() => page.exportMutation.mutate()} />

      <section className="rounded-xl border border-[var(--color-primary-muted)] bg-white p-5 shadow-sm">
        <div className="flex flex-wrap gap-2">
          {page.userCanModify ? <Button type="button" variant="outline" disabled={page.selectedList.length === 0} onClick={() => page.setWhatsappOpen(true)}><MessageCircle className="size-4" />إرسال واتساب ({page.selectedList.length})</Button> : null}
          <Button type="button" variant="outline" onClick={page.selectAllOnPage}><CheckSquare className="size-4" />تحديد الكل</Button>
          <Button type="button" variant="outline" onClick={page.clearSelection}><Square className="size-4" />إلغاء التحديد</Button>
          {page.userCanModify ? (
            <>
              <Button type="button" variant="outline" onClick={() => page.setCreateCircleOpen(true)}><PlusCircle className="size-4" />إنشاء حلقة</Button>
              <Button type="button" variant="outline" disabled={page.selectedList.length === 0} onClick={() => page.setTransferOpen(true)}><Shuffle className="size-4" />نقل الطلاب ({page.selectedList.length})</Button>
            </>
          ) : null}
        </div>
        {page.selectedList.length > 0 ? (
          <p className="mt-3 rounded-lg bg-[var(--color-primary-muted)] px-3 py-2 text-sm text-[var(--color-primary-dark)]">
            <Users className="mr-1 inline size-4" />
            تم تحديد {page.selectedList.length} طالب
          </p>
        ) : null}
      </section>

      <HomeTable
        items={page.list?.items ?? []}
        selectedIds={page.selectedIds}
        pageNumber={page.list?.pageNumber ?? page.pageNumber}
        pageSize={page.list?.pageSize ?? page.pageSize}
        totalCount={page.list?.totalCount ?? 0}
        totalPages={page.list?.totalPages ?? 0}
        canModify={page.userCanModify}
        onToggleStudent={page.toggleStudent}
        onDelete={page.setDeleteTarget}
        onShowTests={(id, name) => page.setTestsTarget({ id, name })}
        onShowReviews={(id, name) => page.setReviewsTarget({ id, name })}
        onPageChange={(nextPage) => {
          page.setPageNumber(nextPage)
          page.setAppliedFilters(buildAppliedOthaiminCenterFilters(page.filterForm, nextPage, page.pageSize, page.circleQuery))
        }}
        onPageSizeChange={(size) => {
          page.setPageSize(size)
          page.setPageNumber(1)
          page.setAppliedFilters(buildAppliedOthaiminCenterFilters(page.filterForm, 1, size, page.circleQuery))
        }}
      />

      <HomeWhatsappDialog open={page.whatsappOpen} selectedCount={page.selectedList.length} isPending={page.whatsappMutation.isPending} canModify={page.userCanModify} onOpenChange={page.setWhatsappOpen} onSubmit={handleWhatsapp} />
      <CreateCircleDialog open={page.createCircleOpen} selectedCount={page.selectedList.length} teachers={page.filterOptionsQuery.data?.teachers ?? []} isPending={page.createCircleMutation.isPending} canModify={page.userCanModify} onOpenChange={page.setCreateCircleOpen} onSubmit={handleCreateCircle} />
      <TransferStudentsDialog open={page.transferOpen} selectedStudents={page.selectedList} circles={page.filterOptionsQuery.data?.transferCircles ?? []} isPending={page.transferMutation.isPending} canModify={page.userCanModify} onOpenChange={page.setTransferOpen} onSubmit={handleTransfer} />
      <OthaiminCenterStudentTestsDialog open={page.testsTarget != null} studentId={page.testsTarget?.id ?? null} studentName={page.testsTarget?.name ?? ''} onOpenChange={(open) => !open && page.setTestsTarget(null)} />
      <OthaiminCenterStudentReviewsDialog open={page.reviewsTarget != null} studentId={page.reviewsTarget?.id ?? null} studentName={page.reviewsTarget?.name ?? ''} onOpenChange={(open) => !open && page.setReviewsTarget(null)} />
      <DeleteStudentDialog open={page.deleteTarget != null} studentId={page.deleteTarget} isPending={page.deleteMutation.isPending} onOpenChange={(open) => !open && page.setDeleteTarget(null)} onConfirm={() => page.deleteTarget && page.deleteMutation.mutate(page.deleteTarget, { onSettled: () => page.setDeleteTarget(null) })} />
    </div>
  )
}
