import { useMemo, useState } from 'react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { useWhatsappPending } from '@/hooks/useWhatsappPending'
import { WhatsappPendingTable } from '@/pages/whatsapp-pending/WhatsappPendingTable'

export function WhatsappPendingPage() {
  const { query, deleteSelectedMutation, deleteAllMutation } = useWhatsappPending()
  const [selectedIds, setSelectedIds] = useState<Set<number>>(new Set())
  const [message, setMessage] = useState<string | null>(null)

  const items = query.data ?? []
  const countLabel = useMemo(() => `${items.length} رسالة`, [items.length])

  const toggleId = (id: number) => {
    const next = new Set(selectedIds)
    if (next.has(id)) next.delete(id)
    else next.add(id)
    setSelectedIds(next)
  }

  const toggleAll = (checked: boolean) => {
    setSelectedIds(checked ? new Set(items.map((item) => item.id)) : new Set())
  }

  const handleDeleteSelected = () => {
    if (selectedIds.size === 0) {
      window.alert('لم تحدد أي رسائل.')
      return
    }
    if (!window.confirm('هل تريد حذف الرسائل المحددة؟')) return
    deleteSelectedMutation.mutate([...selectedIds], {
      onSuccess: () => {
        setSelectedIds(new Set())
        setMessage('تم حذف الرسائل المحددة')
      },
    })
  }

  const handleDeleteAll = () => {
    if (!window.confirm('هل تريد حذف جميع الرسائل المعلقة؟')) return
    deleteAllMutation.mutate(undefined, {
      onSuccess: () => {
        setSelectedIds(new Set())
        setMessage('تم حذف جميع الرسائل')
      },
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

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <PageHeader
        title="عرض رسائل الواتساب"
        description="رسائل قيد الانتظار في جدول الإرسال — تحديد وحذف"
        className="mb-0"
      />

      {message ? <Alert>{message}</Alert> : null}
      {query.isError ? <Alert variant="destructive">تعذر تحميل الرسائل المعلقة</Alert> : null}

      <section className="rounded-xl border bg-white p-5 shadow-sm">
        <div className="flex flex-wrap items-center gap-2">
          <Button type="button" variant="outline" onClick={() => toggleAll(true)}>تحديد الكل</Button>
          <Button type="button" variant="outline" onClick={() => setSelectedIds(new Set())}>إلغاء التحديد</Button>
          <Button type="button" className="bg-red-600 text-white hover:opacity-90" onClick={handleDeleteSelected}>حذف المحدد</Button>
          <Button type="button" className="bg-red-600 text-white hover:opacity-90" onClick={handleDeleteAll}>حذف الكل</Button>
          <span className="font-semibold text-[var(--color-primary)]">{countLabel}</span>
        </div>
      </section>

      {items.length === 0 ? (
        <div className="rounded-xl border bg-white p-12 text-center text-slate-600 shadow-sm">
          لا توجد رسائل معلقة في جدول الواتساب.
        </div>
      ) : (
        <WhatsappPendingTable
          items={items}
          selectedIds={selectedIds}
          onToggle={toggleId}
          onToggleAll={toggleAll}
        />
      )}
    </div>
  )
}
