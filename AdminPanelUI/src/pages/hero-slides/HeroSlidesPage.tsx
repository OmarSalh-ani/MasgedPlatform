import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Plus } from 'lucide-react'
import { PageHeader } from '@/components/shared/PageHeader'
import { Alert } from '@/components/ui/alert'
import { Skeleton } from '@/components/ui/skeleton'
import { useHeroSlides } from '@/hooks/useHeroSlides'
import { HeroSlidesTable } from '@/pages/hero-slides/HeroSlidesTable'
import { DeleteHeroSlideDialog } from '@/pages/hero-slides/dialogs/DeleteHeroSlideDialog'

export function HeroSlidesPage() {
  const { query, deleteMutation } = useHeroSlides()
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
        تعذر تحميل قائمة صور الهيرو. يرجى المحاولة مرة أخرى.
      </Alert>
    )
  }

  const items = query.data ?? []

  return (
    <div>
      <PageHeader
        title="صور بانر الهيرو"
        description="إدارة صور الخلفية المتغيرة في قسم الهيرو بالصفحة الرئيسية (كل 3 ثوانٍ)"
        actions={
          <Link
            to="/hero-slides/new"
            className="inline-flex items-center gap-2 rounded-full bg-white/20 px-5 py-2.5 font-semibold text-white hover:bg-white/30"
          >
            <Plus className="size-4" />
            إضافة صورة
          </Link>
        }
      />

      {deleteMutation.isError && (
        <Alert variant="destructive" className="mb-4">
          تعذر حذف الصورة. يرجى المحاولة مرة أخرى.
        </Alert>
      )}

      <HeroSlidesTable items={items} emptyMessage="لا توجد صور" onDelete={setDeleteId} />

      <DeleteHeroSlideDialog
        open={deleteId !== null}
        onOpenChange={(open) => !open && setDeleteId(null)}
        onConfirm={handleDeleteConfirm}
        isPending={deleteMutation.isPending}
      />
    </div>
  )
}
