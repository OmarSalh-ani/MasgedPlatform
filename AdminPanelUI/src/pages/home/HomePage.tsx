import { BarChart3, CheckSquare, MessageCircle, PlusCircle, Shuffle, Square, Users } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { useHome, useHomeFilterOptions, useHomeRegistrationSettings } from '@/hooks/useHome'
import { canModify, getAdminSession } from '@/lib/authStorage'
import { getHomeCircleTitle } from '@/services/homeService'
import { CreateCircleDialog } from '@/pages/home/dialogs/CreateCircleDialog'
import { DeleteStudentDialog } from '@/pages/home/dialogs/DeleteStudentDialog'
import { HomeWhatsappDialog } from '@/pages/home/dialogs/HomeWhatsappDialog'
import { StudentReviewsDialog } from '@/pages/home/dialogs/StudentReviewsDialog'
import { StudentTestsDialog } from '@/pages/home/dialogs/StudentTestsDialog'
import { TransferStudentsDialog } from '@/pages/home/dialogs/TransferStudentsDialog'
import { HomeFilters } from '@/pages/home/HomeFilters'
import { HomeTable } from '@/pages/home/HomeTable'
import {
  buildAppliedHomeFilters,
  clearSelectedStudentsStorage,
  getDefaultHomeFilterForm,
  loadSelectedStudents,
  saveSelectedStudents,
  type HomeFilterFormState,
} from '@/pages/home/homeUtils'
import { HOME_PAGE_SIZE } from '@/types/home'
import type { HomeFilters as HomeFiltersType, SelectedHomeStudent } from '@/types/home'

export function HomePage() {
  const [searchParams] = useSearchParams()
  const circleQuery = parseOptionalInt(searchParams.get('circle'))
  const session = getAdminSession()
  const userCanModify = canModify()
  const isGirlTeacher = session?.isGirlTeacher ?? false

  const [filterForm, setFilterForm] = useState<HomeFilterFormState>(() => getDefaultHomeFilterForm(circleQuery))
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(HOME_PAGE_SIZE)
  const [appliedFilters, setAppliedFilters] = useState<HomeFiltersType | null>(null)
  const [pageTitle, setPageTitle] = useState('قائمة الطلاب')
  const [selectedStudents, setSelectedStudents] = useState<Map<number, SelectedHomeStudent>>(() => loadSelectedStudents())
  const [deleteTarget, setDeleteTarget] = useState<number | null>(null)
  const [testsTarget, setTestsTarget] = useState<{ id: number; name: string } | null>(null)
  const [reviewsTarget, setReviewsTarget] = useState<{ id: number; name: string } | null>(null)
  const [whatsappOpen, setWhatsappOpen] = useState(false)
  const [createCircleOpen, setCreateCircleOpen] = useState(false)
  const [transferOpen, setTransferOpen] = useState(false)
  const [actionMessage, setActionMessage] = useState<string | null>(null)

  const filterOptionsQuery = useHomeFilterOptions()
  const registrationQuery = useHomeRegistrationSettings()
  const {
    listQuery,
    exportMutation,
    deleteMutation,
    whatsappMutation,
    transferMutation,
    createCircleMutation,
    updateRegistrationMutation,
  } = useHome(appliedFilters)

  useEffect(() => {
    setAppliedFilters(buildAppliedHomeFilters(filterForm, 1, pageSize, circleQuery))
    if (circleQuery) {
      getHomeCircleTitle(circleQuery).then(setPageTitle).catch(() => setPageTitle('قائمة الطلاب'))
    }
  }, [])

  const selectedList = useMemo(() => [...selectedStudents.values()], [selectedStudents])
  const selectedIds = useMemo(() => new Set(selectedList.map((item) => item.id)), [selectedList])
  const list = listQuery.data

  const persistSelection = (next: Map<number, SelectedHomeStudent>) => {
    setSelectedStudents(next)
    saveSelectedStudents(next)
  }

  const applyFilters = (page = 1, nextPageSize = pageSize) => {
    setPageNumber(page)
    setAppliedFilters(buildAppliedHomeFilters(filterForm, page, nextPageSize, circleQuery))
  }

  const handleClearFilters = () => {
    const defaults = getDefaultHomeFilterForm(circleQuery)
    setFilterForm(defaults)
    setPageNumber(1)
    clearSelectedStudentsStorage()
    setSelectedStudents(new Map())
    setAppliedFilters(buildAppliedHomeFilters(defaults, 1, pageSize, circleQuery))
  }

  const toggleStudent = (student: SelectedHomeStudent) => {
    const next = new Map(selectedStudents)
    if (next.has(student.id)) next.delete(student.id)
    else next.set(student.id, student)
    persistSelection(next)
  }

  const selectAllOnPage = () => {
    const next = new Map(selectedStudents)
    for (const item of list?.items ?? []) {
      next.set(item.id, {
        id: item.id,
        studentName: item.studentName,
        fatherName: item.fatherName,
        fatherPhone: item.fatherPhone,
        circleName: item.circleName,
      })
    }
    persistSelection(next)
  }

  const clearSelection = () => {
    clearSelectedStudentsStorage()
    setSelectedStudents(new Map())
  }

  const handleWhatsapp = (payload: { message: string; image?: File | null }) => {
    whatsappMutation.mutate(
      { studentIds: selectedList.map((item) => item.id), message: payload.message, image: payload.image },
      {
        onSuccess: (message) => {
          setActionMessage(message)
          setWhatsappOpen(false)
        },
      },
    )
  }

  const handleTransfer = (circleId: number) => {
    transferMutation.mutate(
      { studentIds: selectedList.map((item) => item.id), circleId },
      {
        onSuccess: () => {
          setTransferOpen(false)
          clearSelection()
          applyFilters(pageNumber)
        },
      },
    )
  }

  const handleCreateCircle = (payload: { circleName: string; teacherId: number }) => {
    createCircleMutation.mutate(
      {
        circleName: payload.circleName,
        teacherId: payload.teacherId,
        studentIds: selectedList.map((item) => item.id),
      },
      {
        onSuccess: () => {
          setCreateCircleOpen(false)
          clearSelection()
          applyFilters(1)
        },
      },
    )
  }

  const handleRegistrationToggle = (forGirl: boolean, enabled: boolean) => {
    updateRegistrationMutation.mutate({ forGirl, enabled })
  }

  if (listQuery.isLoading && !list) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <PageHeader
        title={pageTitle}
        description="إدارة الطلاب والمتابعة"
        className="mb-0"
      >
        {registrationQuery.data?.showControls ? (
          <div className="mt-5 space-y-3 rounded-lg bg-white/10 p-4 text-right">
            <RegistrationToggle
              label="تسجيل الرجال"
              enabled={registrationQuery.data.menEnabled}
              disabled={!userCanModify || updateRegistrationMutation.isPending}
              onChange={(enabled) => handleRegistrationToggle(false, enabled)}
            />
            <RegistrationToggle
              label="تسجيل النساء"
              enabled={registrationQuery.data.womenEnabled}
              disabled={!userCanModify || updateRegistrationMutation.isPending}
              onChange={(enabled) => handleRegistrationToggle(true, enabled)}
            />
          </div>
        ) : null}
      </PageHeader>

      <div className="text-center">
        <Link to="/statistics" className="inline-flex items-center gap-2 rounded-full bg-[var(--color-primary)] px-6 py-3 text-white hover:opacity-90">
          <BarChart3 className="size-5" />
          عرض الإحصائيات
        </Link>
      </div>

      {actionMessage ? <Alert>{actionMessage}</Alert> : null}
      {listQuery.isError ? <Alert variant="destructive">تعذر تحميل قائمة الطلاب</Alert> : null}

      <HomeFilters
        form={filterForm}
        options={filterOptionsQuery.data}
        isGirlTeacher={isGirlTeacher}
        isExporting={exportMutation.isPending}
        onChange={setFilterForm}
        onApply={() => applyFilters(1)}
        onClear={handleClearFilters}
        onExport={() => exportMutation.mutate()}
      />

      <section className="rounded-xl border bg-white p-5 shadow-sm">
        <div className="flex flex-wrap gap-2">
          {userCanModify ? (
            <Button type="button" variant="outline" disabled={selectedList.length === 0} onClick={() => setWhatsappOpen(true)}>
              <MessageCircle className="size-4" />
              إرسال واتساب ({selectedList.length})
            </Button>
          ) : null}
          <Button type="button" variant="outline" onClick={selectAllOnPage}><CheckSquare className="size-4" />تحديد الكل</Button>
          <Button type="button" variant="outline" onClick={clearSelection}><Square className="size-4" />إلغاء التحديد</Button>
          {userCanModify ? (
            <>
              <Button type="button" variant="outline" onClick={() => setCreateCircleOpen(true)}><PlusCircle className="size-4" />إنشاء حلقة</Button>
              <Button type="button" variant="outline" disabled={selectedList.length === 0} onClick={() => setTransferOpen(true)}>
                <Shuffle className="size-4" />
                نقل الطلاب ({selectedList.length})
              </Button>
            </>
          ) : null}
        </div>
        {selectedList.length > 0 ? (
          <p className="mt-3 rounded-lg bg-sky-50 px-3 py-2 text-sm text-sky-800">
            <Users className="mr-1 inline size-4" />
            تم تحديد {selectedList.length} طالب
          </p>
        ) : null}
      </section>

      <HomeTable
        items={list?.items ?? []}
        selectedIds={selectedIds}
        pageNumber={list?.pageNumber ?? pageNumber}
        pageSize={list?.pageSize ?? pageSize}
        totalCount={list?.totalCount ?? 0}
        totalPages={list?.totalPages ?? 0}
        canModify={userCanModify}
        onToggleStudent={toggleStudent}
        onDelete={setDeleteTarget}
        onShowTests={(id, name) => setTestsTarget({ id, name })}
        onShowReviews={(id, name) => setReviewsTarget({ id, name })}
        onPageChange={(page) => {
          setPageNumber(page)
          setAppliedFilters(buildAppliedHomeFilters(filterForm, page, pageSize, circleQuery))
        }}
        onPageSizeChange={(size) => {
          setPageSize(size)
          setPageNumber(1)
          setAppliedFilters(buildAppliedHomeFilters(filterForm, 1, size, circleQuery))
        }}
      />

      <HomeWhatsappDialog open={whatsappOpen} selectedCount={selectedList.length} isPending={whatsappMutation.isPending} canModify={userCanModify} onOpenChange={setWhatsappOpen} onSubmit={handleWhatsapp} />
      <CreateCircleDialog open={createCircleOpen} selectedCount={selectedList.length} teachers={filterOptionsQuery.data?.teachers ?? []} isPending={createCircleMutation.isPending} canModify={userCanModify} onOpenChange={setCreateCircleOpen} onSubmit={handleCreateCircle} />
      <TransferStudentsDialog open={transferOpen} selectedStudents={selectedList} circles={filterOptionsQuery.data?.transferCircles ?? []} isPending={transferMutation.isPending} canModify={userCanModify} onOpenChange={setTransferOpen} onSubmit={handleTransfer} />
      <StudentTestsDialog open={testsTarget != null} studentId={testsTarget?.id ?? null} studentName={testsTarget?.name ?? ''} onOpenChange={(open) => !open && setTestsTarget(null)} />
      <StudentReviewsDialog open={reviewsTarget != null} studentId={reviewsTarget?.id ?? null} studentName={reviewsTarget?.name ?? ''} onOpenChange={(open) => !open && setReviewsTarget(null)} />
      <DeleteStudentDialog open={deleteTarget != null} studentId={deleteTarget} isPending={deleteMutation.isPending} onOpenChange={(open) => !open && setDeleteTarget(null)} onConfirm={() => deleteTarget && deleteMutation.mutate(deleteTarget, { onSettled: () => setDeleteTarget(null) })} />
    </div>
  )
}

function RegistrationToggle({
  label,
  enabled,
  disabled,
  onChange,
}: {
  label: string
  enabled: boolean
  disabled: boolean
  onChange: (enabled: boolean) => void
}) {
  return (
    <label className="flex items-center justify-between gap-4">
      <span className="font-semibold">{label}: {enabled ? 'مفعل' : 'معطل'}</span>
      <input type="checkbox" checked={enabled} disabled={disabled} onChange={(e) => onChange(e.target.checked)} />
    </label>
  )
}

function parseOptionalInt(value: string | null): number | undefined {
  if (!value) return undefined
  const parsed = Number(value)
  return Number.isNaN(parsed) ? undefined : parsed
}
