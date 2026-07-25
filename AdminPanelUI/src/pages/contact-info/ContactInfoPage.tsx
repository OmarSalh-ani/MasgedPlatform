import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Contact, Plus } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useContactInfos } from '@/hooks/useContactInfos'
import { ContactInfoTable } from '@/pages/contact-info/ContactInfoTable'
import { DeleteContactInfoDialog } from '@/pages/contact-info/dialogs/DeleteContactInfoDialog'

export function ContactInfoPage() {
  const { query, deleteMutation } = useContactInfos()
  const [deleteId, setDeleteId] = useState<number | null>(null)

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
        تعذر تحميل قائمة بيانات التواصل. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  const items = query.data ?? []

  return (
    <div>
      <PageHeader
        title="تواصل معنا"
        description="إدارة بيانات التواصل (هاتف، واتساب، بريد، عنوان)"
        actions={
          <Link
            to="/contact-info/new"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <Plus className="size-4" />
            إضافة
          </Link>
        }
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف بيانات التواصل. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <div className="overflow-hidden rounded-xl bg-white shadow-md">
        <div className="bg-[#7C8738] px-5 py-4 font-semibold text-white">
          قائمة بيانات التواصل
        </div>
        {items.length > 0 ? (
          <ContactInfoTable items={items} onDelete={setDeleteId} />
        ) : (
          <div className="p-8 text-center text-slate-500">
            <Contact className="mx-auto mb-3 size-12 opacity-50" />
            <p>
              لا توجد بيانات.{' '}
              <Link to="/contact-info/new" className="text-[var(--color-primary)] underline">
                إضافة
              </Link>
            </p>
          </div>
        )}
      </div>

      <DeleteContactInfoDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
