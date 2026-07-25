import { useEffect, useState } from 'react'
import { Banknote } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Skeleton } from '@/components/ui/skeleton'
import { canModify, isAdmin } from '@/lib/authStorage'
import { useTeacherSalaries } from '@/hooks/useTeacherSalaries'
import { DeleteTeacherSalaryDialog } from '@/pages/teacher-salaries/dialogs/DeleteTeacherSalaryDialog'
import { TeacherSalariesFilters } from '@/pages/teacher-salaries/TeacherSalariesFilters'
import { TeacherSalaryCard } from '@/pages/teacher-salaries/TeacherSalaryCard'

export function TeacherSalariesPage() {
  const userCanModify = canModify()
  const [month, setMonth] = useState(0)
  const [year, setYear] = useState(0)
  const [teacherId, setTeacherId] = useState(0)
  const [appliedFilters, setAppliedFilters] = useState({ month: 0, year: 0, teacherId: 0 })
  const [selectedIds, setSelectedIds] = useState<number[]>([])
  const [deleteTarget, setDeleteTarget] = useState<{ id: number; name: string } | null>(null)

  const { filterOptionsQuery, listQuery, deleteMutation, autoCalculateMutation, payMutation } =
    useTeacherSalaries(appliedFilters)

  useEffect(() => {
    if (filterOptionsQuery.data) {
      setMonth(filterOptionsQuery.data.defaultMonth)
      setYear(filterOptionsQuery.data.defaultYear)
      setAppliedFilters({
        month: filterOptionsQuery.data.defaultMonth,
        year: filterOptionsQuery.data.defaultYear,
        teacherId: 0,
      })
    }
  }, [filterOptionsQuery.data])

  if (!isAdmin()) {
    return <Alert variant="destructive">غير مصرح بالوصول إلى هذه الصفحة.</Alert>
  }

  const handleFilter = () => {
    setAppliedFilters({ month, year, teacherId })
    setSelectedIds([])
  }

  const handleAutoCalculate = () => {
    if (month === 0 || year === 0) {
      window.alert('يرجى اختيار شهر وسنة محددين')
      return
    }
    if (!window.confirm('هل تريد حساب رواتب جميع المعلمين تلقائياً للشهر المحدد؟')) return

    autoCalculateMutation.mutate(
      { month, year },
      {
        onSuccess: (result) => {
          window.alert(`تم حساب رواتب ${result.successCount} معلم بنجاح`)
          if (result.errorCount > 0) {
            window.alert(`حدثت أخطاء في ${result.errorCount} معلم`)
          }
        },
      },
    )
  }

  const handlePay = () => {
    if (selectedIds.length === 0) {
      window.alert('يرجى تحديد راتب واحد على الأقل')
      return
    }
    if (
      !window.confirm(
        'هل أنت متأكد من صرف الرواتب المحددة؟ سيتم إضافة المصروفات إلى سجل المصروفات.',
      )
    ) {
      return
    }

    payMutation.mutate(selectedIds, {
      onSuccess: (result) => {
        window.alert(`تم صرف الرواتب بنجاح! ${result.message}`)
        setSelectedIds([])
      },
    })
  }

  const toggleSelectAll = (checked: boolean) => {
    const items = listQuery.data?.items ?? []
    setSelectedIds(checked ? items.map((item) => item.id) : [])
  }

  if (filterOptionsQuery.isLoading || listQuery.isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (filterOptionsQuery.isError || listQuery.isError) {
    return <Alert variant="destructive">تعذر تحميل رواتب المعلمين.</Alert>
  }

  const items = listQuery.data?.items ?? []
  const options = filterOptionsQuery.data!
  const allSelected = items.length > 0 && selectedIds.length === items.length

  return (
    <div>
      <PageHeader
        title="إدارة رواتب المعلمين"
        description="تتبع وحساب رواتب المعلمين بناءً على الحضور"
      />

      <TeacherSalariesFilters
        options={options}
        month={month}
        year={year}
        teacherId={teacherId}
        onMonthChange={setMonth}
        onYearChange={setYear}
        onTeacherChange={setTeacherId}
        onFilter={handleFilter}
        onAutoCalculate={handleAutoCalculate}
        canModify={userCanModify}
        isAutoCalculating={autoCalculateMutation.isPending}
      />

      {userCanModify && selectedIds.length > 0 && (
        <div className="mb-4 flex justify-end">
          <Button
            type="button"
            className="rounded-full bg-emerald-600 hover:bg-emerald-700"
            disabled={payMutation.isPending}
            onClick={handlePay}
          >
            <Banknote className="size-4" />
            صرف
          </Button>
        </div>
      )}

      {userCanModify && items.length > 0 && (
        <div className="mb-4 flex items-center gap-2">
          <input
            id="select-all-salaries"
            type="checkbox"
            className="size-4 accent-[var(--color-primary)]"
            checked={allSelected}
            onChange={(e) => toggleSelectAll(e.target.checked)}
          />
          <Label htmlFor="select-all-salaries" className="cursor-pointer font-semibold">
            تحديد الكل
          </Label>
        </div>
      )}

      {(deleteMutation.isError || payMutation.isError || autoCalculateMutation.isError) && (
        <Alert variant="destructive" className="mb-4">
          تعذر إتمام العملية. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {items.length > 0 ? (
        <div className="grid gap-5 md:grid-cols-2 xl:grid-cols-3">
          {items.map((item) => (
            <TeacherSalaryCard
              key={item.id}
              item={item}
              selected={selectedIds.includes(item.id)}
              canModify={userCanModify}
              onSelectChange={(checked) => {
                setSelectedIds((prev) =>
                  checked ? [...prev, item.id] : prev.filter((id) => id !== item.id),
                )
              }}
              onDelete={() => setDeleteTarget({ id: item.id, name: item.teacherName })}
            />
          ))}
        </div>
      ) : (
        <div className="rounded-xl bg-white py-16 text-center text-slate-500 shadow-md">
          <Banknote className="mx-auto mb-4 size-12 opacity-40" />
          <p className="text-lg font-semibold">لا توجد رواتب مسجلة</p>
          <p className="mt-1 text-sm opacity-70">اضغط على &quot;إضافة راتب جديد&quot; لبدء إضافة الرواتب</p>
        </div>
      )}

      <DeleteTeacherSalaryDialog
        open={deleteTarget !== null}
        teacherName={deleteTarget?.name ?? ''}
        onOpenChange={(open) => !open && setDeleteTarget(null)}
        onConfirm={() => {
          if (!deleteTarget) return
          deleteMutation.mutate(deleteTarget.id, { onSettled: () => setDeleteTarget(null) })
        }}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
