import { useMemo, useState } from 'react'

import { useSearchParams } from 'react-router-dom'

import { PageHeader } from '@/components/shared/PageHeader'

import { Alert } from '@/components/ui/alert'

import { Skeleton } from '@/components/ui/skeleton'

import { getAdminSession } from '@/lib/authStorage'

import { useCircles } from '@/hooks/useCircles'

import { CirclesFilters } from '@/pages/circles/CirclesFilters'

import { CirclesTable } from '@/pages/circles/CirclesTable'

import { DeleteCircleDialog } from '@/pages/circles/dialogs/DeleteCircleDialog'

import { getCirclesEmptyMessage } from '@/types/circle'



export function CirclesPage() {

  const [searchParams] = useSearchParams()

  const teacherParam = searchParams.get('teacher')

  const teacherId = teacherParam ? Number(teacherParam) : undefined

  const { query, deleteMutation, exportMutation } = useCircles(

    Number.isFinite(teacherId) ? teacherId : undefined,

  )

  const [search, setSearch] = useState('')

  const [deleteId, setDeleteId] = useState<number | null>(null)

  const canModify = getAdminSession()?.isViewOnly !== true



  const items = query.data ?? []

  const filteredItems = useMemo(() => {

    const term = search.trim().toLowerCase()

    if (!term) return items



    return items.filter(

      (item) =>

        item.name.toLowerCase().includes(term) ||

        item.teacherName.toLowerCase().includes(term),

    )

  }, [items, search])



  const handleDeleteConfirm = () => {

    if (deleteId === null) return

    deleteMutation.mutate(deleteId, {

      onSettled: () => setDeleteId(null),

    })

  }



  if (query.isLoading) {

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

        تعذر تحميل قائمة الحلقات. يرجى المحاولة مرة أخرى.

      </Alert>

    )

  }



  const hasSearch = search.trim().length > 0



  return (

    <div>

      <PageHeader title="قائمة الحلقات" description="إدارة حلقات القرآن الكريم" />



      <CirclesFilters

        search={search}

        onSearchChange={setSearch}

        canModify={canModify}

      />



      {deleteMutation.isError && (

        <Alert variant="destructive" className="mb-4">

          تعذر حذف الحلقة. يرجى المحاولة مرة أخرى.

        </Alert>

      )}



      {exportMutation.isError && (

        <Alert variant="destructive" className="mb-4">

          تعذر تصدير الحلقات. يرجى المحاولة مرة أخرى.

        </Alert>

      )}



      <CirclesTable

        items={filteredItems}

        emptyMessage={getCirclesEmptyMessage(hasSearch)}

        canModify={canModify}

        onDelete={setDeleteId}

        onExport={() => exportMutation.mutate()}

        isExporting={exportMutation.isPending}

      />



      <DeleteCircleDialog

        open={deleteId !== null}

        onOpenChange={(open) => !open && setDeleteId(null)}

        onConfirm={handleDeleteConfirm}

        isPending={deleteMutation.isPending}

      />

    </div>

  )

}

