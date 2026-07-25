import { useMemo, useState } from 'react'

import { PageHeader } from '@/components/shared/PageHeader'

import { Alert } from '@/components/ui/alert'

import { Skeleton } from '@/components/ui/skeleton'

import { isAdmin } from '@/lib/authStorage'

import { useExpensives } from '@/hooks/useExpensives'

import { DeleteExpensiveDialog } from '@/pages/expensives/dialogs/DeleteExpensiveDialog'

import { ExpensivesFilters } from '@/pages/expensives/ExpensivesFilters'

import { ExpensivesSummary } from '@/pages/expensives/ExpensivesSummary'

import { ExpensivesTable } from '@/pages/expensives/ExpensivesTable'

import { getExpensivesEmptyMessage } from '@/types/expensives'



export function ExpensivesPage() {

  const { listQuery, summaryQuery, deleteMutation, exportMutation } = useExpensives()

  const [search, setSearch] = useState('')

  const [deleteId, setDeleteId] = useState<number | null>(null)



  const items = listQuery.data ?? []

  const filteredItems = useMemo(() => {

    const term = search.trim().toLowerCase()

    if (!term) return items



    return items.filter(

      (item) =>

        item.reason.toLowerCase().includes(term) ||

        item.supplier.toLowerCase().includes(term) ||

        String(item.totalAmount).includes(term) ||

        item.createdBy.toLowerCase().includes(term),

    )

  }, [items, search])



  if (!isAdmin()) {

    return <Alert variant="destructive">غير مصرح بالوصول إلى هذه الصفحة.</Alert>

  }



  if (listQuery.isLoading || summaryQuery.isLoading) {

    return (

      <div className="space-y-4">

        <Skeleton className="h-24 w-full" />

        <Skeleton className="h-32 w-full" />

        <Skeleton className="h-64 w-full" />

      </div>

    )

  }



  if (listQuery.isError || summaryQuery.isError) {

    return <Alert variant="destructive">تعذر تحميل قائمة المصروفات.</Alert>

  }



  const handleDeleteConfirm = () => {

    if (deleteId === null) return

    deleteMutation.mutate(deleteId, {

      onSettled: () => setDeleteId(null),

    })

  }



  const hasSearch = search.trim().length > 0



  return (

    <div>

      <PageHeader title="قائمة المصروفات" description="إدارة المصروفات والمدفوعات" />



      {summaryQuery.data && <ExpensivesSummary summary={summaryQuery.data} />}



      <ExpensivesFilters search={search} onSearchChange={setSearch} />



      {deleteMutation.isError && (

        <Alert variant="destructive" className="mb-4">

          تعذر حذف المصروف. يرجى المحاولة مرة أخرى.

        </Alert>

      )}



      {exportMutation.isError && (

        <Alert variant="destructive" className="mb-4">

          تعذر تصدير المصروفات. يرجى المحاولة مرة أخرى.

        </Alert>

      )}



      <ExpensivesTable

        items={filteredItems}

        emptyMessage={getExpensivesEmptyMessage(hasSearch)}

        onDelete={setDeleteId}

        onExport={() => exportMutation.mutate()}

        isExporting={exportMutation.isPending}

      />



      <DeleteExpensiveDialog

        open={deleteId !== null}

        onOpenChange={(open) => !open && setDeleteId(null)}

        onConfirm={handleDeleteConfirm}

        isPending={deleteMutation.isPending}

      />

    </div>

  )

}

