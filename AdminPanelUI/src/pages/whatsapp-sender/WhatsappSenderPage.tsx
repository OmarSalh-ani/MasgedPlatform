import { CheckSquare, MessageCircle, Square } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { canModify } from '@/lib/authStorage'
import {
  useWhatsappSender,
  useWhatsappSenderFilterOptions,
  useWhatsappSenderFormOptions,
} from '@/hooks/useWhatsappSender'
import { WhatsappSenderDialog } from '@/pages/whatsapp-sender/dialogs/WhatsappSenderDialog'
import { WhatsappSenderFilters as WhatsappSenderFiltersPanel } from '@/pages/whatsapp-sender/WhatsappSenderFilters'
import { WhatsappSenderTable } from '@/pages/whatsapp-sender/WhatsappSenderTable'
import {
  buildWhatsappSenderFilters,
  clearWhatsappSenderSelectionStorage,
  getDefaultWhatsappSenderFilterForm,
  loadWhatsappSenderSelection,
  partitionByArabicPhone,
  saveWhatsappSenderSelection,
} from '@/pages/whatsapp-sender/whatsappSenderUtils'
import {
  WHATSAPP_SENDER_PAGE_SIZE,
  type SelectedWhatsappSenderStudent,
  type WhatsappSenderFilterForm,
  type WhatsappSenderFilters,
} from '@/types/whatsappSender'

export function WhatsappSenderPage() {
  const userCanModify = canModify()
  const [filterForm, setFilterForm] = useState<WhatsappSenderFilterForm>(getDefaultWhatsappSenderFilterForm)
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(WHATSAPP_SENDER_PAGE_SIZE)
  const [appliedFilters, setAppliedFilters] = useState<WhatsappSenderFilters | null>(null)
  const [selectedStudents, setSelectedStudents] = useState<Map<number, SelectedWhatsappSenderStudent>>(
    () => loadWhatsappSenderSelection(),
  )
  const [whatsappOpen, setWhatsappOpen] = useState(false)
  const [actionMessage, setActionMessage] = useState<string | null>(null)

  const filterOptionsQuery = useWhatsappSenderFilterOptions()
  const formOptionsQuery = useWhatsappSenderFormOptions()
  const { listQuery, whatsappMutation } = useWhatsappSender(appliedFilters)

  useEffect(() => {
    setAppliedFilters(buildWhatsappSenderFilters(filterForm, 1, pageSize))
  }, [])

  const selectedList = useMemo(() => [...selectedStudents.values()], [selectedStudents])
  const selectedIds = useMemo(() => new Set(selectedList.map((item) => item.id)), [selectedList])
  const list = listQuery.data

  const persistSelection = (next: Map<number, SelectedWhatsappSenderStudent>) => {
    setSelectedStudents(next)
    saveWhatsappSenderSelection(next)
  }

  const applyFilters = (page = 1, nextPageSize = pageSize) => {
    setPageNumber(page)
    setAppliedFilters(buildWhatsappSenderFilters(filterForm, page, nextPageSize))
  }

  const handleClearFilters = () => {
    const defaults = getDefaultWhatsappSenderFilterForm()
    setFilterForm(defaults)
    setPageNumber(1)
    clearWhatsappSenderSelectionStorage()
    setSelectedStudents(new Map())
    setAppliedFilters(buildWhatsappSenderFilters(defaults, 1, pageSize))
  }

  const toggleStudent = (student: SelectedWhatsappSenderStudent) => {
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
    clearWhatsappSenderSelectionStorage()
    setSelectedStudents(new Map())
  }

  const handleWhatsapp = (payload: { message: string; formId?: number | null; image?: File | null }) => {
    const { valid, skipped } = partitionByArabicPhone(selectedList)

    if (valid.length === 0) {
      setActionMessage(
        `لا يوجد أرقام إنجليزية صالحة للإرسال. تم تجاهل ${skipped.length} طالب لأن أرقامهم تحتوي على أرقام عربية.`,
      )
      setWhatsappOpen(false)
      return
    }

    whatsappMutation.mutate(
      {
        studentIds: valid.map((item) => item.id),
        message: payload.message,
        formId: payload.formId,
        image: payload.image,
      },
      {
        onSuccess: (message) => {
          const skippedNote = skipped.length > 0 ? ` (تم تجاهل ${skipped.length} رقم عربي)` : ''
          setActionMessage(`${message}${skippedNote}`)
          setWhatsappOpen(false)
        },
      },
    )
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
        title="رسائل الواتساب"
        description="إرسال رسائل واتساب"
        className="mb-0"
      />

      {actionMessage ? <Alert>{actionMessage}</Alert> : null}
      {listQuery.isError ? <Alert variant="destructive">تعذر تحميل قائمة الطلاب</Alert> : null}

      <WhatsappSenderFiltersPanel
        form={filterForm}
        options={filterOptionsQuery.data}
        onChange={setFilterForm}
        onApply={() => applyFilters(1)}
        onClear={handleClearFilters}
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
        </div>
        {selectedList.length > 0 ? (
          <p className="mt-3 rounded-lg bg-sky-50 px-3 py-2 text-sm text-sky-800">
            تم تحديد {selectedList.length} طالب من صفحات متعددة
          </p>
        ) : null}
      </section>

      <WhatsappSenderTable
        items={list?.items ?? []}
        selectedIds={selectedIds}
        pageNumber={list?.pageNumber ?? pageNumber}
        pageSize={list?.pageSize ?? pageSize}
        totalCount={list?.totalCount ?? 0}
        totalPages={list?.totalPages ?? 0}
        onToggleStudent={toggleStudent}
        onPageChange={(page) => {
          setPageNumber(page)
          setAppliedFilters(buildWhatsappSenderFilters(filterForm, page, pageSize))
        }}
        onPageSizeChange={(size) => {
          setPageSize(size)
          setPageNumber(1)
          setAppliedFilters(buildWhatsappSenderFilters(filterForm, 1, size))
        }}
      />

      <WhatsappSenderDialog
        open={whatsappOpen}
        selectedCount={selectedList.length}
        formOptions={formOptionsQuery.data ?? []}
        isPending={whatsappMutation.isPending}
        canModify={userCanModify}
        onOpenChange={setWhatsappOpen}
        onSubmit={handleWhatsapp}
      />
    </div>
  )
}
