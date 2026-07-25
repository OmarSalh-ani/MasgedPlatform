import { useEffect, useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import {
  getEditPrefill,
  resolveStudentPlan,
  useStudentPlanDetail,
  useStudentPlanFormData,
  useStudentPlanMutations,
} from '@/hooks/useStudentPlan'
import { isValidPlanRow } from '@/services/studentPlanService'
import { DeletePlanItemDialog } from '@/pages/student-plans/dialogs/DeletePlanItemDialog'
import { StudentPlanDateFields, StudentPlanNewPlanPanel } from '@/pages/student-plans/StudentPlanFormSections'
import { itemsToEditRows, StudentPlanItemsTable } from '@/pages/student-plans/StudentPlanItemsTable'
import { emptyPlanRow, StudentPlanPlanRow } from '@/pages/student-plans/StudentPlanPlanRow'
import { StudentPlanStudentPicker } from '@/pages/student-plans/StudentPlanStudentPicker'
import type { EditPlanRowInput, PlanRowInput } from '@/types/studentPlan'
import { todayIsoDate } from '@/types/studentPlan'

export function StudentPlanPage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const studentIdParam = searchParams.get('studentId')
  const planIdParam = searchParams.get('planId')
  const newPlan = searchParams.get('newPlan') === '1'
  const editKey = searchParams.get('edit') ?? undefined

  const studentId = studentIdParam ? Number(studentIdParam) : undefined
  const planId = planIdParam ? Number(planIdParam) : undefined

  const formQuery = useStudentPlanFormData()
  const detailQuery = useStudentPlanDetail(studentId, planId)
  const { createPlanMutation, saveMutation, updateItemMutation, deleteItemMutation } =
    useStudentPlanMutations(studentId, planId)

  const [resolving, setResolving] = useState(!!studentId && !planId && !newPlan)
  const [selectedStudentIds, setSelectedStudentIds] = useState<number[]>([])
  const [editMode, setEditMode] = useState(false)
  const [memorizationLevel, setMemorizationLevel] = useState('محدد الحفظ')
  const [planStartDate, setPlanStartDate] = useState(todayIsoDate())
  const [planEndDate, setPlanEndDate] = useState(todayIsoDate())
  const [newRows, setNewRows] = useState<PlanRowInput[]>([emptyPlanRow()])
  const [editRows, setEditRows] = useState<EditPlanRowInput[]>([])
  const [deleteKey, setDeleteKey] = useState<string | null>(null)
  const [saveMessage, setSaveMessage] = useState<string | null>(null)

  useEffect(() => {
    if (!studentId || planId || newPlan) {
      setResolving(false)
      return
    }

    let cancelled = false
    resolveStudentPlan(studentId, planId, editKey)
      .then((result) => {
        if (cancelled) return
        if (result.shouldCreateNew) {
          navigate(`/student-plans?studentId=${studentId}&newPlan=1`, { replace: true })
          return
        }
        if (result.planId) {
          const qs = new URLSearchParams({ studentId: String(studentId), planId: String(result.planId) })
          if (editKey) qs.set('edit', editKey)
          navigate(`/student-plans?${qs.toString()}`, { replace: true })
        }
      })
      .finally(() => {
        if (!cancelled) setResolving(false)
      })

    return () => {
      cancelled = true
    }
  }, [studentId, planId, newPlan, editKey, navigate])

  useEffect(() => {
    if (!detailQuery.data) return
    setMemorizationLevel(detailQuery.data.header.memorizationLevel)
    setPlanStartDate(detailQuery.data.header.planStartDate)
    setPlanEndDate(detailQuery.data.header.planEndDate)
    setEditRows(itemsToEditRows(detailQuery.data.items))
  }, [detailQuery.data])

  useEffect(() => {
    if (!editKey) return
    getEditPrefill(editKey).then((prefill) => {
      setMemorizationLevel(prefill.memorizationLevel)
      setPlanStartDate(prefill.planStartDate)
      setPlanEndDate(prefill.planEndDate)
      setNewRows([
        {
          surahId: prefill.surahId,
          fromAyahNumber: prefill.fromAyahNumber,
          toAyahNumber: prefill.toAyahNumber,
          planType: prefill.planType,
        },
      ])
    })
  }, [editKey])

  const formData = formQuery.data
  const detail = detailQuery.data
  const isViewMode = !!studentId
  const rowsDisabled = isViewMode && (newPlan || (!editMode && !editKey))
  const canSaveRows = !rowsDisabled && (formData?.canModify ?? false)
  const showNewRowsSection = !isViewMode || !!editKey || (isViewMode && editMode && !newPlan)

  const handleCreatePlan = (name: string, fromDate: string, toDate: string) => {
    if (!studentId || !name) {
      window.alert('يرجى إدخال اسم الخطة.')
      return
    }
    createPlanMutation.mutate(
      { studentId, name, fromDate, toDate },
      {
        onSuccess: (newPlanId) => {
          navigate(`/student-plans?studentId=${studentId}&planId=${newPlanId}`)
        },
      },
    )
  }

  const handleSave = () => {
    const validNewRows = newRows.filter(isValidPlanRow)
    const targetStudentIds = studentId ? [] : selectedStudentIds

    if (!studentId && targetStudentIds.length === 0) {
      window.alert('يرجى اختيار طالب واحد على الأقل.')
      return
    }

    if (editKey) {
      const row = newRows[0]
      if (!isValidPlanRow(row)) {
        window.alert('يرجى تعبئة السورة و من آية وإلى آية.')
        return
      }
      updateItemMutation.mutate(
        {
          editKey,
          memorizationLevel,
          planStartDate,
          planEndDate,
          surahId: row.surahId,
          fromAyahNumber: row.fromAyahNumber,
          toAyahNumber: row.toAyahNumber,
          planType: row.planType,
        },
        {
          onSuccess: () => {
            window.alert('تم تحديث الخطة بنجاح.')
            if (studentId && detail?.planId) {
              navigate(`/student-plans?studentId=${studentId}&planId=${detail.planId}`)
            }
          },
        },
      )
      return
    }

    saveMutation.mutate(
      {
        studentIds: targetStudentIds,
        studentId,
        planId,
        memorizationLevel,
        planStartDate,
        planEndDate,
        editMode: editMode && !!studentId,
        editRows: editMode ? editRows : [],
        newRows: validNewRows,
      },
      {
        onSuccess: () => {
          setSaveMessage('تم حفظ الخطة بنجاح.')
          window.alert('تم حفظ الخطة بنجاح.')
          setNewRows([emptyPlanRow()])
          if (studentId && planId) {
            navigate(`/student-plans?studentId=${studentId}&planId=${planId}`)
          }
        },
      },
    )
  }

  const handleDeleteConfirm = () => {
    if (!deleteKey) return
    deleteItemMutation.mutate(deleteKey, {
      onSettled: () => setDeleteKey(null),
    })
  }

  if (formQuery.isLoading || resolving) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    )
  }

  if (formQuery.isError) {
    return <Alert variant="destructive">تعذر تحميل بيانات الخطة.</Alert>
  }

  return (
    <div>
      <PageHeader title="خطة الطالب" />

      {isViewMode && newPlan && (
        <StudentPlanNewPlanPanel
          onCreate={handleCreatePlan}
          onCancel={() => navigate(`/student-plans?studentId=${studentId}`)}
          isPending={createPlanMutation.isPending}
        />
      )}

      {isViewMode && !newPlan && detail && (
        <div className="mb-4 space-y-3 rounded-xl bg-white p-4 shadow-md">
          <div className="flex flex-wrap items-center gap-3">
            <span className="text-sm font-semibold">الخطة:</span>
            <select
              className="max-w-xs rounded border px-3 py-2 text-sm"
              value={planId}
              onChange={(e) =>
                navigate(`/student-plans?studentId=${studentId}&planId=${e.target.value}`)
              }
            >
              {detail.plans.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.display}
                </option>
              ))}
            </select>
            <Link
              to={`/student-plans?studentId=${studentId}&newPlan=1`}
              className="text-sm text-[var(--color-primary)]"
            >
              + إضافة خطة جديدة
            </Link>
          </div>
          <p className="font-semibold">طالب: {detail.studentName}</p>
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" checked={editMode} onChange={(e) => setEditMode(e.target.checked)} />
            تفعيل التعديل
          </label>
        </div>
      )}

      <div className="rounded-xl bg-white p-6 shadow-md">
        <h3 className="mb-4 text-lg font-semibold">{editKey ? 'تعديل بند' : 'إضافة خطة'}</h3>

        {!isViewMode && formData && (
          <div className="mb-6">
            <StudentPlanStudentPicker
              circles={formData.circles}
              students={formData.students}
              selectedIds={selectedStudentIds}
              onChange={setSelectedStudentIds}
            />
          </div>
        )}

        {formData && (
          <StudentPlanDateFields
            memorizationLevel={memorizationLevel}
            planStartDate={planStartDate}
            planEndDate={planEndDate}
            levels={formData.memorizationLevels}
            onLevelChange={setMemorizationLevel}
            onStartChange={setPlanStartDate}
            onEndChange={setPlanEndDate}
          />
        )}

        {saveMessage && (
          <Alert className="mb-4">{saveMessage}</Alert>
        )}

        {(saveMutation.isError || updateItemMutation.isError) && (
          <Alert variant="destructive" className="mb-4">
            تعذر حفظ الخطة. يرجى المحاولة مرة أخرى.
          </Alert>
        )}

        <div className="mb-4">
          <span className="mb-2 block text-sm font-semibold">بنود الخطة</span>
          {detail && !editKey && (
            <StudentPlanItemsTable
              items={detail.items}
              editMode={editMode}
              surahs={formData?.surahs ?? []}
              planTypes={formData?.planTypes ?? []}
              editRows={editRows}
              onEditRowsChange={setEditRows}
              onEdit={(key) => {
                const qs = new URLSearchParams({
                  studentId: String(studentId),
                  edit: key,
                })
                if (planId) qs.set('planId', String(planId))
                navigate(`/student-plans?${qs.toString()}`)
              }}
              onDelete={setDeleteKey}
              canModify={detail.canModify}
            />
          )}
        </div>

        {showNewRowsSection && formData && (
          <>
            <div className="overflow-x-auto">
              <table className="w-full min-w-[600px] text-right text-sm">
                <thead>
                  <tr className="border-b bg-slate-50">
                    <th className="px-2 py-2">السورة</th>
                    <th className="px-2 py-2">من آية</th>
                    <th className="px-2 py-2">إلى آية</th>
                    <th className="px-2 py-2">نوع الخطة</th>
                    <th className="px-2 py-2" />
                  </tr>
                </thead>
                <tbody>
                  {newRows.map((row, index) => (
                    <StudentPlanPlanRow
                      key={index}
                      surahs={formData.surahs}
                      planTypes={formData.planTypes}
                      row={row}
                      onChange={(patch) =>
                        setNewRows((prev) => prev.map((r, i) => (i === index ? patch : r)))
                      }
                    />
                  ))}
                </tbody>
              </table>
            </div>

            <div className="mt-4 flex flex-wrap gap-2">
              <Button
                type="button"
                variant="outline"
                disabled={!canSaveRows}
                onClick={() => setNewRows((prev) => [...prev, emptyPlanRow()])}
              >
                + إضافة سطر
              </Button>
              <Button
                type="button"
                disabled={!canSaveRows || saveMutation.isPending || updateItemMutation.isPending}
                onClick={handleSave}
              >
                {saveMutation.isPending || updateItemMutation.isPending ? 'جاري الحفظ...' : 'حفظ الخطة'}
              </Button>
            </div>
          </>
        )}
      </div>

      <DeletePlanItemDialog
        open={deleteKey !== null}
        onOpenChange={(open) => !open && setDeleteKey(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteItemMutation.isPending}
      />
    </div>
  )
}
