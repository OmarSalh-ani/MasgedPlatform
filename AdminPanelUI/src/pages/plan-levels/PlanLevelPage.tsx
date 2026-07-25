import { useState } from 'react'
import { Layers } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { usePlanLevels } from '@/hooks/usePlanLevels'
import { DeletePlanLevelDialog } from '@/pages/plan-levels/dialogs/DeletePlanLevelDialog'
import { PlanLevelForm } from '@/pages/plan-levels/PlanLevelForm'
import { PlanLevelTable } from '@/pages/plan-levels/PlanLevelTable'
import type { SavePlanLevelFormValues } from '@/pages/plan-levels/savePlanLevelSchema'
import type { PlanUnitTypeValue } from '@/types/planLevel'

export function PlanLevelPage() {
  const { query, saveMutation, deleteMutation, getSaveErrorMessage, getDeleteErrorMessage } =
    usePlanLevels()
  const [deleteId, setDeleteId] = useState<number | null>(null)
  const [editingId, setEditingId] = useState<number | null>(null)

  const handleSave = (values: SavePlanLevelFormValues) => {
    saveMutation.mutate(
      {
        id: editingId ?? undefined,
        payload: {
          levelName: values.levelName.trim(),
          unitType: values.unitType as PlanUnitTypeValue,
          quantity: values.quantity,
        },
      },
      {
        onSuccess: () => {
          setEditingId(null)
        },
      },
    )
  }

  const handleDeleteConfirm = () => {
    if (deleteId === null) return
    deleteMutation.mutate(deleteId, {
      onSettled: () => setDeleteId(null),
    })
  }

  const items = query.data ?? []

  return (
    <div className="mx-auto max-w-7xl space-y-8">
      <PageHeader
        icon={Layers}
        title="مستويات الخطة"
        description="إدارة مستويات الخطة وتحديد القدرة لكل مستوى"
        className="mb-0"
      />

      {query.isLoading && <PlanLevelPageSkeleton />}

      {query.isError && (
        <Alert variant="destructive">
          تعذر تحميل قائمة مستويات الخطة. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      {query.data && (
        <div className="space-y-6">
          <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
            {saveMutation.isError && (
              <Alert variant="destructive" className="mb-4">
                {getSaveErrorMessage(saveMutation.error)}
              </Alert>
            )}
            <PlanLevelForm
              editingId={editingId}
              isPending={saveMutation.isPending}
              onSubmit={handleSave}
              onCancelEdit={() => setEditingId(null)}
            />
          </div>

          {deleteMutation.isError && (
            <Alert variant="destructive">
              {getDeleteErrorMessage(deleteMutation.error)}
            </Alert>
          )}

          <PlanLevelTable items={items} onDelete={setDeleteId} />
        </div>
      )}

      <DeletePlanLevelDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}

function PlanLevelPageSkeleton() {
  return (
    <div className="space-y-6">
      <Skeleton className="h-40 rounded-2xl" />
      <Skeleton className="h-64 rounded-2xl" />
    </div>
  )
}
