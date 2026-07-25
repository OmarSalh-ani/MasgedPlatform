import { CheckSquare, Square } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { WhatsappSenderFilters as StudentFiltersPanel } from '@/pages/whatsapp-sender/WhatsappSenderFilters'
import { WhatsappSenderTable } from '@/pages/whatsapp-sender/WhatsappSenderTable'
import {
  usePushNotificationFilterOptions,
  usePushNotificationStudents,
} from '@/hooks/usePushNotification'
import {
  buildPushNotificationFilters,
  getDefaultPushNotificationFilterForm,
} from '@/pages/push-notifications/pushNotificationUtils'
import {
  PUSH_NOTIFICATION_PAGE_SIZE,
  type PushNotificationFilterForm,
  type PushNotificationStudentFilters,
  type SelectedPushNotificationStudent,
} from '@/types/pushNotification'

interface PushNotificationParentPickerProps {
  selectedStudentIds: number[]
  onChange: (ids: number[]) => void
}

export function PushNotificationParentPicker({
  selectedStudentIds,
  onChange,
}: PushNotificationParentPickerProps) {
  const [filterForm, setFilterForm] = useState<PushNotificationFilterForm>(
    getDefaultPushNotificationFilterForm,
  )
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(PUSH_NOTIFICATION_PAGE_SIZE)
  const [appliedFilters, setAppliedFilters] = useState<PushNotificationStudentFilters | null>(null)
  const [selectedStudents, setSelectedStudents] = useState<Map<number, SelectedPushNotificationStudent>>(
    new Map(),
  )

  const filterOptionsQuery = usePushNotificationFilterOptions()
  const listQuery = usePushNotificationStudents(appliedFilters)

  useEffect(() => {
    setAppliedFilters(buildPushNotificationFilters(filterForm, 1, pageSize))
  }, [])

  const selectedIds = useMemo(() => new Set(selectedStudentIds), [selectedStudentIds])
  const list = listQuery.data

  const updateSelection = (next: Map<number, SelectedPushNotificationStudent>) => {
    setSelectedStudents(next)
    onChange([...next.keys()])
  }

  const applyFilters = (page = 1, nextPageSize = pageSize) => {
    setPageNumber(page)
    setAppliedFilters(buildPushNotificationFilters(filterForm, page, nextPageSize))
  }

  const handleClearFilters = () => {
    const defaults = getDefaultPushNotificationFilterForm()
    setFilterForm(defaults)
    setPageNumber(1)
    setSelectedStudents(new Map())
    onChange([])
    setAppliedFilters(buildPushNotificationFilters(defaults, 1, pageSize))
  }

  const toggleStudent = (student: SelectedPushNotificationStudent) => {
    const next = new Map(selectedStudents)
    if (next.has(student.id)) next.delete(student.id)
    else next.set(student.id, student)
    updateSelection(next)
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
    updateSelection(next)
  }

  const clearSelection = () => updateSelection(new Map())

  if (listQuery.isLoading && !list) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  return (
    <div className="space-y-4">
      <StudentFiltersPanel
        form={filterForm}
        options={filterOptionsQuery.data}
        onChange={setFilterForm}
        onApply={() => applyFilters(1)}
        onClear={handleClearFilters}
      />

      <section className="rounded-xl border bg-white p-5 shadow-sm">
        <div className="flex flex-wrap gap-2">
          <Button type="button" variant="outline" onClick={selectAllOnPage}>
            <CheckSquare className="size-4" />
            تحديد الكل في الصفحة
          </Button>
          <Button type="button" variant="outline" onClick={clearSelection}>
            <Square className="size-4" />
            إلغاء التحديد
          </Button>
        </div>
        {selectedStudentIds.length > 0 ? (
          <p className="mt-3 rounded-lg bg-sky-50 px-3 py-2 text-sm text-sky-800">
            تم تحديد {selectedStudentIds.length} طالب
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
          setAppliedFilters(buildPushNotificationFilters(filterForm, page, pageSize))
        }}
        onPageSizeChange={(size) => {
          setPageSize(size)
          setPageNumber(1)
          setAppliedFilters(buildPushNotificationFilters(filterForm, 1, size))
        }}
      />
    </div>
  )
}
